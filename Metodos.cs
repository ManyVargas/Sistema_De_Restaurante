
namespace Sistema_de_Restaurante.MetodosGenericos
{
    public static class Metodos
    {
        private const int vColumnaAncho = 20;
        //Mostrar opciones del menu
        public static void fntMostrarOpciones()
        {
            Console.WriteLine("\n[1] Ver disponibilidad de mesas.");
            Console.WriteLine("\n[2] Realizar una reserva.");
            Console.WriteLine("\n[3] Cancelar una reserva.");
            Console.WriteLine("\n[4] Ver información de reserva.");
            Console.WriteLine("\n[5] Salir del programa.");
            Console.Write("\nElija una opción: ");
        }

        //Cerrar programa
        public static void fntCerrarPrograma()
        {
            Console.WriteLine("Terminado, presione cualquier tecla para cerrar.");
        }

        //Formato para el ancho de las columnas
        public static string prdFormatoColumna(string texto)
        {
            return texto.PadRight(vColumnaAncho).Substring(0, vColumnaAncho);
        }

        //Lineas horizontales para dividir las filas
        public static void prdLineasHorizontales()
        {
            Console.WriteLine(new string('-', (vColumnaAncho * 5 + 2)));
        }

        //Imprimir cada fila de la tabla
        public static void Fila(int columna1, int columna2, TimeSpan columna3, string columna4, int columna5)
        {
            Console.WriteLine($"|{prdFormatoColumna(columna1.ToString())}|{prdFormatoColumna(columna2.ToString())}|{prdFormatoColumna(columna3.ToString())}|{prdFormatoColumna(columna4)}|{prdFormatoColumna(columna5.ToString())}");
        }

        public static void fntVolverMenuTexto()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nPresione cualquier tecla para volver al menú principal.");
            Console.ResetColor();
            Console.ReadKey();
            Console.Clear();
        }

        public static void fntErrores(string texto)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n" + texto);
            Console.ResetColor();
        }

        public static bool fntContieneNumeros(string input)
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
    }
}
