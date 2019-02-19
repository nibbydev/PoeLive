using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using CsQuery;
using CsQuery.ExtensionMethods.Internal;
using Domain.PoeTrade.ApiDeserializer;

namespace Service {
    public static class HtmlParser {
        private static readonly Regex IntRegex = new Regex(@"\d+");

        public static Item[] ParsePoeTrade(string element) {
            var items = new List<Item>();
            var doc = CQ.Create(element);

            doc["div>.search-results>.item"].Each(domObject => {
                var domItem = new CQ(domObject);

                var domFirstLine = domItem.Find(".first-line");
                var domIcon = domFirstLine.Find(".icon-td>.icon");
                var domSockets = domIcon.Find(".sockets>.sockets-inner");
                var domItemCell = domFirstLine.Find(".item-cell");
                var domRequirements = domItemCell.Find(".requirements");
                var domMods = domItemCell.Find(".item-mods>li>ul");

                var item = new Item {
                    Seller = domItem.Attr("data-seller"),
                    Buyout = domItem.Attr("data-buyout"),
                    Ign = domItem.Attr("data-ign"),
                    League = domItem.Attr("data-league"),
                    Name = domItem.Attr("data-name"),
                    Tab = domItem.Attr("data-tab"),
                    X = int.Parse(domItem.Attr("data-x")),
                    Y = int.Parse(domItem.Attr("data-y")),
                    Icon = domIcon["img"].Attr("src"),

                    Sockets = ExtractSockets(domSockets),
                    Corrupted = !domItemCell["h5>a>span.corrupted"].IsNullOrEmpty(),
                    Requirements = ExtractRequirements(domRequirements),
                    Influence = ExtractInfluence(domMods),

                    Mods = ExtractMods(domMods),
                    Stats = ExtractStats(domFirstLine.Find(".table-stats>table>tbody"))
                };

                // Get item stack and sockets
                item.StackSize = ExtractStackSize(item.Icon);
                item.LargestLink = FindLargestLink(item.Sockets);

                items.Add(item);
            });

            return items.ToArray();
        }


        private static int? ExtractStackSize(string icon) {
            var split = icon.Split("?", 2);
            if (split.Length < 2) {
                return null;
            }

            foreach (var param in split[1].Split("&")) {
                var splitParam = param.Split("=");

                if (splitParam[0].Equals("stackSize")) {
                    return int.Parse(splitParam[1]);
                }
            }

            return null;
        }

        private static Socket[] ExtractSockets(CQ domObjects) {
            /*
                <div class="sockets-inner" style="position: relative; width:94px;">
                    <div class="socket socketD "></div>
                    <div class="socketLink socketLink0"></div>
                    <div class="socket socketI "></div>
                    <div class="socketLink socketLink1"></div>
                    <div class="socket socketS socketRight"></div>
                </div>
                
                or
                
                <div class="sockets-inner" style="position: relative; width:94px;"></div>
             */

            var sockets = new List<Socket>();

            // Loop though sockets and links
            foreach (var domObject in domObjects.Children()) {
                if (domObject.HasClass("socket")) {
                    var socket = new Socket {
                        Color = ExtractSocketColor(domObject)
                    };

                    sockets.Add(socket);
                } else if (domObject.HasClass("socketLink")) {
                    // Get previous socket and mark it as linked
                    sockets.Last().Linked = true;
                }
            }

            return sockets.IsNullOrEmpty() ? null : sockets.ToArray();
        }

        private static Socket.SocketColor? ExtractSocketColor(IDomObject domObject) {
            /*
                 <div class="socket socketS socketRight"></div>
             */

            if (domObject.HasClass("socketD")) {
                return Socket.SocketColor.Green;
            }

            if (domObject.HasClass("socketS")) {
                return Socket.SocketColor.Red;
            }

            if (domObject.HasClass("socketI")) {
                return Socket.SocketColor.Blue;
            }
            
            if (domObject.HasClass("socketG")) {
                return Socket.SocketColor.White;
            }
            
            if (domObject.HasClass("socketA")) {
                return Socket.SocketColor.Abyss;
            }

            return null;
        }

        private static int? FindLargestLink(Socket[] sockets) {
            if (sockets.IsNullOrEmpty()) {
                return null;
            }

            // Maximum number of possible links
            var links = new int[sockets.Length];
            var index = 0;

            // Count all link groups
            foreach (var socket in sockets) {
                if (socket.Linked) {
                    links[index]++;
                } else {
                    index++;
                }
            }

            // Return largest group
            return links.Max() + 1;
        }

        private static Requirement[] ExtractRequirements(CQ domObject) {
            /*
                <ul class="requirements proplist">
                   <li>Level:&nbsp;52</li>
                   <li>Intelligence:&nbsp;17</li>
                   <li><span class="sortable" data-name="ilvl">ilvl: 76</span></li>
                </ul>
                
                or
                
                <ul class="requirements proplist"></ul>
             */

            if (domObject.Children().IsNullOrEmpty()) {
                return null;
            }

            var requirements = new List<Requirement>();

            foreach (var domChild in domObject.Children()) {
                var requirement = new Requirement();
                requirements.Add(requirement);

                if (domChild.InnerHTML.Contains("Level")) {
                    requirement.Type = Requirement.RequirementType.Level;
                } else if (domChild.InnerHTML.Contains("Intelligence")) {
                    requirement.Type = Requirement.RequirementType.Int;
                } else if (domChild.InnerHTML.Contains("Strength")) {
                    requirement.Type = Requirement.RequirementType.Str;
                } else if (domChild.InnerHTML.Contains("Dexterity")) {
                    requirement.Type = Requirement.RequirementType.Dex;
                } else if (domChild.InnerHTML.Contains("ilvl")) {
                    requirement.Type = Requirement.RequirementType.ItemLevel;
                } else if (domChild.InnerHTML.Contains("Max sockets")) {
                    requirement.Type = Requirement.RequirementType.MaxSocket;
                }

                var match = IntRegex.Match(domChild.InnerHTML);
                if (match.Success) requirement.Value = int.Parse(match.Groups[0].Value);
            }


            return requirements.ToArray();
        }

        private static Mod[] ExtractMods(CQ domObject) {
            /*
                <ul class="mods">
                  <li class="sortable" data-name="#+#% to Cold Resistance" data-value="30.0" style="color:#3F6DB3">+<b>30</b>% to Cold Resistance
                     <span class="item-affix item-affix-S">
                     <span class="affix-info-short">S4</span>
                     <span class="affix-info-full">Tier 4 suffix: of the Walrus, min=[30] max=[35]</span>
                     </span>
                  </li>
                  <li class="sortable" data-name="#+#% to Lightning Resistance" data-value="13.0" style="color:#ADAA47">+<b>13</b>% to Lightning Resistance
                     <span class="item-affix item-affix-S">
                     <span class="affix-info-short">S7</span>
                     <span class="affix-info-full">Tier 7 suffix: of the Squall, min=[12] max=[17]</span>
                     </span>
                  </li>
                </ul>
                
                or
                
                <ul class="mods">
                    <li class="" data-name="#Enchants a rare item with a new random modifier" data-value="0" style="">Enchants a rare item with a new random modifier</li>
                </ul>
             */

            if (domObject.Children().IsNullOrEmpty()) {
                return null;
            }

            var mods = new List<Mod>();

            foreach (var domChild in domObject.Children()) {
                var name = domChild.GetAttribute("data-name");
                if (name.IsNullOrEmpty()) {
                    continue;
                }
                
                var mod = new Mod {Name = name};

                var val = domChild.GetAttribute("data-value");
                if (!val.IsNullOrEmpty()) {
                    mod.Value = double.Parse(val, CultureInfo.InvariantCulture);
                    
                    if (mod.Value.Equals(0.0)) {
                        mod.Value = null;
                    }
                }

                var tiers = domChild.Cq().Find(".item-affix>span");
                mod.TierShort = tiers.FirstOrDefault(t => t.HasClass("affix-info-short"))?.InnerHTML;
                mod.TierLong = tiers.FirstOrDefault(t => t.HasClass("affix-info-full"))?.InnerHTML;

                mods.Add(mod);
            }

            return mods.ToArray();
        }

        private static Stat[] ExtractStats(CQ domObject) {
            var stats = new List<Stat>();

            foreach (var domChild in domObject.Children()) {
                foreach (var domChildChild in domChild.ChildElements) {
                    var cq = domChildChild.Cq();

                    if (!cq.HasAttr("data-name")) {
                        continue;
                    }

                    var stat = new Stat {
                        Value = double.Parse(cq.Attr("data-value"), CultureInfo.InvariantCulture)
                    };

                    if (stat.Value <= 0) {
                        continue;
                    }

                    switch (cq.Attr("data-name")) {
                        case "q":
                            stat.Type = Stat.StatType.Quality;
                            break;
                        case "quality_pd":
                            stat.Type = Stat.StatType.Phys;
                            break;
                        case "ed":
                            stat.Type = Stat.StatType.Elem;
                            break;
                        case "aps":
                            stat.Type = Stat.StatType.Aps;
                            break;
                        case "quality_dps":
                            stat.Type = Stat.StatType.Dps;
                            break;
                        case "quality_pdps":
                            stat.Type = Stat.StatType.PDps;
                            break;
                        case "edps":
                            stat.Type = Stat.StatType.EDps;
                            break;

                        case "quality_armour":
                            stat.Type = Stat.StatType.Armour;
                            break;
                        case "quality_evasion":
                            stat.Type = Stat.StatType.Evasion;
                            break;
                        case "quality_shield":
                            stat.Type = Stat.StatType.Shield;
                            break;
                        case "block":
                            stat.Type = Stat.StatType.Block;
                            break;
                        case "crit":
                            stat.Type = Stat.StatType.Crit;
                            break;
                        case "level":
                            stat.Type = Stat.StatType.Level;
                            break;
                    }

                    stats.Add(stat);
                }
            }

            return stats.IsNullOrEmpty() ? null : stats.ToArray();
        }
        
        private static Influence? ExtractInfluence(CQ domObject) {
            var lastMod = domObject.Children().LastOrDefault();
            
            if (lastMod == null) {
                return null;
            }

            if (lastMod.InnerHTML.Contains("Shaped")) {
                return Influence.Shaper;
            } 
            
            if (lastMod.InnerHTML.Contains("Elder")) {
                return Influence.Elder;
            }

            return null;
        }
    }
}