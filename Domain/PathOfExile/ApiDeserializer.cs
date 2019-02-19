namespace Domain.PathOfExile {
    public class ApiDeserializer {
        public Result[] result { get; set; }
    }

    public class Result {
        public string id { get; set; }
        public Listing listing { get; set; }
        public Item item { get; set; }
    }

    public class Listing {
        public string method { get; set; }
        public string indexed { get; set; }
        public Stash stash { get; set; }
        public string whisper { get; set; }
        public Account account { get; set; }
        public Price price { get; set; }
    }

    public class Account {
        public string name { get; set; }
        public string lastCharacterName { get; set; }
    }

    public class Item {
        public int w { get; set; }
        public int h { get; set; }
        public int ilvl { get; set; }
        public string icon { get; set; }
        public string league { get; set; }
        public string name { get; set; }
        public string typeLine { get; set; }
        public bool identified { get; set; }
        public bool? corrupted { get; set; }
        public bool? shaper { get; set; }
        public bool? elder { get; set; }
        public string note { get; set; }
        public object properties { get; set; }
        public string[] explicitMods { get; set; }
        public string[] enchantMods { get; set; }
        public int frameType { get; set; }
        public int? stackSize { get; set; }
        public int? maxStackSize { get; set; }
        public object category { get; set; }
        public object extended { get; set; }
    }

    
    public class Price {
        public string type { get; set; }
        public double amount { get; set; }
        public string currency { get; set; }
    }

    public class Stash {
        public string name { get; set; }
        public int x { get; set; }
        public int y { get; set; }
    }
}