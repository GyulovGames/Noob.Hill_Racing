using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;

public class Car0 : MonoBehaviour
{
    [SerializeField] private float fuelConsuptionRate;
    [SerializeField] private float gasFuelConsuptionRate;
    [SerializeField] private float curentFuelAmount;
    [SerializeField] private float vehicleRotationSpeed;
    [Space(5)]
    [Header("Upgrade Multipliers")]
    [SerializeField] private float engineForce;
    [SerializeField] private float carDampinRatio;
    [SerializeField] private float frontWheelsForce;
    [SerializeField] private float wheelsGrip;
    [SerializeField] private float fuelVolume;
    [Space(5)]
    [SerializeField] private Animator noobAnimator;
    [SerializeField] private Rigidbody2D vehicleRigidBody;
    [SerializeField] private Rigidbody2D frontWheel_RigidBody;
    [SerializeField] private Rigidbody2D rearWheel_RigidBody;
    [SerializeField] private WheelJoint2D frontWheel_Joint;
    [SerializeField] private WheelJoint2D rearWheel_Joint;
    [SerializeField] private CircleCollider2D frontWheel_CircleCollider;
    [SerializeField] private CircleCollider2D rearWheel_CircleCollider;
    [Space(5)]
    [SerializeField] private PhysicsMaterial2D wheelsGripMaterial;

    private bool stop = false;




    private void Start()
    {
        SetCarUpgrades();
        UpdateOnStartUIFuelBar();
    }

    private void SetCarUpgrades()
    {
        engineForce *= YandexGame.savesData.Car0_Upgrades[0];

        JointSuspension2D suspension = frontWheel_Joint.suspension;
        suspension.dampingRatio = carDampinRatio *= YandexGame.savesData.Car0_Upgrades[1];
        frontWheel_Joint.suspension = suspension;
        rearWheel_Joint.suspension = suspension;

        frontWheelsForce *= YandexGame.savesData.Car0_Upgrades[2];

        wheelsGripMaterial.friction = wheelsGrip *= YandexGame.savesData.Car0_Upgrades[3];
        frontWheel_CircleCollider.sharedMaterial = wheelsGripMaterial;
        rearWheel_CircleCollider.sharedMaterial = wheelsGripMaterial;

        fuelVolume *= YandexGame.savesData.Car0_Upgrades[4];
    }
    private void UpdateOnStartUIFuelBar()
    {
        GameCanvas.Instance.UpdateFuelBarOnStart(fuelVolume);
    }



    private void UpdateUIFuelBar(float horizontal)
    {
        if (horizontal > 0 || horizontal < 0)
        {
            curentFuelAmount -= Time.deltaTime * gasFuelConsuptionRate;
        }
        else
        {
            curentFuelAmount -= Time.deltaTime * fuelConsuptionRate;
        }

        if (curentFuelAmount <= 0)
        {
            stop = true;
            StartCoroutine(GameCanvas.Instance.OpenResultWindow("FuelOut"));
        }
    }

    private void NoobAnimations(float value)
    {
        switch (value)
        {
            case 1:
                noobAnimator.SetBool("DrivingForward", true);
                break;
            case -1:
                noobAnimator.SetBool("DrivingBackward", true);
                break;
            default:
                noobAnimator.SetBool("DrivingForward", false);
                noobAnimator.SetBool("DrivingBackward", false);
                break;
        }
    }

    public void HitNoobHead()
    {
        if (!stop)
        {
            stop = true;
            noobAnimator.SetBool("Dead", true);
            StartCoroutine(GameCanvas.Instance.OpenResultWindow("driverCrash"));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Canister")
        {
            print("Full!");
            curentFuelAmount = fuelVolume *= YandexGame.savesData.Car0_Upgrades[4];
            GameCanvas.Instance.UpdateFuelbar(curentFuelAmount);

        }
        if (collision.gameObject.tag == "Coin")
        {
            GameCanvas.Instance.CoinCounter();
        }
    }

    private void FixedUpdate()
    {
        if (!stop)
        {
            float horizontal = GameCanvas.Instance.horizontalInput;
            NoobAnimations(horizontal);
            UpdateUIFuelBar(horizontal);

            rearWheel_RigidBody.AddTorque(-horizontal * engineForce * Time.fixedDeltaTime);
            frontWheel_RigidBody.AddTorque(-horizontal * frontWheelsForce * Time.fixedDeltaTime);
            vehicleRigidBody.AddTorque(horizontal * vehicleRotationSpeed * Time.fixedDeltaTime);
        }
    }
}