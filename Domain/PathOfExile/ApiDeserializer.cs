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
        // 'forum' or 'psapi'
        public string method { get; set; }

        // timestamp
        public string indexed { get; set; }
        public string thread_id { get; set; }
        public Stash stash { get; set; }
        public string whisper { get; set; }

        public Account account { get; set; }

        // explicitly null if absent
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
        public Socket[] sockets { get; set; }
        public string name { get; set; }
        public string typeLine { get; set; }
        public bool identified { get; set; }
        public bool? corrupted { get; set; }
        public bool? shaper { get; set; }
        public bool? elder { get; set; }
        public string note { get; set; }
        public Property[] properties { get; set; }
        public Requirement[] requirements { get; set; }
        public string[] implicitMods { get; set; }
        public string[] explicitMods { get; set; }
        public string[] enchantMods { get; set; }
        public int frameType { get; set; }
        public int? stackSize { get; set; }
        public int? maxStackSize { get; set; }
        public object category { get; set; }
        public Extend extended { get; set; }
    }

    public class Socket {
        public int group { get; set; }
        public string attr { get; set; }
        public string sColour { get; set; }
    }

    public class Property {
        public string name { get; set; }
        public string[][] values { get; set; }
        public int displayMode { get; set; }
        public int type { get; set; }
    }

    public class Requirement {
        public string name { get; set; }
        public string[][] values { get; set; }
        public int displayMode { get; set; }
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

    public class Extend {
        public string text { get; set; }

        public int? ev { get; set; }
        public int? es { get; set; }
        public int? ar { get; set; }
        public bool? ev_aug { get; set; }
        public bool? es_aug { get; set; }
        public bool? ar_aug { get; set; }

        public ExtendMod[] mods { get; set; }
        public object hashes;
    }

    public class ExtendMod {
        public ExtendModXXplicit[] @implicit;
        public ExtendModXXplicit[] @explicit;
    }

    public class ExtendModXXplicit {
        public string name { get; set; }
        public string tier { get; set; }
        public ExtendModMagnitude[] magnitudes { get; set; }
    }

    public class ExtendModMagnitude {
        public string hash { get; set; }
        public int min { get; set; }
        public int max { get; set; }
    }
}