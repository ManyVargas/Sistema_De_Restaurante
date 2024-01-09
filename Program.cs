//Debe tener las referencias de System.Data.SqlClient instalada
using System.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;

//Conexión con base de datos SQL
string connectionString = "Server=tcp:many2024.database.windows.net,1433;Initial Catalog=MesasDB;Persist Security Info=False;User ID=administrador;" +
    "Password=Alejandro.122403;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

//Declaración de Variables
string userChoise;
int vColumnaAncho = 20;



//Mantener programa en ejecución
do
{
    Console.Clear();
    Console.WriteLine("BIENVENIDO!");

    fntMostrarOpciones();
    userChoise = Console.ReadLine();

    switch (userChoise)
    {
        case "1":
            fntVerMesas();
            fntVolverMenuTexto();
            break;
        case "2":
            fntRealizarReserva();
            fntVolverMenuTexto();
            break;
        case "3":
            fntCancelarReserva();
            fntVolverMenuTexto();
            break;
        case "4":
            fntVerInformacionReserva();
            fntVolverMenuTexto();
            break;
        case "5":
            fntCerrarPrograma();
            break;
        default:
            Console.WriteLine("Selección inválida.");
            break;
    }


} while (userChoise != "5");


Console.ReadKey();



/*---------------METODOS-------------*/

//Mostrar opciones del menu
void fntMostrarOpciones()
{
    Console.Write("\n");
    Console.WriteLine("[1] Ver disponibilidad de mesas.");
    Console.WriteLine("[2] Realizar una reserva.");
    Console.WriteLine("[3] Cancelar una reserva.");
    Console.WriteLine("[4] Ver información de reserva.");
    Console.WriteLine("[5] Salir del programa.");
    Console.Write("Elija una opción: ");
}

//Mostrar las mesas del restaurante desde la base de datos
void fntVerMesas()
{
    Console.Clear();
    Console.WriteLine("Catalogo de mesas.");
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
                        Console.WriteLine($"|{prdFormatoColumna("Numero de Mesa", vColumnaAncho)}|{prdFormatoColumna("Personas", vColumnaAncho)}|{prdFormatoColumna("Hora Disponible", vColumnaAncho)}|{prdFormatoColumna("Estado", vColumnaAncho)}");
                        Console.ResetColor();
                        while (reader.Read())
                        {
                            int numeroMesa = reader.GetInt32(reader.GetOrdinal("numero_mesa"));
                            int cantidadPersonas = reader.GetInt32(reader.GetOrdinal("capacidad_personas"));
                            TimeSpan horaDisponible = reader.GetTimeSpan(reader.GetOrdinal("Horas_disponibles"));
                            bool estadoMesa = reader.GetBoolean(reader.GetOrdinal("estado_mesa"));
                            string Disponible = estadoMesa ? "Disponible" : "Reservada";



                            prdLineasHorizontales(20 * 4 + 2);
                            Fila(numeroMesa, cantidadPersonas, horaDisponible, Disponible);
                        }
                    }
                    else
                    {
                        Console.WriteLine("No hay registros disponibles.");
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }
        connection.Close();
    }
}

//Cancelar una reserva
void fntCancelarReserva()
{
    Console.Clear();
    Console.WriteLine("Cancelar Reserva");
    Console.WriteLine("Para mas seguridad ingrese los siguientes datos:\n");

    Console.Write("A nombre de quien esta la reserva: ");
    string nombreDe = Console.ReadLine();

    Console.Write("Id de reserva: ");
    int idReserva = Convert.ToInt32(Console.ReadLine());

    Console.Write("Ingrese el numero de mesa de su reserva: ");
    int numeroMesa = Convert.ToInt32(Console.ReadLine());

    Console.Write("Ingrese a que hora es su reserva en formato (HH:mm): ");
    string horaReservaString = Console.ReadLine();
    TimeSpan horaReserva = TimeSpan.Parse(horaReservaString);


    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction();

        try
        {
            string query = "delete from tabla_reservas where hora_reserva = @hora_reserva and id_reserva = @id_reserva and reserva_para = @reserva_para";
            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@hora_reserva", horaReserva);
                command.Parameters.AddWithValue("@id_reserva", idReserva);
                command.Parameters.AddWithValue("@reserva_para", nombreDe);
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("Operacion realizada correctamente.");
                }
                else
                {
                    Console.WriteLine("No se encontro ningun registro con esos datos.");
                }
            }

            string query2 = "update tabla_mesas_disponibles set estado_mesa = 1 where horas_disponibles = @hora_reserva and numero_mesa = @numero_mesa and reserva_de = @reserva_para";
            using (SqlCommand command2 = new SqlCommand(query2, connection, transaction))
            {


                command2.Parameters.AddWithValue("@numero_mesa", numeroMesa);
                command2.Parameters.AddWithValue("@hora_reserva", horaReserva);
                command2.Parameters.AddWithValue("@reserva_para", nombreDe);
                int rowsAffected = command2.ExecuteNonQuery();
            }

            string query3 = "update tabla_mesas_disponibles set reserva_de = default where numero_mesa = @numero_mesa";
            using (SqlCommand command3 = new SqlCommand(query3, connection, transaction))
            {
                command3.Parameters.AddWithValue("@reserva_para", nombreDe);
                command3.Parameters.AddWithValue("@numero_mesa", numeroMesa);
                command3.ExecuteNonQuery();
            }

            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();

            Console.WriteLine("No se pudo realizar la operación.");
            Console.WriteLine("Error: " + ex.ToString());
        }


        connection.Close();
    }
}

//Realizar una Reserva
void fntRealizarReserva()
{
    Console.Clear();
    Console.WriteLine("Realizar reserva");

    Console.Write("A nombre de quien será la reserva: ");
    string reservaPara = Console.ReadLine();

    Console.WriteLine("\nCual mesa le interesaria reservar?");
    fntVerMesas();

    Console.Write("\nIngrese el numero de mesa: ");
    int numeroMesa = Convert.ToInt32(Console.ReadLine());

    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction();
        try
        {
            string query = "if exists (select numero_mesa from tabla_mesas_disponibles where numero_mesa = @numero_mesa and estado_mesa = 1) begin insert into tabla_reservas " +
                "(id_mesa, numero_mesa, reserva_para,hora_reserva) values((select id_mesa from tabla_mesas_disponibles where numero_mesa = @numero_mesa)" +
                ",@numero_mesa,@reserva_para,(select horas_disponibles from tabla_mesas_disponibles where numero_mesa = @numero_mesa)) end ";

            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@reserva_para", reservaPara);
                command.Parameters.AddWithValue("@numero_mesa", numeroMesa);
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine("Reserva realizada con éxito.");
                }
                else
                {
                    Console.WriteLine("La mesa no está disponible.");
                }
            }

            string query2 = "update tabla_mesas_disponibles set estado_mesa = 0 where numero_mesa = @numero_mesa";
            using (SqlCommand command2 = new SqlCommand(query2, connection, transaction))
            {


                command2.Parameters.AddWithValue("@numero_mesa", numeroMesa);
                int rowsAffected = command2.ExecuteNonQuery();
            }

            string query3 = "update tabla_mesas_disponibles set reserva_de = @reserva_para where numero_mesa = @numero_mesa";
            using (SqlCommand command3 = new SqlCommand(query3, connection, transaction))
            {
                command3.Parameters.AddWithValue("@reserva_para", reservaPara);
                command3.Parameters.AddWithValue("@numero_mesa", numeroMesa);
                command3.ExecuteNonQuery();
            }

            string query4 = "select id_reserva from tabla_reservas where numero_mesa = @numero_mesa and reserva_para = @reserva_para";
            using (SqlCommand command4 = new SqlCommand(query4, connection, transaction))
            {
                command4.Parameters.AddWithValue("@numero_mesa", numeroMesa);
                command4.Parameters.AddWithValue("@reserva_para",reservaPara);
                command4.ExecuteNonQuery();
                using (SqlDataReader reader = command4.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int idReserva = reader.GetInt32(reader.GetOrdinal("id_reserva"));
                        Console.WriteLine($"El Id de su reserva es {idReserva}");
                    }
                }
            }

            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            Console.WriteLine("No se pudo realizar la operacion.");
            Console.WriteLine("Error: " + ex.ToString());
        }
        connection.Close();
    }

}


//Cerrar programa
void fntCerrarPrograma()
{
    Console.WriteLine("Terminado, presione cualquier tecla para cerrar.");
}

//Formato para el ancho de las columnas
string prdFormatoColumna(string texto, int ancho)
{
    return texto.PadRight(ancho).Substring(0, ancho);
}

//Lineas horizontales para dividir las filas
void prdLineasHorizontales(int longitud)
{
    Console.WriteLine(new string('-', (longitud + 2)));
}

//Imprimir cada fila de la tabla
void Fila(int columna1, int columna2, TimeSpan columna3, string columna4)
{
    Console.WriteLine($"|{prdFormatoColumna(columna1.ToString(), vColumnaAncho)}|{prdFormatoColumna(columna2.ToString(), vColumnaAncho)}|{prdFormatoColumna(columna3.ToString(), vColumnaAncho)}|{prdFormatoColumna(columna4, vColumnaAncho)}");
}

void fntVolverMenuTexto()
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("\nPresione cualquier tecla para volver al menú principal.");
    Console.ResetColor();
    Console.ReadKey();
}