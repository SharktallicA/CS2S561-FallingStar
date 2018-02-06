﻿/*
    purpose: handles probe movement and gravity application
    usage: attached to probe
*/

using UnityEngine;
using UnityEngine.UI;

public class ShipMovement : MonoBehaviour
{
    public float moveSpeed = 10000; //normal move speed
    public float superSpeed = 10000; //warped speed
    public float normalFuelRate = 1f; //fuel consumption rate for normal move speed
    public float superFuelRate = 2.5f; //fuel consumption rate for warped speed
    public GameObject lensflare; //reference to lens flare
    public GameObject particles; //reference to particles
    public GameObject speedometer; //reference to speedometer object
    public GameObject fuelcounter; //reference to fuelcounter object
    private float previousSpeed = 0f;

    void FixedUpdate()
    {
        //purpose: conducts probe movement if certain key(s) are pressed

        if (GetComponent<ProbeVariables>().GetFuel() > 0)
        {
            if (Input.GetKey(KeyCode.LeftShift) & Input.GetKey(KeyCode.W)) //if both keys W and LShift are held
            {
                //apply additional force in direction of local Z axis
                GetComponent<Rigidbody>().AddRelativeForce(0, 0, superSpeed * Time.deltaTime, ForceMode.Acceleration);

                //update fuel
                GetComponent<ProbeVariables>().SetFuel(GetComponent<ProbeVariables>().GetFuel() - superFuelRate);

                //enable engine effects
                lensflare.GetComponent<LensFlare>().brightness = 1.5f;
                particles.GetComponent<ParticleSystem>().Play();
            }
            else if (Input.GetKey(KeyCode.S)) //if key S is held
            {
                if (previousSpeed > GetComponent<ProbeVariables>().GetCurrentSpeed()) //slow down whilst speed is decreasing
                {
                    //slow down probe
                    GetComponent<Rigidbody>().AddRelativeForce(0, 0, -moveSpeed * Time.deltaTime, ForceMode.Acceleration);

                    //update fuel
                    GetComponent<ProbeVariables>().SetFuel(GetComponent<ProbeVariables>().GetFuel() - superFuelRate);

                    //disable engine effects
                    lensflare.GetComponent<LensFlare>().brightness = 0f;
                    particles.GetComponent<ParticleSystem>().Stop();
                }
            }
            else if (Input.GetKey(KeyCode.Q)) //if key Q is held
            {
                //bring probe to a stop
                GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);

                //disable engine effects
                lensflare.GetComponent<LensFlare>().brightness = 0f;
                particles.GetComponent<ParticleSystem>().Stop();
            }
            else if (Input.GetKey(KeyCode.W)) //if key W is held
            {
                //apply force in positive direction of local Z axis
                GetComponent<Rigidbody>().AddRelativeForce(0, 0, moveSpeed * Time.deltaTime, ForceMode.Acceleration);

                //update fuel
                GetComponent<ProbeVariables>().SetFuel(GetComponent<ProbeVariables>().GetFuel() - normalFuelRate);

                //enable engine effects
                lensflare.GetComponent<LensFlare>().brightness = 1f;
                particles.GetComponent<ParticleSystem>().Play();
            }
            else
            {
                //disable all engine effects
                lensflare.GetComponent<LensFlare>().brightness = 0;
                particles.GetComponent<ParticleSystem>().Stop();
            }
        }
        else
        {
            //disable all engine effects
            lensflare.GetComponent<LensFlare>().brightness = 0;
            particles.GetComponent<ParticleSystem>().Stop();
        }

        //manage fuel tasks
        if (fuelcounter != null)
        {
            fuelcounter.GetComponent<Text>().text = "Fuel: " + GetComponent<ProbeVariables>().GetFuelRounded().ToString();
        }

        //manage speed tasks
        if (speedometer != null)
        {
            speedometer.GetComponent<Text>().text = "Speed: " + GetComponent<ProbeVariables>().GetCurrentSpeed().ToString() + " KP/H";
        }

        //store current speed to check in the next FixedUpdate call so that the script can ensure the slowdown function doesn't reverse the probe
        previousSpeed = GetComponent<ProbeVariables>().GetCurrentSpeed();
    }
}