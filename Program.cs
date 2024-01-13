//Referencias de los namespaces para los metodos
using Sistema_de_Restaurante.MetodosGenericos;
using Sistema_de_Restaurante.MetodosBaseDatos;

//Declaración de Variables generales
string userChoise;



//Mantener programa en ejecución
do
{
    Console.Clear();
    Console.WriteLine("BIENVENIDO!");

    Metodos.fntMostrarOpciones();
    userChoise = Console.ReadLine()!;

    switch (userChoise)
    {
        case "1":
            Console.Clear();
            MetodosBaseDeDatos.fntVerMesas();
            Metodos.fntVolverMenuTexto();
            break;
        case "2":
            Console.Clear();
            MetodosBaseDeDatos.fntRealizarReserva();
            Metodos.fntVolverMenuTexto();
            break;
        case "3":
            Console.Clear();
            MetodosBaseDeDatos.fntCancelarReserva();
            Metodos.fntVolverMenuTexto();
            break;
        case "4":
            Console.Clear();
            MetodosBaseDeDatos.fntConsultarReserva();
            Metodos.fntVolverMenuTexto();
            break;
        case "5":
            Metodos.fntCerrarPrograma();
            break;
        default:
            Console.Clear() ;
            Metodos.fntErrores("\nSelección inválida!");
            Metodos.fntVolverMenuTexto();
            break;
    }


} while (userChoise != "5");


Console.ReadKey();

