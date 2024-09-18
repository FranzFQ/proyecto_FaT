using System.Security.AccessControl;
using System.Text.Json;
using System.Timers;

class Program
{
    static void Main()
    {
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string folderName = "Archives";
        string binName = "Papelera";
        string FAT = "FAT";
        string folderPath = Path.Combine(desktopPath, folderName);
        string binPath = Path.Combine(desktopPath, binName);
        string FATPath = Path.Combine(desktopPath, FAT); 

        if (Directory.Exists(folderPath) == false)
        {
            Directory.CreateDirectory(folderPath);
        }
        if (Directory.Exists(binPath) == false)
        {
            Directory.CreateDirectory(binPath);
        }

        if (Directory.Exists(FATPath) == false)
        {
            Directory.CreateDirectory(FATPath);
        }

        string archivesFilePath = Path.Combine(folderPath, "Archives.json");
        List<Archive> archives = new List<Archive>();

        string binFilePath = Path.Combine(binPath, "bin.json");
        List<Archive> Bin = new List<Archive>();

        if (File.Exists(archivesFilePath) == true)
        {
            string jsonFromFile = File.ReadAllText(archivesFilePath);
            archives = JsonSerializer.Deserialize<List<Archive>>(jsonFromFile) ?? new List<Archive>();
        }

        if (File.Exists(binFilePath) == true)
        {
            string jsonFromFile = File.ReadAllText(binFilePath);
            Bin = JsonSerializer.Deserialize<List<Archive>>(jsonFromFile) ?? new List<Archive>();
        }

        while (true)
        {
            Console.WriteLine("\n Seleccione una opción:");
            Console.WriteLine("1. Crear un archivo");
            Console.WriteLine("2. Listar todos los archivos");
            Console.WriteLine("3. Abrir un archivo en especifico");
            Console.WriteLine("4. Modificar un archivo");
            Console.WriteLine("5. Eliminar un archivo");
            Console.WriteLine("6. Recuperar un archivo");
            Console.WriteLine("7. Salir");

            Console.WriteLine("Ingrese el numero de la opcion: ");
            string option = Console.ReadLine()!;

            if (option == "1"){
                Agregar(archives, FATPath, archivesFilePath);

            }else if(option == "2"){
                Console.WriteLine("Los archivos agregados actualmente son: ");
                mostrar(archives);

            }else if(option == "3"){
                var archive = buscar(archives);
                Console.WriteLine("Los datos del archivo son: ");
                Console.WriteLine("Nombre: " + archive.Name);
                Console.WriteLine("Texto: " + archive.Text);
                Console.WriteLine("Cantidad de archivos: " + archive.Parts);
                Console.WriteLine("Cantidad de caracteres: " + archive.Caracters);
                Console.WriteLine("Fecha de creacion: " + archive.Time);
                Console.WriteLine("Modificacion: " + archive.Modification);

            }else if(option == "4"){
                modificar(archives, FATPath);

            }else if(option == "5"){
                eliminacion(archives, FATPath, Bin, archivesFilePath, binFilePath);

            }else if(option == "6"){
                recuparacion(archives, FATPath, Bin, archivesFilePath, binFilePath);

            }else if(option == "7"){
                break;
            }
            else{
                Console.WriteLine("El Argumento no es valido");
            }
        }
    }

    static void Agregar(List<Archive> archives, string desktopPath, string json)
    {   
        int parts = 1;
        string words = ""; 

        Console.Write("Ingrese el nombre del archivo: ");
        string name = Console.ReadLine()!;

        Console.Write("Ingrese el texto: ");
        string text = Console.ReadLine()!;

        for (var i = 0; i < text.Length; i++){
            words += text[i];
            if ((i % 20) == 0){
                File.WriteAllText(desktopPath + "/" + name + " parte" + " " + parts.ToString(), words + Environment.NewLine);
                words = "";
                parts += 1;
            }else if (i + 1 == text.Length){
                File.WriteAllText(desktopPath + "/" + name + " parte" + " " + parts.ToString(), words + Environment.NewLine);
            }
        }
        Console.WriteLine("El archivo se ha creado de manera exitosa");

        Archive new_archive = new Archive(name, text, parts, text.Length, DateTime.Now.ToString());
        archives.Add(new_archive);

        string jsonstring = JsonSerializer.Serialize(archives);
        File.WriteAllText(json, jsonstring);
    }

    static int mostrar(List<Archive> archives){
        int count = 1;
        foreach(var archive in archives){
            Console.WriteLine("- " + count + " Nombre: " + archive.Name + " Cantidad de caracteres: " + archive.Caracters + " Fecha de creacion: " + archive.Time + " Modificacion: " + archive.Modification);
            count += 1;
        }
        return count;
    }

    static Archive buscar(List<Archive> archives){
        var vault = archives[0];
        int legth = mostrar(archives);
        Console.WriteLine("Ingrese el numero del texto que estes buscando:");
        int number = int.Parse(Console.ReadLine()!);
        if (number > legth - 1){
            throw new Exception("El archivo no existe");
        }else{
        foreach (var archive in archives){
            if (archive.Name == archives[number - 1].Name){
                vault = archive;
                break;
                }
            }
        }
        return vault;
    }

    static void modificar(List<Archive> archives, string desktopPath){
        Console.WriteLine("Que texto desea modificar");
        var archive = buscar(archives);
        string new_text = "";
        Console.WriteLine("El texto actual del archivo es: \n" + archive.Text);
        Console.WriteLine("Ingrese el texto: ");
        while (true){
            new_text += Console.ReadLine()!;
            Console.WriteLine("Presione la tecla -Escape- para confirmar cambios");
            var new_key = Console.ReadKey();
            if (new_key.Key == ConsoleKey.Escape){
                break;
            }
        }
        archive.Modification = DateTime.Now.ToString();
        archive.Text += " " + new_text.ToString();
        int continue_text = File.ReadAllLines(desktopPath + "/" + archive.Name + " parte " + archive.Parts.ToString()).Length;
        string text = "";

        for (int i = 0; i < new_text.Length; i++){
            text +=  new_text[i]; 
            if ((continue_text + i) == 20){
                File.AppendAllText(desktopPath + "/" + archive.Name + " parte " + archive.Parts.ToString(), text);
                text = "";
                archive.Parts += 1;
            }else if((i % 20) == 0){
                File.AppendAllText(desktopPath + "/" + archive.Name + " parte " + archive.Parts.ToString(), text + Environment.NewLine);
                text = "";
                archive.Parts += 1;
            }else if (i + 1 == new_text.Length){
                File.AppendAllText(desktopPath + "/" + archive.Name + " parte " + archive.Parts.ToString(), text + Environment.NewLine);
            }
        }
        
        Console.WriteLine("El texto se ha agregado correctamente");

    }

    static void eliminacion(List<Archive> archives, string desktopPath, List<Archive> Bin, string json_archive, string json_bin){
        Console.WriteLine("Que archivo desea eliminar");
        var archive = buscar(archives);
        archives.Remove(archive);
        string jsonstring_archive = JsonSerializer.Serialize(archives);
        File.WriteAllText(json_archive, jsonstring_archive);
        Bin.Add(archive);
        string jsonstring_bin = JsonSerializer.Serialize(Bin);
        File.WriteAllText(json_bin, jsonstring_bin);
        for (int i = 0; i < archive.Parts + 1; i++){
            File.Delete(desktopPath + "/" + archive.Name + " parte " + i.ToString());
        }
        Console.WriteLine("Se elimino correctamente");
    }

    static void recuparacion(List<Archive> archives, string desktopPath, List<Archive> Bin, string json_archive, string json_bin){
        var count = 1;
        string text = "";
        int parts = 1;
        Console.WriteLine("Que archivo desea recuparar");
        foreach (var archive in Bin){
            Console.WriteLine("- " + count + " " + archive.Name);
            count += 1;
        }
        Console.WriteLine("Ingrese el numero del archivo que quiere restablecer");
        int position = int.Parse(Console.ReadLine()!);
        if (position > count){
            throw new Exception("El archivo no existe");
        }else{
            foreach(var archive in Bin){
                if (archive.Name == Bin[position - 1].Name){
                    archives.Add(archive);
                    string jsonstring_archive = JsonSerializer.Serialize(archives);
                    File.WriteAllText(json_archive, jsonstring_archive);
                    Bin.Remove(archive);
                    string jsonstring_bin = JsonSerializer.Serialize(Bin);
                    File.WriteAllText(json_bin, jsonstring_bin);
                    for (int i = 0; i < archive.Text.Length; i++){
                        text += archive.Text[i];
                    if ((i % 20) == 0){
                        File.WriteAllText(desktopPath + "/" + archive.Name + " parte " + parts.ToString(), text + Environment.NewLine);
                        text = "";
                        parts += 1;
                    }else if (i + 1 == text.Length){
                        File.WriteAllText(desktopPath + "/" + archive.Name + " parte " + parts.ToString(), text + Environment.NewLine);
                        }

                    }
                    break;
                }
            }
            Console.WriteLine("Se restablecio el archivo");
        }

    }
}

class Archive
{
    public string Name { get; set; }
    public string Text { get; set; }
    public int Parts {get; set;}
    public int Caracters {get; set;}
    public string Time {get; set;}
    public string Modification {get; set;}

    public Archive(string name, string text, int parts, int caracters, string time, string modification = "")
    {
        Name = name;
        Text = text;
        Parts = parts;
        Caracters = caracters;
        Time = time;
        Modification = modification;

    }
}
