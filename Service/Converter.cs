using System;
using Domain;

namespace Service {
    public static class Converter {
        public static Item ConvertPoe(Domain.PathOfExile.Result result) {
            try {
                throw new NotImplementedException();
            } catch (Exception e) {
                Console.WriteLine(e);
                return null;
            }
        }
        
        public static Item ConvertPt(Domain.PoeTrade.ApiDeserializer.Item item) {
            try {
                throw new NotImplementedException();
            } catch (Exception e) {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}