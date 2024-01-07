//Declaración de Variables
string userChoise;


Console.WriteLine("BIENVENIDO!");

//Mantener programa en ejecución
do
{
    fntMostrarOpciones();
    userChoise = Console.ReadLine();

    switch (userChoise)
    {
        case "1":
            //fntVerMesas();
            break;
        case "2":
            //fntRealizarReserva();
            break;
        case "3":
            //fntCancelarReserva();
            break;
        case "4":
            //fntConsultarReserva();
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
void fntMostrarOpciones()
{
    Console.WriteLine("\n");
    Console.WriteLine("Que desea realizar ?");
    Console.WriteLine("[1] Ver disponibilidad de mesas.");
    Console.WriteLine("[2] Realizar una reserva.");
    Console.WriteLine("[3] Cancelar una reserva.");
    Console.WriteLine("[4] Consultar reserva.");
    Console.WriteLine("[5] Salir del programa.");
    Console.Write("Elija la opción: ");
}

void fntCerrarPrograma()
{
    Console.WriteLine("Terminado, presione cualquier tecla para cerrar.");
}