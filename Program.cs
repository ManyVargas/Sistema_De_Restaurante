//Debe tener las referencias de System.Data.SqlClient instalada
using System.Data.SqlClient;
using System.Runtime.Intrinsics.X86;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

//Conexión con base de datos SQL
string connectionString = "Server=tcp:many2024.database.windows.net,1433;Initial Catalog=MesasDB;Persist Security Info=False;User ID=administrador;" +
    "Password=Alejandro.122403;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

//Declaración de Variables generales
string userChoise;
int vColumnaAncho = 20;



//Mantener programa en ejecución
do
{
    Console.Clear();
    Console.WriteLine("BIENVENIDO!");

    fntMostrarOpciones();
    userChoise = Console.ReadLine()!;

    switch (userChoise)
    {
        case "1":
            Console.Clear();
            fntVerMesas();
            fntVolverMenuTexto();
            break;
        case "2":
            Console.Clear();
            fntRealizarReserva();
            fntVolverMenuTexto();
            break;
        case "3":
            Console.Clear();
            fntCancelarReserva();
            fntVolverMenuTexto();
            break;
        case "4":
            Console.Clear();
            fntConsultarReserva();
            fntVolverMenuTexto();
            break;
        case "5":
            fntCerrarPrograma();
            break;
        default:
            Console.WriteLine("\nSelección inválida!");
            fntVolverMenuTexto();
            break;
    }


} while (userChoise != "5");


Console.ReadKey();

/*---------------METODOS-------------*/

//Mostrar opciones del menu
void fntMostrarOpciones()
{


    Console.WriteLine("\n[1] Ver disponibilidad de mesas.");
    Console.WriteLine("\n[2] Realizar una reserva.");
    Console.WriteLine("\n[3] Cancelar una reserva.");
    Console.WriteLine("\n[4] Ver información de reserva.");
    Console.WriteLine("\n[5] Salir del programa.");
    Console.Write("\nElija una opción: ");
}

//Mostrar las mesas del restaurante desde la base de datos
void fntVerMesas()
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
                        Console.WriteLine($"|{prdFormatoColumna("Numero de Mesa", vColumnaAncho)}|{prdFormatoColumna("Personas", vColumnaAncho)}|{prdFormatoColumna("Hora Disponible", vColumnaAncho)}|{prdFormatoColumna("Estado", vColumnaAncho)}|{prdFormatoColumna("Reserva Actual", vColumnaAncho)}");
                        Console.ResetColor();
                        while (reader.Read())
                        {
                            int numeroMesa = reader.GetInt32(reader.GetOrdinal("numero_mesa"));
                            int cantidadPersonas = reader.GetInt32(reader.GetOrdinal("capacidad_personas"));
                            TimeSpan horaDisponible = reader.GetTimeSpan(reader.GetOrdinal("Horas_disponibles"));
                            bool estadoMesa = reader.GetBoolean(reader.GetOrdinal("estado_mesa"));
                            string Disponible = estadoMesa ? "Disponible" : "Reservada";
                            int reservaActual = reader.GetInt32(reader.GetOrdinal("id_reserva"));


                            prdLineasHorizontales(vColumnaAncho * 5 + 2);
                            Fila(numeroMesa, cantidadPersonas, horaDisponible, Disponible, reservaActual);
                        }
                    }
                    else
                    {
                        fntErrores("No hay registros disponibles.");
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
    Console.WriteLine("\nCANCELAR RESERVA");
    Console.WriteLine("\nPara mas seguridad ingrese los siguientes datos:");
    string nombreDe;
    do
    {
        Console.Write("\nA nombre de quien esta la reserva: ");
        nombreDe = Console.ReadLine()!;

        if (string.IsNullOrEmpty(nombreDe))
        {
            fntErrores("El nombre no puede estar vacio");
        }

        if (fntContieneNumeros(nombreDe))
        {
            fntErrores("El nombre no puede contener números.");
        }
    } while (string.IsNullOrEmpty(nombreDe) || fntContieneNumeros(nombreDe));

    int idReserva;
    while (true)
    {
        Console.Write("\nId de reserva: ");

        if (int.TryParse(Console.ReadLine(), out idReserva))
        {
            break;
        }
        fntErrores("El id de reserva debe ser un número válido.");
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
                    fntErrores("No se encontro ningun registro con esos datos.");
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
            fntErrores("Error en la base de datos: " + ex.ToString());
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            fntErrores("Error inesperado: " + ex.ToString());
        }


        connection.Close();
    }
}

//Realizar una Reserva
void fntRealizarReserva()
{
    string reservaPara;

    Console.WriteLine("\nREALIZAR RESERVA");
    do
    {
        Console.Write("\nA nombre de quien será la reserva: ");
        reservaPara = Console.ReadLine()!;
        if (string.IsNullOrEmpty(reservaPara))
        {
            fntErrores("El nombre no puede estar vacio");
        }
        if (fntContieneNumeros(reservaPara))
        {
            fntErrores("El nombre no puede contener números.");
        }
    } while (string.IsNullOrEmpty(reservaPara) || fntContieneNumeros(reservaPara));

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
        fntErrores("Debe ingresar un número de mesa válido.");
    }

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
                    Console.WriteLine("\nReserva realizada con éxito.");
                }
                else
                {
                    fntErrores("La mesa no está disponible.");
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

            transaction.Commit();
        }
        catch (SqlException ex)
        {
            transaction.Rollback();
            fntErrores("Error en la base de datos: " + ex.ToString());
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            fntErrores("Error: " + ex.ToString());
        }
        connection.Close();
    }

}

//Hacer consulta de una reserva
void fntConsultarReserva()
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
            fntErrores("Debe ingresar un número válido.");
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
            fntErrores("No existe reserva con ese ID");
        }
    }
    catch (SqlException ex)
    {
        fntErrores("\nError en la base de datos: " + ex.ToString());
    }
    catch (Exception ex)
    {
        fntErrores("\nError: " + ex.ToString());
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
void Fila(int columna1, int columna2, TimeSpan columna3, string columna4, int columna5)
{
    Console.WriteLine($"|{prdFormatoColumna(columna1.ToString(), vColumnaAncho)}|{prdFormatoColumna(columna2.ToString(), vColumnaAncho)}|{prdFormatoColumna(columna3.ToString(), vColumnaAncho)}|{prdFormatoColumna(columna4, vColumnaAncho)}|{prdFormatoColumna(columna5.ToString(), vColumnaAncho)}");
}

void fntVolverMenuTexto()
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("\nPresione cualquier tecla para volver al menú principal.");
    Console.ResetColor();
    Console.ReadKey();
    Console.Clear();
}

void fntErrores(string texto)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("\n" + texto);
    Console.ResetColor();
}

static bool fntContieneNumeros(string input)
{
    foreach (char caracter in input)
    {
        if (char.IsDigit(caracter))
        {
            return true;
        }
    }
    return false;
}