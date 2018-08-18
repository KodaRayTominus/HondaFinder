using HondaFinder.entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HondaFinder
{
	public static class VehicleDB
	{
		/// <summary>
		/// adds a Vehical Object to the dataBase
		/// </summary>
		/// <param name="v"></param>
		public static void AddVehicle(Vehicle v)
		{
			//creates db context
			HondaDBContext context = new HondaDBContext();
			//adds the the object to database
			context.Vehicles.Add(v);
			//saves the changes
			context.SaveChanges();
		}

        /// <summary>
        /// Gets all vehicles and returns them as a list
        /// </summary>
        /// <returns>List of all vehices in database</returns>
        public static IQueryable<Vehicle> GetAllVehicles()
        {
            HondaDBContext context = new HondaDBContext();

            return context.Vehicles
                            .OrderBy(c => c.Model);
        }

        /// <summary>
        /// Get a specific Vehicle by id
        /// </summary>
        /// <param name="id">id number of the vehicl you are looking for</param>
        /// <returns>vehicle object of searched object</returns>
        public static Vehicle GetVehicle(int id)
        {
            HondaDBContext context = new HondaDBContext();

            return context.Vehicles.Find(id);
        }

        /// <summary>
        /// Gets Vehicle by SearchForm Parameters, if none found, calls back up search until atleast on vehicle is found
        /// </summary>
        /// <param name="model">Required model of vehicle you are searching for</param>
        /// <param name="year"> year of vehicle you are looking for</param>
        /// <param name="mileage">mileage of the vehicle you are looking for</param>
        /// <param name="condition">condition of the vehicle you are looking for</param>
        /// <param name="priceMin">minimum price for vehicle you are looking for</param>
        /// <param name="priceMax">maximum price for vehicle you are looking for</param>
        /// <param name="color">color of vehicle you are looking for</param>
        /// <returns></returns>
        public static List<Vehicle> GetBySearchWindowParameters(string model, int? year, int? mileage, string condition, 
                                                                double? priceMin, double? priceMax, string color)
        {
            HondaDBContext context = new HondaDBContext();

            
            if(model != null)
            {
                var vehicles = (from v in context.Vehicles
                                where v.Model == model
                                select v);
                if(year != null)
                {
                    vehicles = GetVehiclesByYear(Convert.ToInt32(year), ref context, ref vehicles);
                }

                if(mileage != null)
                {
                    vehicles = GetVehiclesByMileage(Convert.ToInt32(mileage), ref context, ref vehicles);
                }

                if(condition != null)
                {
                    vehicles = GetVehiclesByCondition(condition, ref context, ref vehicles);
                }

                if(priceMin != null || priceMax != null)
                {
                    vehicles = GetVehiclesByPrice(Convert.ToDouble(priceMin), Convert.ToDouble(priceMax), ref context, ref vehicles);
                }

                if(color != null)
                {
                    vehicles = GetVehiclesByColor(color, ref context, ref vehicles);
                }
                if(vehicles.ToList().Count == 0)
                {
                    vehicles = BackUpSearch(model, mileage, priceMin, priceMax, ref context, ref vehicles);
                }

                return vehicles.ToList();

            }
                return new List<Vehicle>();
        }

        /// <summary>
        /// back up search, used when main search returns no results
        /// </summary>
        /// <param name="model">model name of vehicle to search for</param>
        /// <param name="mileage">base mileage of vehicle to search for</param>
        /// <param name="priceMin"> base price for vehicle to search for</param>
        /// <param name="priceMax"> base max price for vehicle to search for</param>
        /// <param name="context">reference of DBContext Class</param>
        /// <param name="vehicles">reference to the Query</param>
        /// <returns>query of vehicles found</returns>
        private static IQueryable<Vehicle> BackUpSearch(string model, int? mileage, double? priceMin, double? priceMax, ref HondaDBContext context, ref IQueryable<Vehicle> vehicles)
        {
            context.Dispose();
            
            context = new HondaDBContext();


            int backUpMileageIncrementor = 5000;

            double backUpPriceIncrementor = 1000;


            int? backUpMileage = mileage;
            double? backUpPrice = priceMax;

            while (vehicles.ToList().Count == 0)
            {
                backUpMileage += backUpMileageIncrementor;

                backUpPrice += backUpPriceIncrementor;
                
                vehicles = null;

                vehicles = (from v in context.Vehicles
                            where v.Model == model
                            select v);

                if (backUpMileage != null)
                {
                    vehicles = GetVehiclesByMileage(Convert.ToInt32(backUpMileage), ref context, ref vehicles);

                }

                if (priceMin != null || backUpPrice != null)
                {
                    vehicles = GetVehiclesByPrice(Convert.ToDouble(priceMin), Convert.ToDouble(backUpPrice), ref context, ref vehicles);
                }
            }
            return vehicles;
        }

        /// <summary>
        /// gets vehicles by color
        /// </summary>
        /// <param name="color">vehicle color to be searched for</param>
        /// <param name="context">DBContext to be used, if passed null, one will be created</param>
        /// <param name="vehicles">Query of vehicles from context, must at least call GetAllVehicles before use</param>
        /// <returns>list of vehicles found</returns>
        public static IQueryable<Vehicle> GetVehiclesByColor(string color, ref HondaDBContext context, ref IQueryable<Vehicle> vehicles)
        {
            if(context == null)
            {
                context = new HondaDBContext();
            }

            vehicles = (from v in context.Vehicles
                            where v.Color == color
                            select v);

            return vehicles;
        }

        /// <summary>
        /// finds vehicles by condition
        /// </summary>
        /// <param name="condition">vehicle condition to be searched for</param>
        /// <param name="context">DBContext to be used, if passed null, one will be created</param>
        /// <param name="vehicles">Query of vehicles from context, must at least call GetAllVehicles before use</param>
        /// <returns></returns>
        public static IQueryable<Vehicle> GetVehiclesByCondition(string condition, ref HondaDBContext context, ref IQueryable<Vehicle> vehicles)
        {
            if (context == null)
            {
                context = new HondaDBContext();
            }

            vehicles = (from v in context.Vehicles
                            where v.Condition == condition
                            select v);

            return vehicles;
        }

        /// <summary>
        /// find vehicles by model
        /// </summary>
        /// <param name="model">model of vehicle to be searched for</param>
        /// <param name="context">DBContext to be used, if passed null, one will be created</param>
        /// <param name="vehicles">Query of vehicles from context, must at least call GetAllVehicles before use</param>
        /// <returns></returns>
        public static IQueryable<Vehicle> GetVehiclesByModel(string model, ref HondaDBContext context, ref IQueryable<Vehicle> vehicles)
        {
            if (context == null)
            {
                context = new HondaDBContext();
            }

            vehicles = (from v in context.Vehicles
                         where v.Model == model
                         select v);

            return vehicles;
        }

        /// <summary>
        /// finds vehicles by year
        /// </summary>
        /// <param name="year">year of vehicle to be searched for</param>
        /// <param name="context">DBContext to be used, if passed null, one will be created</param>
        /// <param name="vehicles">Query of vehicles from context, must at least call GetAllVehicles before use</param>
        /// <returns></returns>
        public static IQueryable<Vehicle> GetVehiclesByYear(int year, ref HondaDBContext context, ref IQueryable<Vehicle> vehicles)
        {
            if (context == null)
            {
                context = new HondaDBContext();
            }

            vehicles = (from v in context.Vehicles
                                      where v.Year == year
                                      select v);

            return vehicles;
        }

        /// <summary>
        /// find vehicles by min and max pricing
        /// </summary>
        /// <param name="lowPrice">lowest price to search for vehicle</param>
        /// <param name="highPrice">max price to search for vehicle</param>
        /// <param name="context">DBContext to be used, if passed null, one will be created</param>
        /// <param name="vehicles">Query of vehicles from context, must at least call GetAllVehicles before use</param>
        /// <returns></returns>
        public static IQueryable<Vehicle> GetVehiclesByPrice(double lowPrice, double highPrice, ref HondaDBContext context, ref IQueryable<Vehicle> vehicles)
        {
            if (context == null)
            {
                context = new HondaDBContext();
            }

            vehicles = (from v in context.Vehicles
                                      where v.Price <= highPrice && v.Price >= lowPrice
                                      select v);

            return vehicles;
        }

        /// <summary>
        /// finds vehicles by mileage
        /// </summary>
        /// <param name="mileage">mileage of vehicle to search for</param>
        /// <param name="context">DBContext to be used, if passed null, one will be created</param>
        /// <param name="vehicles">Query of vehicles from context, must at least call GetAllVehicles before use</param>
        /// <returns></returns>
        public static IQueryable<Vehicle> GetVehiclesByMileage(int mileage, ref HondaDBContext context, ref IQueryable<Vehicle> vehicles)
        {
            if (context == null)
            {
                context = new HondaDBContext();
            }

            vehicles = (from v in context.Vehicles
                                      where v.Mileage < mileage
                                      select v);

            return vehicles;
        }

        /// <summary>
        /// updates a vehicle in the database
        /// </summary>
        /// <param name="v">vehicle to update</param>
        public static void Update(Vehicle v)
        {
            HondaDBContext context = new HondaDBContext();

            //tell EF this product has only been modified
            //its already in the db
            context.Entry(v).State = EntityState.Modified;

            //sends update query to the database
            context.SaveChanges();
        }
    }
}
