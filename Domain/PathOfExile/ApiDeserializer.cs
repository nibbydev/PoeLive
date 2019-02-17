namespace Domain.PathOfExile {
    public class ApiDeserializer {
        public class ApiResult {
            public class Listing {
                public class Stash {
                    public string name { get; set; }
                    public int x { get; set; }
                    public int y { get; set; }
                }

                public class Account {
                    public string name { get; set; }
                    public string lastCharacterName { get; set; }
                }
                
                public class Price {
                    public string type { get; set; }
                    public double amount { get; set; }
                    public string currency { get; set; }
                }

                public string method { get; set; }
                public string indexed { get; set; }
                public Stash stash { get; set; }
                public string whisper { get; set; }
                public Account account { get; set; }
                public Price price { get; set; }
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
            }

            public string id { get; set; }
            public Listing listing { get; set; }
            public Item item { get; set; }
        }

        public ApiResult[] result { get; set; }
    }
}