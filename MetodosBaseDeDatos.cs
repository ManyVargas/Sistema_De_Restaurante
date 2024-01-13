//Añadiendo referencias necesarias
using System.Data;
using System.Data.SqlClient;
using Sistema_de_Restaurante.MetodosGenericos;

namespace Sistema_de_Restaurante.MetodosBaseDatos
{
    public static class MetodosBaseDeDatos
    {
        //Conexión con base de datos SQL
        private const string connectionString = "Server=tcp:many2024.database.windows.net,1433;Initial Catalog=MesasDB;Persist Security Info=False;User ID=administrador;" +
            "Password=Alejandro.122403;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        //Mostrar las mesas del restaurante desde la base de datos
        public static void fntVerMesas()
        {
            Console.WriteLine("CATALOGO DE MESAS.");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "select * from tabla_mesas_disponibles";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        command.ExecuteNonQuery();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"|{Metodos.prdFormatoColumna("Numero de Mesa")}|{Metodos.prdFormatoColumna("Personas")}|{Metodos.prdFormatoColumna("Hora Disponible")}|{Metodos.prdFormatoColumna("Estado")}|{Metodos.prdFormatoColumna("Reserva Actual")}");
                                Console.ResetColor();
                                while (reader.Read())
                                {
                                    int numeroMesa = reader.GetInt32(reader.GetOrdinal("numero_mesa"));
                                    int cantidadPersonas = reader.GetInt32(reader.GetOrdinal("capacidad_personas"));
                                    TimeSpan horaDisponible = reader.GetTimeSpan(reader.GetOrdinal("Horas_disponibles"));
                                    bool estadoMesa = reader.GetBoolean(reader.GetOrdinal("estado_mesa"));
                                    string Disponible = estadoMesa ? "Disponible" : "Reservada";
                                    int reservaActual = reader.GetInt32(reader.GetOrdinal("id_reserva"));


                                    Metodos.prdLineasHorizontales();
                                    Metodos.Fila(numeroMesa, cantidadPersonas, horaDisponible, Disponible, reservaActual);
                                }
                            }
                            else
                            {
                                Metodos.fntErrores("No hay registros disponibles.");
                            }
                        }
                    }
                    catch (SqlException ex) { Metodos.fntErrores("Error en la base de datos: " + ex.ToString()); }
                    catch (Exception ex) { Metodos.fntErrores("Error inesperado: " + ex.ToString()); }
                }
                connection.Close();
            }
        }

        //Cancelar una reserva
        public static void fntCancelarReserva()
        {
            Console.WriteLine("\nCANCELAR RESERVA");
            Console.WriteLine("\nPara mas seguridad ingrese los siguientes datos:");
            string nombreDe;
            do
            {
                Console.Write("\nA nombre de quien esta la reserva: ");
                nombreDe = Console.ReadLine()!;

                if (string.IsNullOrEmpty(nombreDe))
                {
                    Metodos.fntErrores("El nombre no puede estar vacio");
                }

                if (Metodos.fntContieneNumeros(nombreDe))
                {
                    Metodos.fntErrores("El nombre no puede contener números.");
                }
            } while (string.IsNullOrEmpty(nombreDe) || Metodos.fntContieneNumeros(nombreDe));

            int idReserva;
            while (true)
            {
                Console.Write("\nId de reserva: ");

                if (int.TryParse(Console.ReadLine(), out idReserva))
                {
                    break;
                }
                Metodos.fntErrores("El id de reserva debe ser un número válido.");
            }


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    string query = "delete from tabla_reservas where id_reserva = @id_reserva and reserva_para = @reserva_para";
                    using (SqlCommand command = new SqlCommand(query, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@id_reserva", idReserva);
                        command.Parameters.AddWithValue("@reserva_para", nombreDe);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("\nOperacion realizada correctamente.");
                        }
                        else
                        {
                            Metodos.fntErrores("No se encontro ningun registro con esos datos.");
                        }
                    }

                    string query2 = "update tabla_mesas_disponibles set estado_mesa = 1 where id_reserva = @id_reserva";
                    using (SqlCommand command2 = new SqlCommand(query2, connection, transaction))
                    {

                        command2.Parameters.AddWithValue("@id_reserva", idReserva);
                        command2.ExecuteNonQuery();
                    }

                    string query3 = "update tabla_mesas_disponibles set id_reserva = default where id_reserva = @id_reserva";
                    using (SqlCommand command3 = new SqlCommand(query3, connection, transaction))
                    {

                        command3.Parameters.AddWithValue("@id_reserva", idReserva);
                        command3.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch (SqlException ex)
                {
                    transaction.Rollback();
                    Metodos.fntErrores("Error en la base de datos: " + ex.ToString());
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Metodos.fntErrores("Error inesperado: " + ex.ToString());
                }


                connection.Close();
            }
        }

        //Realizar una Reserva
        public static void fntRealizarReserva()
        {
            string reservaPara;

            Console.WriteLine("\nREALIZAR RESERVA");
            do
            {
                Console.Write("\nA nombre de quien será la reserva: ");
                reservaPara = Console.ReadLine()!;
                if (string.IsNullOrEmpty(reservaPara))
                {
                    Metodos.fntErrores("El nombre no puede estar vacio");
                }
                if (Metodos.fntContieneNumeros(reservaPara))
                {
                    Metodos.fntErrores("El nombre no puede contener números.");
                }
            } while (string.IsNullOrEmpty(reservaPara) || Metodos.fntContieneNumeros(reservaPara));

            Console.WriteLine("\nCual mesa le interesaria reservar?");
            fntVerMesas();
            int numeroMesa;
            while (true)
            {
                Console.Write("\nIngrese el numero de mesa: ");

                if (int.TryParse(Console.ReadLine(), out numeroMesa) && numeroMesa <= 10)
                {
                    break;
                }
                Metodos.fntErrores("Debe ingresar un número de mesa válido.");
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    //Nombre del procedimiento almacenado
                    string storedProcedureName = "insertarReserva";

                    using (SqlCommand command = new SqlCommand(storedProcedureName, connection, transaction))
                    {
                        //Indicar que el comando es un procedimiento almacenado
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@reserva_para", reservaPara);
                        command.Parameters.AddWithValue("@numero_mesa", numeroMesa);


                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("\nReserva realizada con éxito.");
                            string query4 = "select id_reserva from tabla_reservas where numero_mesa = @numero_mesa and reserva_para = @reserva_para";
                            using (SqlCommand command4 = new SqlCommand(query4, connection, transaction))
                            {
                                command4.Parameters.AddWithValue("@numero_mesa", numeroMesa);
                                command4.Parameters.AddWithValue("@reserva_para", reservaPara);
                                command4.ExecuteNonQuery();
                                using (SqlDataReader reader = command4.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        int idReserva = reader.GetInt32(reader.GetOrdinal("id_reserva"));
                                        Console.WriteLine($"\nEl Id de su reserva es {idReserva}");
                                    }
                                }
                            }
                        }
                        else
                        {
                            Metodos.fntErrores("La mesa no está disponible.");
                        }
                    }

                    string query2 = "update tabla_mesas_disponibles set estado_mesa = 0 where numero_mesa = @numero_mesa";
                    using (SqlCommand command2 = new SqlCommand(query2, connection, transaction))
                    {


                        command2.Parameters.AddWithValue("@numero_mesa", numeroMesa);
                        command2.ExecuteNonQuery();
                    }

                    string query3 = "UPDATE tabla_mesas_disponibles SET id_reserva = tabla_reservas.id_reserva FROM tabla_mesas_disponibles INNER JOIN tabla_reservas ON tabla_mesas_disponibles.id_mesa = tabla_reservas.id_mesa WHERE tabla_reservas.numero_mesa = @numero_mesa;";
                    using (SqlCommand command3 = new SqlCommand(query3, connection, transaction))
                    {
                        command3.Parameters.AddWithValue("@numero_mesa", numeroMesa);
                        command3.ExecuteNonQuery();
                    }

                    

                    transaction.Commit();
                }
                catch (SqlException ex)
                {
                    transaction.Rollback();
                    Metodos.fntErrores("Error en la base de datos: " + ex.ToString());
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Metodos.fntErrores("Error inesperado: " + ex.ToString());
                }
                connection.Close();
            }

        }

        //Hacer consulta de una reserva
        public static void fntConsultarReserva()
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();


                Console.WriteLine("\nCONSULTAR RESERVA");
                int reserva;
                while (true)
                {
                    Console.Write("\nIngrese el ID asignado a su reserva: ");
                    if (int.TryParse(Console.ReadLine(), out reserva))
                    {
                        break;
                    }
                    Metodos.fntErrores("Debe ingresar un número válido.");
                }

                string query = "select * from tabla_reservas where id_reserva = " + reserva;

                SqlCommand command = new SqlCommand(query, connection);

                command.ExecuteNonQuery();

                SqlDataReader registro = command.ExecuteReader();
                if (registro.Read())
                {
                    reserva = Convert.ToInt32(registro["id_reserva"]);
                    int Mesa = Convert.ToInt32(registro["numero_mesa"]);
                    string Nombre = registro["reserva_para"].ToString()!;
                    TimeSpan Hora = registro.GetTimeSpan(registro.GetOrdinal("hora_reserva"));
                    Console.WriteLine($"\n\tId de la reserva:[ {reserva} ]\t  Titular: [ {Nombre} ]\tHora: [ {Hora} ]\tNumero de mesa:[ {Mesa} ]");
                }
                else
                {
                    Metodos.fntErrores("No existe reserva con ese ID");
                }
            }
            catch (SqlException ex)
            {
                Metodos.fntErrores("Error en la base de datos: " + ex.ToString());
            }
            catch (Exception ex)
            {
                Metodos.fntErrores("Error inesperado: " + ex.ToString());
            }
        }
    }
}
