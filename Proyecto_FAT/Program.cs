using System.Text.Json;

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
                mostrar(archives);

            }else if(option == "3"){

            }else if(option == "4"){

            }else if(option == "5"){

            }else if(option == "6"){

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

        for (var i = 1; i < text.Length; i++){
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

        Archive new_archive = new Archive(name, text, parts);
        archives.Add(new_archive);

        string jsonstring = JsonSerializer.Serialize(archives);
        File.WriteAllText(json, jsonstring);
    }

    static void mostrar(List<Archive> archives){
        int count = 1;
        Console.WriteLine("Los archivos agregados actualmente son");
        foreach(var archive in archives){
            Console.WriteLine("-", count + "Nombre: ", archive.Name);
        }
    }
}

class Archive
{
    public string Name { get; set; }
    public string Text { get; set; }
    public int Parts {get; set;}

    public Archive(string name, string text, int parts)
    {
        Name = name;
        Text = text;
        Parts = parts;

    }
}
