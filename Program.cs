using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Misc;

namespace cs
{

    class Program
    {

        public interface IPersonDal
        {
            void GetAll(); // Method to get all products
            person GetById(int id); // Method to get a product by ID
            void Add(person person); // Method to add a new product
            void Count(); // Method to count the total number of products
            void Add(int id, string name, int price); // Method to add a product with specific details
            void Update(person p); // Method to update a product
            void Delete(int id); // Method to delete a product
        }
        class MySQLDal : IPersonDal
        {
            List<person> persons = new List<person>();


            MySqlConnection DatabaseConnection()
            {
                string connectionString = @"server=localhost;port=3306;database=new;user=root;password=root;";
                return new MySqlConnection(connectionString);

            }
            List<person> GetMySqlConnection()
            {

                var connection = DatabaseConnection();

                try
                {
                    connection.Open();
                    System.Console.WriteLine("SUCCESSFULLY CONNECTED");
                    string sql = "Select * from products";
                    MySqlCommand comm = new MySqlCommand(sql, connection);
                    MySqlDataReader read = comm.ExecuteReader();
                    while (read.Read())
                    {



                        persons.Add(new person() { id = int.Parse(read["id"].ToString()), name = read["name"].ToString(), price = int.Parse(read[2].ToString()) });


                    }
                    read.Close();
                }
                catch (Exception e)
                {
                    System.Console.WriteLine("ERROR MESSAGE::" + e.Message);


                }
                finally
                {
                    connection.Close();

                }
                return persons;



            }

            public void GetAll()
            {
                if (persons.Count == 0) // IF DATA HAS NOT BEEN TAKEN YET
                {
                    GetMySqlConnection(); // Veriyi çek
                }

                foreach (var i in persons)
                {
                    System.Console.WriteLine($"{i.name} {i.price}");

                }


            }
            public person GetById(int id)
            {
                person person = null;


                using (var connection = DatabaseConnection())
                {

                    try
                    {
                        connection.Open();
                        System.Console.WriteLine("SUCCESSFULLY CONNECTED.");

                        string sql = "SELECT * FROM products WHERE id = @id";
                        MySqlCommand command = new MySqlCommand(sql, connection);
                        command.Parameters.AddWithValue("@id", id);
                        MySqlDataReader read = command.ExecuteReader();
                        read.Read();
                        if (read.HasRows)
                        {
                            person = new person() { id = id, name = read["name"].ToString(), price = int.Parse(read["fiyat"].ToString()) };
                            System.Console.WriteLine($"WE FOUND THE PERSON WHO HAS THE İD **{id}**.");
                        }
                        else
                        {
                            System.Console.WriteLine("THERE IS NO ONE WITH THIS ID");
                        }
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine(e.Message);
                    }
                    return person;
                }
            }
            public void Add(person person)
            {
                try
                {
                    persons.Add(person);
                    System.Console.WriteLine("SUCCESSFULLY ADDED NEW PERSON");
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.Message);
                }

            }
            public void Count()
            {
                using (var conn = DatabaseConnection())
                {
                    string sql = "Select count(*) from products";
                    conn.Open();
                    var comm = new MySqlCommand(sql, conn);
                    var result = comm.ExecuteScalar();
                    if (result != null)
                    {
                        result = int.Parse(comm.ExecuteScalar().ToString());
                        System.Console.WriteLine($"WE CAN SUCCESSFULLY REACH THE DATABASE YOU HAVE TOTTALY **{result}** DATA İN DATABASE");
                    }
                    else
                    {
                        System.Console.WriteLine("CANNOT FETCH THE DATA FROM DATABASE");
                    }


                }
            }

            public void Add(int id, string name, int price)
            {
                using (var conn = DatabaseConnection())
                {
                    conn.Open();
                    string add = $"insert into products (id,name,fiyat) values({id},'{name}',{price})";
                    MySqlCommand command = new MySqlCommand(add, conn);
                    try
                    {
                        command.ExecuteNonQuery();
                        System.Console.WriteLine($"İD:{id} NAME:{name} PRİCE : {price} SUCCESSFULY ADDED.");
                        System.Console.WriteLine($"NEW PRODUCT COUNT");
                        Count();

                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine($"WARNİNG:: {e.Message}");
                    }


                }



            }

            public void Update(person p)
            {
                using (var conn = DatabaseConnection())
                {
                    string sql = $"update products set name='{p.name}',fiyat={p.price} where id={p.id}";
                    conn.Open();
                    var comm = new MySqlCommand(sql, conn);

                    try
                    {
                        comm.ExecuteNonQuery();
                        System.Console.WriteLine("YOU HAVE SUCCESSFULLY UPDATED FİELDS");

                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine(e.Message);
                    }



                }
            }

            public void Delete(int id)
            {
                using (var conn = DatabaseConnection())
                {
                    conn.Open();
                    var sql = $"delete  from products where id=@id";
                    var comm = new MySqlCommand(sql, conn);
                    comm.Parameters.AddWithValue("@id", id);

                    try
                    {
                        comm.ExecuteNonQuery();
                        System.Console.WriteLine($"THE ELEMENT WİTH ID **{id}** HAS BEEN SUCCESSFULLY DELETED");
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine(e.Message);
                    }

                }

            }
        }
        class PersonManager : IPersonDal
        {
            IPersonDal _IpersonDal;
            public PersonManager(IPersonDal personDal)
            {
                _IpersonDal = personDal;
            }
            public void Add(person person)
            {
                throw new NotImplementedException();
            }
            public void GetAll()
            {
                _IpersonDal.GetAll();
            }

            public person GetById(int id)
            {
                return _IpersonDal.GetById(id);
            }
            public void Count()
            {
                _IpersonDal.Count();
            }

            public void Add(int id, string name, int price)
            {
                _IpersonDal.Add(id, name, price);
            }

            public void Update(person p)
            {
                _IpersonDal.Update(p);
            }

            public void Delete(int id)
            {
                _IpersonDal.Delete(id);
            }
        }

        static void Main(string[] args)
        {
            /*SOME EXAMPLES  */

            PersonManager person = new PersonManager(new MySQLDal());
            person.Count();
            person.Add(6, "NEW NAME", 234);
            person p = new person() { id = 1, name = "UPDATED", price = 5 };
            person.Update(p);
            person.Delete(5);







        }




    }








}
