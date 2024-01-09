﻿using System.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;

//Conexión con base de datos SQL
string connectionString = "Server=tcp:many2024.database.windows.net,1433;Initial Catalog=MesasDB;Persist Security Info=False;User ID=administrador;" +
    "Password=Alejandro.122403;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

//Declaración de Variables
string userChoise;
int vColumnaAncho = 20;


Console.WriteLine("BIENVENIDO!");

//Mantener programa en ejecución
do
{
    fntMostrarOpciones();
    userChoise = Console.ReadLine();

    switch (userChoise)
    {
        case "1":
            fntVerMesas();
            break;
        case "2":
            //fntRealizarReserva();
            break;
        case "3":
            //fntCancelarReserva();
            break;
        case "4":
            fntConsultarReserva();
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
    Console.WriteLine("[4] Consultar reserva.");
    Console.WriteLine("[5] Salir del programa.");
    Console.Write("Elija una opción: ");
}

//Mostrar las mesas del restaurante desde la base de datos
void fntVerMesas()
{
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