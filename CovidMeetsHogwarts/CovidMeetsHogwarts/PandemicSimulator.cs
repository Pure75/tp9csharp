using System;
using System.Collections.Generic;
using System.Linq;

namespace CovidMeetsHogwarts
{
    public class PandemicSimulator
    {
        public static List<Human> infectious;
        
        /// <summary>
        /// initialize pandemic by infecting a random human by the corona virus in given location.
        /// </summary>
        /// <param name="location">location where the pandemic is simulated</param>
        public static void InitializePandemic(Location location)
        {
            infectious = new List<Human>();
            Random rnd = new Random();
            Human human = location.GetHumans()[rnd.Next(0,location.GetHumans().Count)];
            human.SetVirus(new Virus(" Covid-19",0.6,3,14));
            human.SetSir(Human.SIR.INFECTIOUS);
            infectious.Add(human);
            
        }

        /// <summary>
        /// move/travel given human to a neighboring spot according to their
        /// travelling rate.
        /// </summary>
        /// <param name="human">human to move (or not)</param>
        static void MoveAround(Human human)
        {
            if (PandemicSimulator.IfProbabilities(human.GetTravellingRate()))
            {            
                Random rnd = new Random();
                Node currentSpot = human.GetCurrentSpot();
                Node voisin = currentSpot.GetNeighbors()[rnd.Next(0,currentSpot.GetNeighbors().Count)];
                currentSpot.GetHumans().Remove(human);
                
                human.SetCurrentSpot(voisin);
            }
            
            
        }

        /// <summary>
        /// try to infect susceptible humans at the transmitter's spot.
        /// the following factors are taken into account:
        ///     - the virus' infection range
        ///     - the virus's transmission rate
        ///     - the average hygiene between the transmitter and the susceptible human
        ///     - the distance between the transmitter and the susceptible human (also average of social distance)
        /// </summary>
        /// <param name="transmitter">the human carrying the virus</param>
        /// <param name="justGotInfected">the list of humans to update when someone gets infected</param>
        static void InfectOthers(Human transmitter, List<Human> justGotInfected)
        {
            int infectionRange = transmitter.GetVirus().GetInfectionRange();
            int nombreDessaieTotal=Math.Min(infectionRange, transmitter.GetCurrentSpot().GetHumans().Count - 1);
            int nombreDessaieEffectue = 0;
            foreach (Human human in transmitter.GetCurrentSpot().GetHumans())
            {
                if (nombreDessaieEffectue == nombreDessaieTotal)
                {
                    break;
                }
                if (IfProbabilities(transmitter.GetVirus().GetTransmissionRate()))
                {
                    if (human.GetSir() != Human.SIR.SUSCEPTIBLE )
                    {
                        nombreDessaieEffectue++;
                        if(human==transmitter)
                        {
                            nombreDessaieEffectue--;
                            
                        }
                        continue;
                    }

                    double moyenneHygiene = (human.GetHygiene() + transmitter.GetHygiene()) / 2;
                    double moyenneSocialDistance= (human.GetSocialDistance() + transmitter.GetSocialDistance()) / 2;
                    if (!IfProbabilities(moyenneHygiene) && !IfProbabilities(moyenneSocialDistance))
                    {
                        human.SetSir(Human.SIR.INFECTIOUS);
                        human.SetVirus(new Virus(" Covid-19",0.6,3,14));
                        justGotInfected.Add(human);
                    }

                }
                else
                {
                    nombreDessaieEffectue++;
                }
            }
            
            
        }
        
        /// <summary>
        /// update pandemic by a unit of time at given location.
        ///     - infectious humans will infect around them
        ///     - some of the infectious humans will heal/die if enough days have passed
        ///     - some humans will travel to a neighboring spot
        /// the infectious list should be updated as well
        /// </summary>
        /// <param name="location">location where the pandemic is simulated</param>
        /// <returns>return number of infectious humans at this round</returns>
        public static int UpdatePandemic(Location location)
        {
            List<Human> infected = new List<Human>(infectious);

            foreach (Human human in infected)
            {
                human.GetVirus().SetLifetime(human.GetVirus().GetLifetime()-1);
                if (human.GetVirus().GetLifetime() > 0)
                {
                    List<Human> justGotInfected=new List<Human>();
                    InfectOthers(human,justGotInfected);
                    foreach (Human hum in justGotInfected)
                    {
                        if (hum.GetSir() == Human.SIR.INFECTIOUS)
                        {
                            infectious.Add(hum);
                        }
                        
                    }
                }
                else
                {
                    human.SetSir(Human.SIR.REMOVED);
                    human.SetVirus(null);
                    
                    infectious.Remove(human);
                }
                location.GetHumans().ForEach(hum=>MoveAround(hum));
            }

            return infectious.Count;

        }

        private static bool IfProbabilities(double param)
        {
            Random rnd = new Random();
            int rand = rnd.Next(100);
            if (param * 100 > rand)
            {
                return false;
            }
            return true;
        }
    }
}