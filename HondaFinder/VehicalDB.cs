using HondaFinder.entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HondaFinder
{
	public static class VehicalDB
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

        public static List<Vehicle> GetAllVehicles()
        {
            HondaDBContext context = new HondaDBContext();

            return context.Vehicles
                            .OrderBy(c => c.Model)
                            .ToList();
        }

        public static Vehicle GetVehicle(int id)
        {
            HondaDBContext context = new HondaDBContext();

            return context.Vehicles.Find(id);
        }

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
                vehicles = GetVehiclesByYear(Convert.ToInt32(year));
            }

            if(mileage != null)
            {
                vehicles = GetVehiclesByMileage(Convert.ToInt32(mileage));
            }

            if(condition != null)
            {
                vehicles = GetVehiclesByCondition(condition);
            }

            if(priceMin != null)
            {
                vehicles = GetVehiclesByPrice(Convert.ToDouble(priceMin), Convert.ToDouble(priceMax));
            }

            if(color != null)
            {
                vehicles = GetVehiclesByColor(color);
            }

                return vehicles.ToList();

            }
                return new List<Vehicle>();
        }

        private static IQueryable<Vehicle> GetVehiclesByColor(string color)
        {
            HondaDBContext context = new HondaDBContext();

            var vehicles = (from v in context.Vehicles
                            where v.Color == color
                            select v);

            return vehicles;
        }

        private static IQueryable<Vehicle> GetVehiclesByCondition(string condition)
        {
            HondaDBContext context = new HondaDBContext();

            var vehicles = (from v in context.Vehicles
                            where v.Condition == condition
                            select v);

            return vehicles;
        }

        public static IQueryable<Vehicle> GetVehiclesByModel(string model)
        {
            HondaDBContext context = new HondaDBContext();

            var vehicles = (from v in context.Vehicles
                         where v.Model == model
                         select v);

            return vehicles;
        }

        public static IQueryable<Vehicle> GetVehiclesByYear(int year)
        {
            HondaDBContext context = new HondaDBContext();

            var vehicles = (from v in context.Vehicles
                                      where v.Year == year
                                      select v);

            return vehicles;
        }

        public static IQueryable<Vehicle> GetVehiclesByPrice(double lowPrice, double highPrice)
        {
            HondaDBContext context = new HondaDBContext();

            var vehicles = (from v in context.Vehicles
                                      where v.Price <= highPrice && v.Price >= lowPrice
                                      select v);

            return vehicles;
        }

        public static IQueryable<Vehicle> GetVehiclesByMileage(int mileage)
        {
            HondaDBContext context = new HondaDBContext();

            var vehicles = (from v in context.Vehicles
                                      where v.Mileage < mileage
                                      select v);

            return vehicles;
        }

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
