using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LeName : MonoBehaviour //sorry
{

    public GameObject UpgradesMenu;
    int Option_1;
    int Option_2;
    int Option_3;

    // Start is called before the first frame update
    void Start()
    {
       UpgradesMenu.SetActive(false);


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FilthySubroutine()
    {
        OpenMenu();

    }

    public void OpenMenu()
    {
        UpgradesMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PlayerController.disableInputs = false;

        Random rnd = new Random();

        Option_1 = rnd.Next(0, 13); //INCREASE THIS NUMBER WHEN ADDING UPGRADES
        Option_2 = rnd.Next(0, 13);
        Option_3 = rnd.Next(0, 13);


    }

    public void CloseMenu()
    {
        UpgradesMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
        PlayerController.disableInputs = true;
    }

    private void CallUpgrade(int Upgrade) //ADD UPGRADE SCRIPTS HERE
    {
        switch(Upgrade)
        {
            case 1:
            IncreaseHealth();
            break;

            case 2:
                IncreaseStamina();
                break;

            case 3:
                IncreaseIRR();
                break;

            case 4:
                IncreaseDodgeDistance();
                break;

            case 5:
                IncreaseJumpheight();
                break;

            case 6:
                ToggleStaminaTimer();
                break;

            case 7:
                ReduceComboDelay();
                break;

            case 8:
                ReduceExplosionDamage();
                break;

            case 9:
                IncreaseADMG();
                break;

            case 10:
                IncreaseLaserDMG();
                break; 

                case 11:
                IncreaseLaserCost();
                break;

            case 12:
                IncreaseFlightCost();
                break;

            default:
                //ur a dumbass if this gets reached :)
               break;



        }
    }

    public void Option1()
    {
        CallUpgrade(Option_1);
    }

    public void Option2()
    {
        CallUpgrade(Option_2);
    }

    public void Option3()
    {
        CallUpgrade(Option_3);
    }

    //========= UPGRADES!!! =====================

    public void IncreaseHealth(int Percentage = 10) //1
    {
        playerHealth += (playerHealth / 100 * Percentage);
    }

    public void IncreaseStamina(int Percentage = 20)//2
    {
        MaxEnergy += (MaxEnergy / 100 * Percentage);
    }

    public void IncreaseIRR(int Percentage = 15) //stands for StaminaRegenRate 3
    {
        energyRegenRate += (energyRegenRate / 100 * Percentage);
    }

    public void IncreaseDodgeDistance(int Percentage = 10) //4
    {
        rollForce += (rollForce / 100 * Percentage);
    }

    public void IncreaseJumpheight(int Percentage = 20) //5
    {
        jumpForce += (jumpForce / 100 * Percentage);
    }

    /*public void IncreaseDodgeConsumption(int Percentage = -35)
    {
        VARIABLE:)
    }
    */

    public void ToggleStaminaTimer() //6
    {
        //ADD VARIABLE BC I CANT FIND IT
    }

    public void ReduceComboDelay(int Percentage) //7
    {
        //ADD VARIABLE BC I CANT FIND IT
    }

    public void ReduceExplosionDamage(int Percentage = 20) //WILL also reduce damage taken by hostiles 8
    {
        blastDamage += (blastDamage / 100 * Percentage);
    }

    public void IncreaseADMG(int Percentage = 15)//9
    {
        punchDamage += (punchDamage / 100 * Percentage);
    }

    public void IncreaseLaserDMG(int Percentage = 10)//10
    {
        laserDamage += (laserDamage / 100 * Percentage);
    }

    public void IncreaseLaserCost(int Percentage = -15)//11
    {
        laserDrain += (laserDrain / 100 * Percentage);
    }

    public void IncreaseFlightCost(int Percentage = -20)//12
    {
        flightDrain += (flightDrain / 100 * Percentage);
    }


}
