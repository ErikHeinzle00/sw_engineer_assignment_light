using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using JsonConverter = Newtonsoft.Json.JsonConverter;

namespace equipment_tracker_lib;

public class Equipment
{

    // The properties of the equipment object
    public int Id { get; set; }

    public Status Status { get; set; }

    public string Name { get; set; }

    private static readonly string ProjectDirectory =
        Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;


    // The path gets created dynamically based on the project directory on the user's machine ( Tested on Windows and macOS )
    [Newtonsoft.Json.JsonIgnore]
    public static readonly string FilePath = Path.Combine(ProjectDirectory, "data", "equipment.json");

    // This method lets the user add a new equipment object to the json file
    public void AddEquipment()
    {
        // The console asks the user for the name and tells the possible choices for the status of the equipment
        Console.Write("Enter the name of the equipment: ");
        string name = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exception("The name of the equipment cannot be empty.");
        }

        Console.WriteLine("Which status does the equipment have?");
        Console.WriteLine("O: Operational");
        Console.WriteLine("I: Inoperable");
        Console.WriteLine("N: Needs Maintenance");
        Console.WriteLine("U: Unknown");
        Console.WriteLine("M: Missing");
        Console.WriteLine("D: Damaged");


        // The user's input gets checked if it matches one of the status options, if not, an exception gets thrown
        var key = Console.ReadKey().Key;
        Status status = key switch
        {
            ConsoleKey.O => Status.Operational,
            ConsoleKey.I => Status.Inoperable,
            ConsoleKey.N => Status.NeedsMaintenance,
            ConsoleKey.U => Status.Unknown,
            ConsoleKey.M => Status.Missing,
            ConsoleKey.D => Status.Damaged,
            _ => throw new Exception("Invalid status selection.")
        };

        // A new list of equipment objects gets initialized
        var equipmentList = new List<Equipment>();

        // The path gets checked if it exists and the existing equipment objects get read from the file, if the path does not exist, an exception gets thrown
        if (File.Exists(FilePath))
        {
            var existingJson = File.ReadAllText(FilePath);
            equipmentList = JsonConvert.DeserializeObject<List<Equipment>>(existingJson) ?? new List<Equipment>();
        }
        else
        {
            // The first Console.WriteLine() is just to add a line break
            Console.WriteLine();
            throw new Exception("The Filepath does not exist or is not accessible.");
        }

        // The ID gets generated by incrementing the highest/latest ID in the list
        int nextId = (equipmentList.Count > 0) ? equipmentList.Max(e => e.Id) + 1 : 1;

        // The new equipment object gets created and added to the list
        var newEquipment = new Equipment { Id = nextId, Name = name, Status = status };
        equipmentList.Add(newEquipment);

        // The updated list gets serialized and saved to the file
        var settings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new StringEnumConverter() },
            Formatting = Formatting.Indented
        };
        var updatedJson = JsonConvert.SerializeObject(equipmentList, settings);
        File.WriteAllText(FilePath, updatedJson);
    }

    // This method lets the user change the status of an equipment object
    public void ChangeStatus()
    {   // A new list of equipment objects gets initialized
        var equipmentList = new List<Equipment>();
        
        // The path gets checked if it exists and the existing equipment objects get read from the file, if the path does not exist, an exception gets thrown
        if (File.Exists(FilePath))
        {
            var existingJson = File.ReadAllText(FilePath);
            equipmentList = JsonConvert.DeserializeObject<List<Equipment>>(existingJson) ?? new List<Equipment>();
        }
        else
        {
            // The first Console.WriteLine() is just to add a line break
            Console.WriteLine();
            throw new Exception("The Filepath does not exist or is not accessible.");
        }
        
        
        // The user gets asked for the ID of the equipment object to change the status
        Console.Write("Enter the ID of the equipment: ");
        int id = int.Parse(Console.ReadLine());
        
        // The equipment object with the specified ID gets retrieved and if it is found, the status gets changed, otherwise an exception gets thrown
        var equipmentToUpdate = equipmentList.FirstOrDefault(e => e.Id == id);
        if (equipmentToUpdate != null)
        {
            // Display the current status in DarkCyan color
            Console.Write($"The equipment with ID {id} currently has the following status: ");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine($"{equipmentToUpdate.Status}");
            Console.ResetColor();

            Console.WriteLine("Which status should the equipment have?");
            Console.WriteLine("O: Operational");
            Console.WriteLine("I: Inoperable");
            Console.WriteLine("N: Needs Maintenance");
            Console.WriteLine("U: Unknown");
            Console.WriteLine("M: Missing");
            Console.WriteLine("D: Damaged");

            var key = Console.ReadKey().Key;
            Status status = key switch
            {
                ConsoleKey.O => Status.Operational,
                ConsoleKey.I => Status.Inoperable,
                ConsoleKey.N => Status.NeedsMaintenance,
                ConsoleKey.U => Status.Unknown,
                ConsoleKey.M => Status.Missing,
                ConsoleKey.D => Status.Damaged,
                _ => throw new Exception("Invalid status selection.")
            };

            // The status of the equipment object gets updated
            equipmentToUpdate.Status = status;

            // Display the updated status in DarkCyan color. The first Console.WriteLine to make a break line
            Console.WriteLine();
            Console.Write($"The equipment with ID {id} now has the following status: ");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine($"{equipmentToUpdate.Status}");
            Console.ResetColor();
        }
        else
        {
            throw new Exception("Equipment with the specified ID not found.");
        }

        // The updated list gets serialized and saved to the file
        var settings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new StringEnumConverter() },
            Formatting = Formatting.Indented
        };

        var updatedJson = JsonConvert.SerializeObject(equipmentList, settings);
        File.WriteAllText(FilePath, updatedJson);
    }

    public void RetrieveStatus(int id)
    {
        // A new list of equipment objects gets initialized
        var equipmentList = new List<Equipment>();

        // The path gets checked if it exists, then the existing objects get read and processed. If the path does not exist or is not accessible, an exception gets thrown
        if (File.Exists(FilePath))
        {

            // The existing equipment objects get read from the file
            var existingJson = File.ReadAllText(FilePath);

            // The existing equipment objects get deserialized from the JSON file
            equipmentList = JsonConvert.DeserializeObject<List<Equipment>>(existingJson) ?? new List<Equipment>();

            // The equipment object with the specified ID gets retrieved
            var equipmentWithId = equipmentList.FirstOrDefault(e => e.Id == id);

            // The ID gets checked if it exists in the list and if it is found, the status gets displayed, otherwise an exception gets thrown
            if (equipmentWithId != null)
            {
                Console.WriteLine($"The equipment with ID {id} currently has the following status:");
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine($"{equipmentWithId.Status}");
                Console.ResetColor();
            }
            else
            {
                throw new Exception("No equipment found with the specified ID.");
            }
        }
        else
        {
            throw new Exception("No equipment list found.");
        }
    }
    
    // This method lets the user search for an equipment object by ID
    public void SearchById()
    {
        // A new list of equipment objects gets initialized
        var equipmentList = new List<Equipment>();
        if (File.Exists(FilePath))
        {
            var existingJson = File.ReadAllText(FilePath);
            equipmentList = JsonConvert.DeserializeObject<List<Equipment>>(existingJson) ?? new List<Equipment>();
        }
        else
        {
            throw new Exception("No equipment list found.");
        }

        Console.WriteLine("Enter the search term:");
        string searchTerm = Console.ReadLine();

        // Check if the searchTerm is empty
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            throw new Exception("The search term cannot be empty.");
        }

        // Convert the search term to lower case for case-insensitive search
        string lowerCaseSearchTerm = searchTerm.ToLower();

        // Filter the equipment list to find matches
        var matchingEquipments = equipmentList.Where(e =>
            (e.Name.Length >= 5 && e.Name.ToLower().Substring(0, 5).Contains(lowerCaseSearchTerm)) ||
            e.Name.ToLower() == lowerCaseSearchTerm ||
            (e.Status.ToString().Length >= 5 && e.Status.ToString().ToLower().Substring(0, 5).Contains(lowerCaseSearchTerm)) ||
            e.Status.ToString().ToLower() == lowerCaseSearchTerm);

        // Check if any matches were found
        if (matchingEquipments.Any())
        {
            // Print the IDs of the matching equipment
            Console.WriteLine("Matching equipment IDs:");
            foreach (var equipment in matchingEquipments)
            {
                Console.WriteLine(equipment.Id);
            }
        }
        else
        {
            Console.WriteLine("No matching equipment found.");
        }
    }
}