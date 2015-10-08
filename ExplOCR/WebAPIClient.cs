using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExplOCR
{
    class WebAPIClient
    {
        public void WebTransferItems(TransferItem[] items)
        {
            if (field_id == null)
            {
                GetInfoFieldID();
            }

            errorString = "";
            Dictionary<string, string> requestValues = new Dictionary<string, string>();
            try
            {
                // Build dictionary of POST request items
                CreateRequestValues(items, requestValues);
            }
            catch (Exception ex)
            {
                if (errorString != "")
                {
                    MessageBox.Show(errorString);
                    return;
                }
                else
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }

            // Build string for the API POST request from the dictionary.
            string content = "{" + Environment.NewLine +
                "\"input_values\":{" + Environment.NewLine;
            foreach(string key in requestValues.Keys)
            {
                content += "\""+key + "\":\"" + requestValues[key]+"\","+Environment.NewLine;
            }
            content = content.Substring(0, content.Length - (1+Environment.NewLine.Length));
            content += "}"+Environment.NewLine+"}";

            string response = "";
            try
            {
                // Network interaction
                HttpClient client = new HttpClient();
                Task<HttpResponseMessage> task = client.PostAsync(Properties.Settings.Default.APIServer, new StringContent(content));
                task.Wait();
                HttpResponseMessage message = task.Result;
                Task<string> task2 = message.Content.ReadAsStringAsync();
                response = task2.Result;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Server response was not processed: "+ex.Message);
            }

            try
            {
                // Parse the server response string.
                PrimitiveJSONObject json = new PrimitiveJSONObject(response, 0);
                string feedback = "";
                if (json.Content["status"].Value != "200")
                {
                    feedback = "Something went wrong with the network. HTTP says: "+json.Content["status"].Value;
                }
                else if (json.Content["response"].Content["is_valid"].Value != "true")
                {
                    feedback = "Not a valid submission. Error messages: " + Environment.NewLine;
                    foreach (PrimitiveJSONObject err in json.Content["response"].Content["validation_messages"].Content.Values)
                    {
                        feedback += err.Value + Environment.NewLine;
                    }
                }
                else
                {
                    feedback = "Success.";
                }

                MessageBox.Show(feedback);
            }
            catch
            {
                MessageBox.Show("Unable to parse server response:"+Environment.NewLine+response);
            }
        }

        private void CreateRequestValues(TransferItem[] items, Dictionary<string, string> requestValues)
        {
            RequireTextValue("SYSTEM", items);
            RequireValueExists("BODY", items);
            RequireValueExists("CUSTOM_CATEGORY", items);
            RequireValueExists("CUSTOM_DESCRIPTION", items);
            requestValues[field_id["user_login"]] = Properties.Settings.Default.IdentityName;
            requestValues[field_id["user_api_key"]] = Properties.Settings.Default.IdentityKey;
            requestValues[field_id["object_name"]] = TransferItem.FindItem("BODY", items).Values[0].Text;
            requestValues[field_id["system_name"]] = TransferItem.FindItem("SYSTEM", items).Values[0].Text;
            requestValues[field_id["first_discovered"]] = "";
            requestValues[field_id["system_description"]] = TransferItem.FindItem("CUSTOM_CATEGORY", items).Values[0].Text;
            requestValues[field_id["object_description"]] = TransferItem.FindItem("CUSTOM_DESCRIPTION", items).Values[0].Text;


            if (TransferItem.FindItem("EARTH_MASSES", items) == null)
            {
                CreateRequestStar(items, requestValues);
            }
            else if (TransferItem.FindItem("DESCRIPTION", items).Values[0].Text.StartsWith("Giant"))
            {
                CreateRequestGiant(items, requestValues);
            }
            else
            {
                CreateRequestPlanet(items, requestValues);
            }
        }

        private void CreateRequestPlanet(TransferItem[] items, Dictionary<string, string> requestValues)
        {            
            RequireTextValue("DESCRIPTION", items);
            RequireNumericalValue("EARTH_MASSES", items);
            RequireNumericalValue("RADIUS", items);
            RequireNumericalValue("SURFACE_TEMP", items);
            bool volcanism = TransferItem.FindItem("VOLCANISM_TYPE", items) != null;
            bool rings = TransferItem.FindItem("RING_TYPE", items) != null;
            bool terraform = TransferItem.FindItem("TERRAFORMING", items) != null;

            requestValues[field_id["object_type_group"]] = "Terrestrial Planet";
            requestValues[field_id["terrestrial_planet_type"]] = planetTypes[TransferItem.FindItem("DESCRIPTION", items).Values[0].Text];
            requestValues[field_id["earth_mass_terrestrial_planet"]] = TransferItem.FindItem("EARTH_MASSES", items).Values[0].Value.ToString();
            requestValues[field_id["radius_terrestrial_planet"]] = TransferItem.FindItem("RADIUS", items).Values[0].Value.ToString();
            requestValues[field_id["surface_temp_terrestrial_planet"]] = TransferItem.FindItem("SURFACE_TEMP", items).Values[0].Value.ToString();
            if (volcanism)
            {
                requestValues[field_id["volcanism_type"]] = CapitalizeWords(TransferItem.FindItem("VOLCANISM_TYPE", items).Values[0].Text);
                if (!volcanismTypes.ContainsKey(TransferItem.FindItem("VOLCANISM_TYPE", items).Values[0].Text))
                {
                    errorString += "Volcanism type " + TransferItem.FindItem("VOLCANISM_TYPE", items).Values[0].Text + " is not valid." + Environment.NewLine;
                }
            }

            requestValues[field_id["is_ringed_terrestrial_planet"]] = rings ? "Yes" : "No";
            if (terraform)
            {
                requestValues[field_id["terraform_status"]] = TransferItem.FindItem("TERRAFORMING", items).Values[0].Text.TrimEnd(new char[] { '.' });
            }
            else
            {
                requestValues[field_id["terraform_status"]] = "None";
            }

            AddAtmosphereInformation(items, requestValues);
            AddCompositionInformation(items, requestValues);
            AddOrbitInformation(items, requestValues);
            AddRotationInformation(items, requestValues);
            if (rings)
            {
                AddRingInformation(items, requestValues);
            }
        }

        private void CreateRequestGiant(TransferItem[] items, Dictionary<string, string> requestValues)
        {
            bool rings = TransferItem.FindItem("RING_TYPE", items) != null;

            RequireTextValue("DESCRIPTION", items);
            RequireNumericalValue("EARTH_MASSES", items);
            RequireNumericalValue("RADIUS", items);
            RequireNumericalValue("SURFACE_TEMP", items);

            requestValues[field_id["object_type_group"]] = "Gas Giant \\/ Water Giant";
            requestValues[field_id["gas_giant_type"]] = giantTypes[TransferItem.FindItem("DESCRIPTION", items).Values[0].Text];
            requestValues[field_id["earth_mass_gas_giant"]] = TransferItem.FindItem("EARTH_MASSES", items).Values[0].Value.ToString();
            requestValues[field_id["radius_gas_giant"]] = TransferItem.FindItem("RADIUS", items).Values[0].Value.ToString();
            requestValues[field_id["surface_temp_gas_giant"]] = TransferItem.FindItem("SURFACE_TEMP", items).Values[0].Value.ToString();
            requestValues[field_id["is_ringed_gas_giant"]] = rings ? "Yes" : "No";

            AddAtmosphereInformation(items, requestValues);
            AddOrbitInformation(items, requestValues);
            AddRotationInformation(items, requestValues);
            if (rings)
            {
                AddRingInformation(items, requestValues);
            }
        }

        private void CreateRequestStar(TransferItem[] items, Dictionary<string, string> requestValues)
        {
            bool multi = TransferItem.FindItem("ORBIT_MAJOR", items) != null;
            bool rings = TransferItem.FindItem("RING_TYPE", items) != null;

            RequireTextValue("DESCRPTION", items);
            RequireNumericalValue("AGE", items);
            RequireNumericalValue("SOLAR_MASSES", items);
            RequireNumericalValue("SOLAR_RADIUS", items);
            RequireNumericalValue("SURFACE_TEMP", items);

            requestValues[field_id["object_type_group"]] = "Star";
            requestValues[field_id["is_multi_star"]] = multi ? "Yes" : "No";
            requestValues[field_id["is_ringed_star"]] = rings ? "Yes" : "No";
            requestValues[field_id["star_type"]] = starTypes[TransferItem.FindItem("DESCRIPTION", items).Values[0].Text];
            requestValues[field_id["age"]] = TransferItem.FindItem("AGE", items).Values[0].Value.ToString();
            requestValues[field_id["solar_mass"]] = TransferItem.FindItem("SOLAR_MASSES", items).Values[0].Value.ToString();
            requestValues[field_id["solar_radius"]] = TransferItem.FindItem("SOLAR_RADIUS", items).Values[0].Value.ToString();
            requestValues[field_id["surface_temp_star"]] = TransferItem.FindItem("SURFACE_TEMP", items).Values[0].Value.ToString();
            if (TransferItem.FindItem("ID_HIPP", items) != null)
            {
                requestValues[field_id["star_cat_id_hipp"]] = TransferItem.FindItem("ID_HIPP", items).Values[0].Value.ToString();
            }
            if (TransferItem.FindItem("ID_HD", items) != null)
            {
                requestValues[field_id["star_cat_id_hd"]] = TransferItem.FindItem("ID_HD", items).Values[0].Value.ToString();
            }
            if (TransferItem.FindItem("ID_GLIESE", items) != null)
            {
                requestValues[field_id["star_cat_id_gliese"]] = TransferItem.FindItem("ID_GLIESE", items).Values[0].Value.ToString();
            }
            if (multi)
            {
                AddOrbitInformation(items, requestValues);
            }
            if (rings)
            {
                AddRingInformation(items, requestValues);
            }
        }

        private void AddRingInformation(TransferItem[] items, Dictionary<string, string> requestValues)
        {
            int count = 0;
            for (int i = 0; i < 5; i++)
            {
                if (TransferItem.FindItem("RING_TYPE", items, i+1) != null)
                {
                    count++;
                }
            }
            RequireTextValue("RING_TYPE", items, 1);
            RequireNumericalValue("RING_MASS", items, count);
            RequireNumericalValue("ORBIT_MAJOR", items, count+1);
            RequireNumericalValue("RING_INNER", items, count);
            RequireNumericalValue("RING_OUTER", items, count);

            requestValues[field_id["ring_reserves_types"]] = TransferItem.FindItem("MINING_RESERVES", items).Values[0].Text;
            requestValues[field_id["number_of_rings"]] = count.ToString();
            for (int i = 0; i < count; i++)
            {
                requestValues[field_id["ring_name_" + (i + 1).ToString()]] = (i + 1).ToString();
                requestValues[field_id["ring_type_" + (i + 1).ToString()]] = TransferItem.FindItem("RING_TYPE", items,i+1).Values[0].Text;
                requestValues[field_id["ring_mass_" + (i + 1).ToString()]] = TransferItem.FindItem("RING_MASS", items,i+1).Values[0].Value.ToString();
                requestValues[field_id["ring_semi_major_axis_" + (i + 1).ToString()]] = TransferItem.FindItem("ORBIT_MAJOR", items, i + 1).Values[0].Value.ToString();
                requestValues[field_id["ring_inner_radius_" + (i + 1).ToString()]] = TransferItem.FindItem("RING_INNER", items, i + 1).Values[0].Value.ToString();
                requestValues[field_id["ring_outer_radius_" + (i + 1).ToString()]] = TransferItem.FindItem("RING_OUTER", items, i + 1).Values[0].Value.ToString();
            }
        }

        private void AddCompositionInformation(TransferItem[] items, Dictionary<string, string> requestValues)
        {
            foreach (string s in compositionOrder)
            {
                requestValues[field_id[s.ToLower()]] = "";
            }

            RequireNumericalValue("COMPOSITION", items);
            foreach (TransferItemValue tiv in TransferItem.FindItem("COMPOSITION", items).Values)
            {
                if (compositionOrder.IndexOf(tiv.Text) < 0)
                {
                    continue;
                }
                requestValues[field_id["terrestrial_planet_composition"] + "_" + (1 + compositionOrder.IndexOf(tiv.Text)).ToString()] = CapitalizeWords(tiv.Text);
                requestValues[field_id[tiv.Text.ToLower()]] = tiv.Value.ToString();
            }
        }

        private void AddAtmosphereInformation(TransferItem[] items, Dictionary<string, string> requestValues)
        {
            TransferItem type = TransferItem.FindItem("ATMOSPHERE_TYPE", items);
            TransferItem composition = TransferItem.FindItem("ATMOSPHERE", items);
            TransferItem pressure = TransferItem.FindItem("SURFACE_PRESSURE", items);
            if (pressure != null)
            {
                requestValues[field_id["surface_pressure"]] = pressure.Values[0].Value.ToString();
            }
            if (type != null)
            {
                if (atmosphereTypes.ContainsKey(type.Values[0].Text))
                {
                    requestValues[field_id["atmosphere_type"]] = atmosphereTypes[type.Values[0].Text];
                }
                else if (type.Values[0].Text.StartsWith("SUITABLE FOR WATER"))
                {
                    requestValues[field_id["atmosphere_type"]] = atmosphereTypes["SUITABLE FOR WATER BASED LIFE"];
                }
            }
            if(composition != null)
            {
                foreach (TransferItemValue tiv in composition.Values)
                {
                    if (atmosphereOrder.IndexOf(tiv.Text) < 0)
                    {
                        continue;
                    }
                    requestValues[field_id["atmospheric_composition"]+"_"+(1+atmosphereOrder.IndexOf(tiv.Text)).ToString()] = tiv.Text;
                    requestValues[field_id[atmosphereElements[tiv.Text]]] = tiv.Value.ToString();
                }
            }
        }

        private void AddOrbitInformation(TransferItem[] items, Dictionary<string, string> requestValues)
        {
            RequireNumericalValue("ORBIT_PERIOD", items);
            RequireNumericalValue("ORBIT_MAJOR", items);
            RequireNumericalValue("ORBIT_ECCENTRICITY", items);
            RequireNumericalValue("ORBIT_INCLINATION", items);
            RequireNumericalValue("ORBIT_PERIAPSIS", items);

            requestValues[field_id["orbital_period"]] = TransferItem.FindItem("ORBIT_PERIOD", items).Values[0].Value.ToString();
            requestValues[field_id["semi_major_axis"]] = TransferItem.FindItem("ORBIT_MAJOR", items).Values[0].Value.ToString();
            requestValues[field_id["orbital_eccentricity"]] = TransferItem.FindItem("ORBIT_ECCENTRICITY", items).Values[0].Value.ToString();
            requestValues[field_id["orbital_inclination"]] = TransferItem.FindItem("ORBIT_INCLINATION", items).Values[0].Value.ToString();
            requestValues[field_id["arg_of_periapsis"]] = TransferItem.FindItem("ORBIT_PERIAPSIS", items).Values[0].Value.ToString();
        }

        private void AddRotationInformation(TransferItem[] items, Dictionary<string, string> requestValues)
        {
            RequireNumericalValue("ROTATION_PERIOD", items);
            RequireNumericalValue("ROTATION_TILT", items);
            RequireTextValue("ROTATION_LOCKED", items);

            requestValues[field_id["rotational_period"]] = TransferItem.FindItem("ROTATION_PERIOD", items).Values[0].Value.ToString();
            requestValues[field_id["axial_tilt"]] = TransferItem.FindItem("ROTATION_TILT", items).Values[0].Value.ToString();
            requestValues[field_id["is_tidally_locked"]] = CapitalizeWords(TransferItem.FindItem("ROTATION_LOCKED", items).Values[0].Text);
        }

        private bool RequireValueExists(string name, TransferItem[] items)
        {
            return RequireValueExists(name, items, 1);
        }

        private bool RequireNumericalValue(string name, TransferItem[] items)
        {
            return RequireNumericalValue(name, items, 1);
        }

        private bool RequireTextValue(string name, TransferItem[] items)
        {
            return RequireTextValue(name, items, 1);
        }

        private bool RequireValueExists(string name, TransferItem[] items, int count)
        {
            TransferItem ti = TransferItem.FindItem(name, items);
            if (ti == null || ti.Values == null || ti.Values.Count == 0)
            {
                errorString += "Value required: " + name + Environment.NewLine;
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool RequireTextValue(string name, TransferItem[] items, int count)
        {
            TransferItem ti = TransferItem.FindItem(name, items);
            if (ti == null || ti.Values == null || ti.Values.Count == 0 || string.IsNullOrEmpty(ti.Values[0].Text))
            {
                errorString += "Text value required: " + name + Environment.NewLine;
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool RequireNumericalValue(string name, TransferItem[] items, int count)
        {
            TransferItem ti = TransferItem.FindItem(name, items);
            if (ti == null || ti.Values == null || ti.Values.Count == 0 || double.IsNaN(ti.Values[0].Value))
            {
                errorString += "Numerical value required: " + name + Environment.NewLine;
                return false;
            }
            else
            {
                return true;
            }
        }

        private string CapitalizeWords(string p)
        {
            string[] words = p.ToLower().Split(new char[] { ' ' });
            for (int i = 0; i < words.Length; i++)
            {
                words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1);
            }
            return string.Join(" ", words);
        }

        private void GetInfoFieldID()
        {
            HttpClient client = new HttpClient();
            //client.BaseAddress = new Uri("http://www.elitegalaxyonline.com/gravityformsapi/forms/24/submissions");
            //client.PostAsync("http://www.elitegalaxyonline.com/gravityformsapi/forms/24/submissions", "");
            Task<HttpResponseMessage> task = client.GetAsync("http://www.elitegalaxyonline.com/api/ego_submissions/field_list/");
            task.Wait();
            HttpResponseMessage message = task.Result;
            Task<string> task2 = message.Content.ReadAsStringAsync();
            string test = task2.Result;

            field_id = new Dictionary<string, string>();
            PrimitiveJSONObject pjo = new PrimitiveJSONObject(test, 0);
            PrimitiveJSONObject[] array = pjo.Content["field_list"].Array;
            foreach (PrimitiveJSONObject obj in array)
            {
                field_id[obj.Content["field_name"].Value] = obj.Content["field_input_id"].Value;
            }


            starTypes = new Dictionary<string, string>();
            starTypes["ClassO"] = "Class O Star";
            starTypes["ClassB"] = "Class B Star";
            starTypes["ClassA"] = "Class A Star";
            starTypes["ClassF"] = "Class F Star";
            starTypes["ClassG"] = "Class G Star";
            starTypes["ClassK"] = "Class K Star";
            starTypes["ClassM"] = "Class M Star";
            starTypes["ClassL"] = "Class L Star";
            starTypes["ClassT"] = "Class T Star";
            starTypes["ClassY"] = "Class Y Star";
            starTypes["ClassTT"] = "T Tauri Type Star";
            starTypes["ClassS"] = "Class S Star";
            starTypes["ClassGK"] = "Orange Giant";
            starTypes["ClassGF"] = "Yellow-White Supergiant";
            starTypes["ClassGA"] = "Blue-White Supergiant";
            starTypes["ClassGM"] = "Red Giant";
            starTypes["ClassMS"] = "MS Class Star";
            starTypes["ClassC"]="Carbon Star";
            starTypes["ClassCN"] = "C-N Class Star";
            starTypes["ClassSGM"] = "Red Supergiant";
            starTypes["ClassH"] = "AE\\/BE Class Star";
            starTypes["ClassW"] = "Wolf-Rayet Star";
            starTypes["ClassDA"] = "White Dwarf - DA Class";
            starTypes["ClassDB"] = "White Dwarf - DB Class";
            starTypes["ClassDAB"] = "White Dwarf - DAB Class";
            starTypes["ClassDC"] = "White Dwarf - DC Class";
            starTypes["ClassDCV"] = "White Dwarf - DCV Class";
            starTypes["ClassN"]="Neutron Star";
            starTypes["ClassBH"]="Black Hole";
            starTypes["ClassSB"]="Supermassive Black Hole";

            giantTypes = new Dictionary<string, string>();
            giantTypes["Giant1"] = "Class I Gas Giant";
            giantTypes["Giant2"] = "Class II Gas Giant";
            giantTypes["Giant3"] = "Class III Gas Giant";
            giantTypes["Giant4"] = "Class IV Gas Giant";
            giantTypes["Giant5"] = "Class V Gas Giant";
            giantTypes["GiantH"] = "Helium-rich";
            giantTypes["GiantAL"] = "Gas giant with ammonia-based life";
            giantTypes["GiantWL"] = "Gas giant with water-based life";                                     
            giantTypes["GiantW"] = "Water Giant";

            planetTypes = new Dictionary<string, string>();
            planetTypes["PlanetI"] = "Icy planet";
            planetTypes["PlanetR"] = "Rocky planet";
            planetTypes["PlanetRI"] = "Rocky ice planet";
            planetTypes["PlanetM"] = "High metal content planet";
            planetTypes["PlanetMR"] = "Metal rich planet";
            planetTypes["PlanetW"] = "Water world";
            planetTypes["PlanetT"] = "Earth-like world";
            planetTypes["PlanetA"] = "Ammonia world";

            atmosphereTypes = new Dictionary<string, string>();
            atmosphereTypes["AMMONIA"] = "Ammonia";
            atmosphereTypes["AMMONIA AND OXYGEN"] = "Ammonia and Oxygen";
            atmosphereTypes["ARGON"] = "Argon";
            atmosphereTypes["ARGON-RICH"] = "Argon-Rich";
            atmosphereTypes["CARBON DIOXIDE"] = "Carbon Dioxide";
            atmosphereTypes["CARBON DIOXIDE-RICH"] = "Carbon Dioxide-Rich";
            atmosphereTypes["HELIUM"] = "Helium";
            atmosphereTypes["METHANE"] = "Methane";
            atmosphereTypes["METHANE-RICH"] = "Methane-Rich";
            atmosphereTypes["NEON"] = "Neon";
            atmosphereTypes["NEON-RICH"] = "Neon-Rich";
            atmosphereTypes["NITROGEN"] = "Nitrogen";
            atmosphereTypes["NITROGEN-RICH"] = "Nitrogen-Rich";
            atmosphereTypes["NO ATMOSPHERE"] = "No Atmosphere";
            atmosphereTypes["SILICATE VAPOUR"] = "Silicate Vapour";
            atmosphereTypes["SUITABLE FOR WATER BASED LIFE"] = "Suitable For Water-Based Life";
            atmosphereTypes["SULPHUR DIOXIDE"] = "Sulphur Dioxide";
            atmosphereTypes["WATER"] = "Water";
            atmosphereTypes["WATER-RIC"] = "Water-Rich";

            volcanismTypes = new Dictionary<string, string>();
            volcanismTypes["CARBON DIOXIDE GEYSERS"] = "Carbon Dioxide Geysers";
            volcanismTypes["IRON MAGMA"] = "Iron Magma";
            volcanismTypes["METHANE MAGMA"] = "Methane Magma";
            volcanismTypes["NITROGEN MAGMA"] = "Nitrogen Magma";
            volcanismTypes["NO VOLCANISM"] = "No Volcanism";
            volcanismTypes["SILICATE MAGMA"] = "Silicate Magma";
            volcanismTypes["SILICATE VAPOUR GEYSERS"] = "Silicate Vapour Geysers";
            volcanismTypes["WATER GEYSERS"] = "Water Geysers";
            volcanismTypes["WATER MAGMA"] = "Water Magma";

            compositionElements = new Dictionary<string, string>();
            compositionElements["ICE"] = "ice";
            compositionElements["ROCK"] = "rock";
            compositionElements["METAL"] = "metal";

            atmosphereElements = new Dictionary<string, string>();
            atmosphereElements["CARBON DIOXIDE"] = "carbon_dioxide";
            atmosphereElements["WATER"] = "water";
            atmosphereElements["NITROGEN"] = "nitrogen";
            atmosphereElements["HYDROGEN"] = "hydrogen";
            atmosphereElements["HELIUM"] = "helium";
            atmosphereElements["SULPHUR DIOXIDE"] = "sulphur_dioxide";
            atmosphereElements["ARGON"] = "argon";
            atmosphereElements["OXYGEN"] = "oxygen";
            atmosphereElements["AMMONIA"] = "ammonia";
            atmosphereElements["NEON"] = "neon";
            atmosphereElements["SILICATES"] = "silicates";
            atmosphereElements["IRON"] = "iron";

            compositionOrder = new List<string>(new string[] { "ICE", "ROCK", "METAL" });
            atmosphereOrder = new List<string>(new string[] { "CARBON DIOXIDE", "WATER", "NITROGEN", "HYDROGEN", "HELIUM", "SULPHUR DIOXIDE", 
                                             "ARGON", "OXYGEN", "METHANE", "AMMONIA", "NEON", "SILICATES", "IRON" });

        }

        Dictionary<string, string> field_id = null;
        Dictionary<string, string> starTypes = null;
        Dictionary<string, string> giantTypes = null;
        Dictionary<string, string> planetTypes = null;
        Dictionary<string, string> atmosphereTypes = null;
        Dictionary<string, string> volcanismTypes = null;
        Dictionary<string, string> atmosphereElements = null;
        Dictionary<string, string> compositionElements = null;
        List<string> atmosphereOrder;
        List<string> compositionOrder;
        string errorString = "";
    }

    class PrimitiveJSONObject
    {
        public PrimitiveJSONObject(string input, int index)
        {
            input = input.Trim();
            Read(input, 0);
        }

        private PrimitiveJSONObject()
        {
        }

        private int Read(string input, int index)
        {
            if (input[index]=='{')
            {
                index = ReadDictionary(input, index);
            }
            else if(input[index]=='[')
            {
                index = ReadArray(input, index);
            }
            else if(input[index]=='\"')
            {
                index = ReadPrimitive(input, index);
            }
            return index;
        }

        private int ReadPrimitive(string input, int index)
        {
            index++;
            int end = input.IndexOf('\"', index);
            if (end < 0)
            {
                throw new Exception("Bad JSON format.");
            }
            Value = input.Substring(index, end - index);
            return ++end;
        }

        private int ReadArray(string input, int index)
        {
            index++;
            List<PrimitiveJSONObject> objects = new List<PrimitiveJSONObject>();
            while (true)
            {
                if (input[index] == '{' || input[index] == '[' || input[index] == '\"')
                {
                    PrimitiveJSONObject sub = new PrimitiveJSONObject();
                    index = sub.Read(input, index);
                    objects.Add(sub);
                    if (input[index] == ',')
                    {
                        index++;
                    }
                }
                else if (input[index] == ']')
                {
                    index++;
                    break;
                }
                else
                {
                    throw new Exception("Bad JSON format.");
                }
            }
            Array = objects.ToArray();
            return index;
        }

        private int ReadDictionary(string input, int index)
        {
            index++;
            Content = new Dictionary<string, PrimitiveJSONObject>();
            while (true)
            {
                string key;
                if (input[index] == '\"')
                {
                    PrimitiveJSONObject sub = new PrimitiveJSONObject();
                    index = sub.Read(input, index);
                    key = sub.Value;
                }
                else if (input[index] == '}')
                {
                    return ++index;
                }
                else
                {
                    throw new Exception("Bad JSON format.");
                }

                if (input[index] == ':')
                {
                    index++;
                }
                else
                {
                    throw new Exception("Bad JSON format.");
                }

                if (input[index] == '{' || input[index] == '[' || input[index] == '\"')
                {
                    PrimitiveJSONObject sub = new PrimitiveJSONObject();
                    index = sub.Read(input, index);
                    Content[key] = sub;

                    if (input[index] == ',')
                    {
                        index++;
                    }
                }
                else
                {
                    int index2 = input.IndexOf(',', index);
                    Content[key] = new PrimitiveJSONObject("\"" + input.Substring(index, index2 - index) + "\"", 0);
                    index = index2 + 1;
                }
            }
        }

        private string CapitalizeWords(string p)
        {
            string[] words = p.ToLower().Split(new char[] { ' ' });
            for (int i = 0; i < words.Length; i++)
            {
                words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1);
            }
            return string.Join(" ", words);
        }

        public Dictionary<string, PrimitiveJSONObject> Content;
        public PrimitiveJSONObject[] Array;
        public string Value;
    }
}
