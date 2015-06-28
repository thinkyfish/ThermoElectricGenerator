//Simple ThermoNuclearGenerator PartModule
//Author: Tristan MacCrehan
//License: MIT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;


public class ThermoNuclearGenerator : PartModule
{
	[KSPField(isPersistant = true)]
	public double  installtime = 0;

	[KSPField] public string ResourceName;

	[KSPField] public double StartFuel = 1.0;

	[KSPField] public double HalfLife = 1.0;

	[KSPField] public double BaseRate = 1.0;

	[KSPField(isPersistant = false, guiActive = true, guiFormat = "F2", guiName = "Energy Output")]
	public float Output;

	[KSPField(isPersistant = false, guiActive = true, guiUnits = " Years", guiFormat = "F3", guiName = "Half Life")]
	public float HalfYears;

	public double RemainingFuel(){
		// Here is the half life equation.
		return ((double)StartFuel) * Math.Pow (0.5,this.getAge()/(double)HalfLife);
	}

	public double getRate(){
		//The idea is that we don't care about how much there is left really, just the ratio
		//compared to the start value. The BaseRate actually governs the inital power output.
		return (double)BaseRate * (RemainingFuel () / (double)StartFuel);
	}

	public double getAge(){
		//installtime minus the current game time give the age in seconds
		return Planetarium.GetUniversalTime() - (double)installtime;	
	}

	public override void OnStart(PartModule.StartState state)
	{
		if(this.installtime == 0.0 && state.Equals(PartModule.StartState.PreLaunch))
		{
			//if we haven't installed the part yet, we set the installtime to current game time
			this.installtime = (double)Planetarium.GetUniversalTime ();
		}	

		//Get the number of years from the half-life by dividing # of seconds in a year.
		HalfYears = (float)(HalfLife / 9203400.0); //Kerbin year
		//HalfYears = (float)(HalfLife / 31540000.0);  //earth year

	}

	public void FixedUpdate()
	{

		if(HighLogic.LoadedSceneIsFlight) //Fixes Null Bug in Editor
		{
			Output = (float)getRate();
			// we multiply the Output rate against the physics step to get the amount to add
			// and we multiply by -1 to indicate we are adding power, not removing it.
			this.part.RequestResource(this.ResourceName, Output * TimeWarp.fixedDeltaTime * -1.0);
		}
	}
}