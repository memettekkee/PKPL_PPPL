using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.Runtime.InteropServices;
using static System.Windows.Forms.AxHost;
using Newtonsoft.Json;
using static xtUML1.JsonData;

namespace xtUML1
{
    public partial class Form1 : Form
    {
        //private OpenFileDialog openFileDialog;
        //private SaveFileDialog saveFileDialog;

        private readonly StringBuilder sourceCodeBuilder;
        private string selectedFilePath;
        private string ConstructName;
        private bool isJsonFileSelected = false;
        private bool isBtnParseClicked = false;
        private string status;

        public Form1()
        {
            InitializeComponent();
            sourceCodeBuilder = new StringBuilder();

            //openFileDialog = new OpenFileDialog();
            //saveFileDialog = new SaveFileDialog();
        }

        private void btnSelect_Click_1(object sender, EventArgs e)
        {
            // Menampilkan dialog untuk memilih file JSON
            OpenFileDialog filePath = new OpenFileDialog();
            filePath.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
            filePath.Title = "Select JSON File";
            //DialogResult result = openFileDialog.ShowDialog();

            if (filePath.ShowDialog() == DialogResult.OK)
            {

                selectedFilePath = filePath.FileName;

                // ubah semua sesuai kebutuhan

                //string jsonContent = File.ReadAllText(openFileDialog.FileName);

                //dynamic jsonObj = JObject.Parse(jsonContent);

                //selectedFilePath = openFileDialog.FileName;

                textBox1.Text = selectedFilePath;

                // tampilkan sintax error parsing di textBox4 jika tidak lolos parsing

                /*msgBox.Text = jsonContent;*/ // untuk menampilkan isi file json (setelah lolos parsing)

                isJsonFileSelected = true;
            }
        }

        private JArray ProcessJson(string filePath)
        {
            JArray jsonArray;
            try
            {
                string jsonContent = File.ReadAllText(filePath);
                CheckJsonCompliance(jsonContent);
                jsonArray = new JArray(JToken.Parse(jsonContent));
                msgBox.Text = jsonArray.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading the file {Path.GetFileName(filePath)}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                jsonArray = new JArray();
            }

            return jsonArray;
        }

        private void btnParse_Click(object sender, EventArgs e)
        {
            // parsing
            // tampilkan sintax error parsing di textBox4 jika tidak lolos parsing
            // tampilkan isi file json di textBox4 jika lolos parsing

            if (selectedFilePath == null || selectedFilePath.Length == 0)
            {
                MessageBox.Show("Please select a folder containing JSON files first.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

                return;
            }

            JArray jsonArray = this.ProcessJson(selectedFilePath);

            msgBox.Clear();

            CheckParsing15.Point1(this, jsonArray);
            CheckParsing15.Point2(this, jsonArray);
            CheckParsing15.Point3(this, jsonArray);
            CheckParsing15.Point4(this, jsonArray);
            CheckParsing15.Point5(this, jsonArray);
            CheckParsing610.Point6(this, jsonArray);
            CheckParsing610.Point7(this, jsonArray);
            CheckParsing610.Point8(this, jsonArray);
            CheckParsing610.Point9(this, jsonArray);
            CheckParsing610.Point10(this, jsonArray);
            CheckParsing1115.Point11(this, jsonArray);
            CheckParsing1115.Point13(this, jsonArray);
            CheckParsing1115.Point14(this, jsonArray);
            CheckParsing1115.Point15(this, jsonArray);

            //btnCheck_Click1(sender, e);

            ParsingPoint.Point25(this, jsonArray);
            ParsingPoint.Point27(this, jsonArray);
            ParsingPoint.Point28(this, jsonArray);
            ParsingPoint.Point29(this, jsonArray);
            ParsingPoint.Point30(this, jsonArray);
            ParsingPoint.Point34(this, jsonArray);
            ParsingPoint.Point35(this, jsonArray);

            CheckParsing1115.Point99(this, jsonArray);


            if (string.IsNullOrWhiteSpace(msgBox.Text))
            {
                msgBox.Text = "Model has successfully passed parsing";
                isBtnParseClicked = true;
            }

        }

        public RichTextBox GetMessageBox()
        {
            return msgBox;
        }

        private void HandleError(string errorMessage)
        {
            msgBox.Text += $"{errorMessage}{Environment.NewLine}";
            Console.WriteLine(errorMessage);
        }

        private void CheckJsonCompliance(string jsonContent)
        {
            try
            {
                JObject jsonObj = JObject.Parse(jsonContent);

                // Dictionary to store state model information
                Dictionary<string, string> stateModels = new Dictionary<string, string>();
                HashSet<string> usedKeyLetters = new HashSet<string>();
                HashSet<int> stateNumbers = new HashSet<int>();

                JToken subsystemsToken = jsonObj["subsystems"];
                if (subsystemsToken != null && subsystemsToken.Type == JTokenType.Array)
                {
                    // Iterasi untuk setiap subsystem dalam subsystemsToken
                    foreach (var subsystem in subsystemsToken)
                    {
                        JToken modelToken = subsystem["model"];
                        if (modelToken != null && modelToken.Type == JTokenType.Array)
                        {
                            foreach (var model in modelToken)
                            {
                                ValidateClassModel(model, stateModels, usedKeyLetters, stateNumbers);
                            }
                        }
                    }

                    // Setelah memvalidasi semua model, panggil ValidateEventDirectedToStateModelHelper untuk setiap subsystem
                    foreach (var subsystem in subsystemsToken)
                    {
                        ValidateEventDirectedToStateModelHelper(subsystem["model"], stateModels, null);
                    }
                }

                ValidateTimerModel(jsonObj, usedKeyLetters);
            }
            catch (Exception ex)
            {
                HandleError($"Error: {ex.Message}");
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ValidateClassModel(JToken model, Dictionary<string, string> stateModels, HashSet<string> usedKeyLetters, HashSet<int> stateNumbers)
        {
            string objectType = model["type"]?.ToString();
            string objectName = model["class_name"]?.ToString();
            Console.WriteLine($"Running CheckKeyLetterUniqueness for {objectName}");

            if (objectType == "class")
            {
                Console.WriteLine($"Checking class: {objectName}");

                string assignerStateModelName = $"{objectName}_ASSIGNER";
                JToken assignerStateModelToken = model[assignerStateModelName];

                if (assignerStateModelToken == null || assignerStateModelToken.Type != JTokenType.Object)
                {
                    HandleError($"Syntax error 16: Assigner state model not found for {objectName}.");
                    return;
                }

                string keyLetter = model["KL"]?.ToString();

                // Pemanggilan CheckKeyLetterUniqueness
                CheckKeyLetterUniqueness(usedKeyLetters, keyLetter, objectName);

                // Check if KeyLetter is correct
                JToken keyLetterToken = assignerStateModelToken?["KeyLetter"];
                if (keyLetterToken != null && keyLetterToken.ToString() != keyLetter)
                {
                    HandleError($"Syntax error 17: KeyLetter for {objectName} does not match the rules.");
                }

                // Check uniqueness of states
                CheckStateUniqueness(stateModels, assignerStateModelToken?["states"], objectName, assignerStateModelName);

                // Check uniqueness of state numbers
                CheckStateNumberUniqueness(stateNumbers, assignerStateModelToken?["states"], objectName);

                // Store state model information
                string stateModelKey = $"{objectName}.{assignerStateModelName}";
                stateModels[stateModelKey] = objectName;
            }
        }

        private void CheckStateUniqueness(Dictionary<string, string> stateModels, JToken statesToken, string objectName, string assignerStateModelName)
        {
            if (statesToken is JArray states)
            {
                HashSet<string> uniqueStates = new HashSet<string>();

                foreach (var state in states)
                {
                    string stateName = state["state_name"]?.ToString();
                    string stateModelName = $"{objectName}.{stateName}";

                    // Check uniqueness of state model
                    if (!uniqueStates.Add(stateModelName))
                    {
                        HandleError($"Syntax error 18: State {stateModelName} is not unique in {assignerStateModelName}.");
                    }
                }
            }
        }

        private void CheckStateNumberUniqueness(HashSet<int> stateNumbers, JToken statesToken, string objectName)
        {
            if (statesToken is JArray stateArray)
            {
                foreach (var state in stateArray)
                {
                    int stateNumber = state["state_number"]?.ToObject<int>() ?? 0;

                    if (!stateNumbers.Add(stateNumber))
                    {
                        HandleError($"Syntax error 19: State number {stateNumber} is not unique.");
                    }
                }
            }
        }

        private void CheckKeyLetterUniqueness(HashSet<string> usedKeyLetters, string keyLetter, string objectName)
        {
            string expectedKeyLetter = $"{keyLetter}_A";
            Console.WriteLine("Running ValidateClassModel");
            Console.WriteLine($"Checking KeyLetter uniqueness: {expectedKeyLetter} for {objectName}");

            if (!usedKeyLetters.Add(expectedKeyLetter))
            {
                HandleError($"Syntax error 20: KeyLetter for {objectName} is not unique.");
            }
        }

        private void ValidateTimerModel(JObject jsonObj, HashSet<string> usedKeyLetters)
        {
            string timerKeyLetter = jsonObj["subsystems"]?[0]?["model"]?[0]?["KL"]?.ToString();
            string timerStateModelName = $"{timerKeyLetter}_ASSIGNER";

            JToken timerModelToken = jsonObj["subsystems"]?[0]?["model"]?[0];
            JToken timerStateModelToken = jsonObj["subsystems"]?[0]?["model"]?[0]?[timerStateModelName];

            // Check if Timer state model exists
            if (timerStateModelToken == null || timerStateModelToken.Type != JTokenType.Object)
            {
                HandleError($"Syntax error 21: Timer state model not found for TIMER.");
                return;
            }

            // Check KeyLetter of Timer state model
            JToken keyLetterToken = timerStateModelToken?["KeyLetter"];
            if (keyLetterToken == null || keyLetterToken.ToString() != timerKeyLetter)
            {
                HandleError($"Syntax error 21: KeyLetter for TIMER does not match the rules.");
            }
        }

        private void ValidateEventDirectedToStateModelHelper(JToken modelsToken, Dictionary<string, string> stateModels, string modelName)
        {
            if (modelsToken != null && modelsToken.Type == JTokenType.Array)
            {
                foreach (var model in modelsToken)
                {
                    string modelType = model["type"]?.ToString();
                    string className = model["class_name"]?.ToString();

                    if (modelType == "class")
                    {
                        JToken assignerToken = model[$"{className}_ASSIGNER"];

                        if (assignerToken != null)
                        {
                            Console.WriteLine($"assignerToken.Type: {assignerToken.Type}");

                            if (assignerToken.Type == JTokenType.Object)
                            {
                                JToken statesToken = assignerToken["states"];

                                if (statesToken != null && statesToken.Type == JTokenType.Array)
                                {
                                    JArray statesArray = (JArray)statesToken;

                                    foreach (var stateItem in statesArray)
                                    {
                                        string stateName = stateItem["state_name"]?.ToString();
                                        string stateModelName = $"{modelName}.{stateName}";

                                        JToken eventsToken = stateItem["events"];
                                        if (eventsToken is JArray events)
                                        {
                                            foreach (var evt in events)
                                            {
                                                string eventName = evt["event_name"]?.ToString();
                                                JToken targetsToken = evt["targets"];

                                                if (targetsToken is JArray targets)
                                                {
                                                    foreach (var target in targets)
                                                    {
                                                        string targetStateModel = target?.ToString();

                                                        // Check if target state model is in the state models dictionary
                                                        if (!stateModels.ContainsKey(targetStateModel))
                                                        {
                                                            HandleError($"Syntax error 24: Event '{eventName}' in state '{stateModelName}' targets non-existent state model '{targetStateModel}'.");
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void btnTranslate_Click(object sender, EventArgs e)
        {
            // translate jika lolos parsing
            // tampilkan hasil translate di textBox3

            // yg kutandain klo mau di aktifin fitur check parse dlu baru translet itu dibuka komennya

            // bawah ini

            //if (isBtnParseClicked) 
            //{
            try
            {
                if (!string.IsNullOrEmpty(selectedFilePath) && File.Exists(selectedFilePath))
                {
                    sourceCodeBuilder.Clear();
                    GenerateJava(selectedFilePath);
                    msgBox.Text = File.ReadAllText(selectedFilePath);
                }
                else
                {
                    MessageBox.Show("Please select a valid JSON file first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating Java code: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // sama bawah ini

            //}



            //else
            //{
            //    MessageBox.Show("Please click Parse the file first before Translating it.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}


        }

        private void GenerateJava(string FilePath)
        {
            string umlDiagramJson = File.ReadAllText(FilePath);

            // Decode JSON data
            JsonData json = JsonConvert.DeserializeObject<JsonData>(umlDiagramJson);

            // Generate into Java 
            GeneratePackage(json.sub_name);

            foreach (var model in json.model)
            {
                if (model.type == "class")
                {
                    GenerateClass(model, json);
                }
                else if (model.type == "association" && model.model != null)
                {
                    GenerateAssociationClass(model.model);
                }
                if (model.type == "imported_class")
                {
                    sourceCodeBuilder.AppendLine($"//Imported Class");
                    GenerateImportedClass(model, json);
                }
            }

            bool generateAssocClass = json.model.Any(model => model.type == "association");

            //bool generateAssoc = json.model.Any(model => model.type == "association");

            //if (generateAssocClass)
            //{
            //    sourceCodeBuilder.AppendLine($"// Generate Association Class");
            //    GenerateAssocClass();
            //}


            //foreach (var model in json.model)
            //{
            //    if (model.type == "association")
            //    {
            //        GenerateObjAssociation(model);
            //    }
            //}

            //if (generateAssoc)
            //{
            //    GenerateAssoc();
            //}

            // Display or save the generated Java code

            richTextBox2.Text = sourceCodeBuilder.ToString();
        }

        private void GeneratePackage(string packageName)
        {
            sourceCodeBuilder.AppendLine($"package {packageName};\n");
        }

        private void GenerateClass(JsonData.Model model, JsonData json)
        {
            sourceCodeBuilder.AppendLine($"public class {model.class_name} {{");
            this.ConstructName = model.class_name;

            var sortedAttributes = model.attributes.OrderBy(attr => attr.attribute_name);

            foreach (var attribute in model.attributes)
            {
                GenerateAttribute(attribute, json);
            }

            sourceCodeBuilder.AppendLine("");

            //foreach (var status in model.attributes)
            //{
            //    GenerateState(status);
            //}

            //sourceCodeBuilder.AppendLine("");

            if (model.attributes != null)
            {
                GenerateConstructor(model.attributes);
            }

            sourceCodeBuilder.AppendLine("");

            foreach (var attribute in model.attributes)
            {
                GenerateGetter(attribute);
            }

            sourceCodeBuilder.AppendLine("");

            foreach (var attribute in model.attributes)
            {
                GenerateSetter(attribute);
            }

            sourceCodeBuilder.AppendLine("");

            if (model.states != null)
            {
                status = StateStatus(model.attributes.FirstOrDefault(attr => attr.data_type == "state"));

                Transition(model.states, status);

                GenerateStateTransitionMethods(model.states, status);

                foreach (var stateAttribute in model.attributes.Where(attr => attr.data_type == "state"))
                {
                    GenerateGetState(stateAttribute);
                }

            }

            //if (model.states != null)
            //{
            //    GenerateGetState();
            //}


            sourceCodeBuilder.AppendLine("}\n\n");
        }
        private void GenerateAttribute(JsonData.Attribute1 attribute, JsonData json)
        {
            // Adjust data types as needed
            string dataType = MapDataType(attribute.data_type);

            if (attribute.data_type != "state" && attribute.data_type != "inst_event" && attribute.data_type != "inst_ref" && attribute.data_type != "inst_ref_set" && attribute.data_type != "inst_ref_<timer>" && attribute.data_type != "inst_event")
            {
                sourceCodeBuilder.AppendLine($"\tprivate {dataType} {attribute.attribute_name};");
            }
            else if (attribute.data_type == "state")
            {
                sourceCodeBuilder.AppendLine($"\tprivate {attribute.attribute_name};");
            }
            else if (attribute.data_type == "inst_ref_<timer>")
            {
                sourceCodeBuilder.AppendLine($"\tprivate {dataType} {attribute.attribute_name};");
            }
            else if (attribute.data_type == "inst_ref")
            {
                sourceCodeBuilder.AppendLine($"\tprivate {attribute.related_class_name} {attribute.attribute_name}Ref;");
            }
            else if (attribute.data_type == "inst_ref_set")
            {
                sourceCodeBuilder.AppendLine($"\tprivate {attribute.related_class_name} {attribute.attribute_name}RefSet = [];");
            }
            else if (attribute.data_type == "inst_event")
            {
                string cName = null;
                string sName = null;

                foreach (JsonData.Model modell in json.model)
                {
                    if (modell.class_id == attribute.class_id)
                    {
                        cName = modell.class_name;
                    }
                }

                // kemungkinan berubah
                foreach (JsonData.Model state in json.model)
                {
                    if (state.states != null)
                    {
                        foreach (var states in state.states)
                        {
                            if (states.state_id == attribute.state_id && states.state_name == attribute.state_name)
                            {
                                sName = states.state_event[0];
                            }
                        }
                    }
                }

                sourceCodeBuilder.AppendLine($"\n\tprivate void {attribute.event_name}({cName} {cName}) {{");
                sourceCodeBuilder.AppendLine($"\t\t{cName}.{sName}();");
                sourceCodeBuilder.AppendLine($"\t}}");
            }
            else
            {
                return;
            }

        }

        //private void GenerateState(JsonData.Attribute1 status)
        //{
        //    if (status.attribute_name == "status")
        //    {
        //        sourceCodeBuilder.AppendLine("    private String state;");
        //    }
        //}

        private void GenerateAssociationClass(JsonData.Model associationModel)
        {
            // Check if associationModel is not null
            if (associationModel == null)
            {
                // Handle the case where associationModel is null, e.g., throw an exception or log a message
                return;
            }

            sourceCodeBuilder.AppendLine($"public class assoc_{associationModel.class_name} {{");

            foreach (var attribute in associationModel.attributes)
            {
                // Adjust data types as needed
                string dataType = MapDataType(attribute.data_type);

                sourceCodeBuilder.AppendLine($"\tprivate {dataType} {attribute.attribute_name};");
            }

            // Check if associatedClass.@class is not null before iterating
            if (associationModel.@class != null)
            {
                foreach (var associatedClass in associationModel.@class)
                {
                    if (associatedClass.class_multiplicity == "1..1")
                    {
                        sourceCodeBuilder.AppendLine($"\tprivate {associatedClass.class_name} {associatedClass.class_name};");
                    }
                    else
                    {
                        sourceCodeBuilder.AppendLine($"\tprivate array {associatedClass.class_name}List;");
                    }
                }
            }

            sourceCodeBuilder.AppendLine("");

            if (associationModel.attributes != null)
            {
                GenerateConstructor(associationModel.attributes);
            }

            foreach (var attribute in associationModel.attributes)
            {
                GenerateGetter(attribute);
            }

            foreach (var attribute in associationModel.attributes)
            {
                GenerateSetter(attribute);
            }
            sourceCodeBuilder.AppendLine("}\n\n");
        }

        private void GenerateImportedClass(JsonData.Model imported, JsonData json)
        {
            if (imported == null)
            {
                return;
            }
            sourceCodeBuilder.AppendLine($"class {imported.class_name} {{");

            foreach (var attribute in imported.attributes)
            {
                GenerateAttribute(attribute, json);
            }

            sourceCodeBuilder.AppendLine("");

            if (imported.attributes != null)
            {
                GenerateConstructor(imported.attributes);
            }

            sourceCodeBuilder.AppendLine("");

            foreach (var attribute in imported.attributes)
            {
                GenerateGetter(attribute);
            }

            sourceCodeBuilder.AppendLine("");

            foreach (var attribute in imported.attributes)
            {
                GenerateSetter(attribute);
            }

            sourceCodeBuilder.AppendLine("");

            if (imported.states != null)
            {
                status = StateStatus(imported.attributes.FirstOrDefault(attr => attr.data_type == "state"));

                Transition(imported.states, status);

                GenerateStateTransitionMethods(imported.states, status);

                foreach (var stateAttribute in imported.attributes.Where(attr => attr.data_type == "state"))
                {
                    GenerateGetState(stateAttribute);
                }
            }
        }

        private void GenerateConstructor(List<JsonData.Attribute1> attributes)
        {

            sourceCodeBuilder.Append($"\tpublic {this.ConstructName} (");

            foreach (var attribute in attributes)
            {
                if (attribute.data_type != "state" && attribute.data_type != "inst_ref_<timer>" && attribute.data_type != "inst_ref" && attribute.data_type != "inst_ref_set" && attribute.data_type != "inst_event")
                {
                    string dataType = MapDataType(attribute.data_type);
                    sourceCodeBuilder.Append($"{dataType} {attribute.attribute_name}, ");
                    //sourceCodeBuilder.Append($"{attribute.attribute_name},");
                }
                else if (attribute.data_type == "inst_ref_<timer>")
                {
                    sourceCodeBuilder.Append($"TIMER {attribute.attribute_name},");
                }
                else if (attribute.data_type == "inst_ref")
                {
                    sourceCodeBuilder.Append($"{attribute.related_class_name} {attribute.attribute_name}Ref,");
                }
                else if (attribute.data_type == "inst_ref_set")
                {
                    sourceCodeBuilder.Append($"{attribute.related_class_name} {attribute.attribute_name}RefSet,");
                }

            }

            // Remove the trailing comma and add the closing parenthesis
            if (attributes.Any())
            {
                sourceCodeBuilder.Length -= 1; // Remove the last character (",")
            }

            sourceCodeBuilder.AppendLine(") {");

            foreach (var attribute in attributes)
            {
                if (attribute.data_type != "state" && attribute.data_type != "inst_ref_<timer>" && attribute.data_type != "inst_ref" && attribute.data_type != "inst_ref_set" && attribute.data_type != "inst_event")
                {
                    sourceCodeBuilder.AppendLine($"\t\tthis.{attribute.attribute_name} = {attribute.attribute_name};");
                }
                else if (attribute.data_type == "inst_ref")
                {
                    sourceCodeBuilder.AppendLine($"\t\tthis.{attribute.attribute_name}Ref = {attribute.attribute_name}Ref;");
                }
                else if (attribute.data_type == "inst_ref_set")
                {
                    sourceCodeBuilder.AppendLine($"\t\tthis.{attribute.attribute_name}RefSet[] = {attribute.attribute_name}RefSet;");
                }
                else if (attribute.data_type == "inst_ref_<timer>")
                {
                    sourceCodeBuilder.AppendLine($"\t\tthis.{attribute.attribute_name} = {attribute.attribute_name}");
                }
            }

            //foreach (var attribute in attributes)
            //{
            //    //if (attribute.attribute_name != "status")
            //    if (attribute.data_type != "state")
            //    {

            //        string dataType = MapDataType(attribute.data_type);
            //        sourceCodeBuilder.Append($"{dataType} {attribute.attribute_name}, ");
            //    }

            //}

            //// Remove the trailing comma and add the closing parenthesis
            //if (attributes.Any())
            //{
            //    sourceCodeBuilder.Length -= 1; // Remove the last character (",")
            //}

            //sourceCodeBuilder.AppendLine(") {");

            //foreach (var attribute in attributes)
            //{
            //    //if (attribute.attribute_name != "status")
            //    if (attribute.data_type != "state")
            //    {
            //        sourceCodeBuilder.AppendLine($"        this.{attribute.attribute_name} = {attribute.attribute_name};");
            //    }
            //}


            // Handle the "state" datatype separately outside the loop
            var stateAttribute = attributes.FirstOrDefault(attr => attr.data_type == "state");
            if (stateAttribute != null)
            {
                // Check if the attribute has a default value and it is a string
                if (!string.IsNullOrEmpty(stateAttribute.default_value) && stateAttribute.data_type.ToLower() == "state")
                {
                    int lastDotIndex = stateAttribute.default_value.LastIndexOf('.');
                    // Replace "status" with "state" and "aktif" with "active"
                    string stringValue = stateAttribute.default_value.Substring(lastDotIndex + 1);
                    sourceCodeBuilder.AppendLine($"\t\tthis.{stateAttribute.attribute_name} = \"{stringValue}\";");
                }
            }

            sourceCodeBuilder.AppendLine("\t}");
        }

        private void GenerateGetter(JsonData.Attribute1 getter)
        {
            //if (getter.attribute_name != "status")
            if (getter.data_type != "state" && getter.data_type != "inst_ref_<timer>" && getter.data_type != "inst_ref" && getter.data_type != "inst_ref_set" && getter.data_type != "inst_event")
            {
                string dataType = MapDataType(getter.data_type);
                sourceCodeBuilder.AppendLine($"\tpublic {dataType} get{getter.attribute_name}() {{"); // ini belom diubah
                sourceCodeBuilder.AppendLine($"\t\tthis.{getter.attribute_name};");
                sourceCodeBuilder.AppendLine($"\t}}");
            }
            else if (getter.data_type == "inst_ref_<timer>")
            {
                string dataType = MapDataType(getter.data_type);
                sourceCodeBuilder.AppendLine($"\tpublic {dataType} get{getter.attribute_name}() {{");
                sourceCodeBuilder.AppendLine($"\t\treturn this.{getter.attribute_name};");
                sourceCodeBuilder.AppendLine($"\t}}");
            }
            else if (getter.data_type == "inst_ref")
            {
                string dataType = MapDataType(getter.data_type);
                sourceCodeBuilder.AppendLine($"\tpublic {dataType} get{getter.attribute_name}Ref() {{");
                sourceCodeBuilder.AppendLine($"\t\treturn this.{getter.attribute_name}Ref;");
                sourceCodeBuilder.AppendLine($"\t}}");
            }
            else if (getter.data_type == "inst_ref_set")
            {
                string dataType = MapDataType(getter.data_type);
                sourceCodeBuilder.AppendLine($"\tpublic {dataType} get{getter.attribute_name}RefSet() {{");
                sourceCodeBuilder.AppendLine($"\t\treturn this.{getter.attribute_name}RefSet;");
                sourceCodeBuilder.AppendLine($"\t}}");
            }
            else if (getter.data_type == "inst_event")
            {
                return;
            }

        }
        private void GenerateSetter(JsonData.Attribute1 setter)
        {
            //if (setter.attribute_name != "status")
            if (setter.data_type != "state" && setter.data_type != "inst_ref_<timer>" && setter.data_type != "inst_ref" && setter.data_type != "inst_ref_set" && setter.data_type != "inst_event")
            {
                string dataType = MapDataType(setter.data_type);
                sourceCodeBuilder.AppendLine($"\tpublic void set{setter.attribute_name}({dataType} {setter.attribute_name}) {{"); // ini ( String get() ) nya belom jadi
                sourceCodeBuilder.AppendLine($"\t\tthis.{setter.attribute_name} = {setter.attribute_name};");
                sourceCodeBuilder.AppendLine($"\t}}");
            }
            else if (setter.data_type == "inst_ref_<timer>")
            {
                sourceCodeBuilder.AppendLine($"\tpublic void set{setter.attribute_name}(TIMER {setter.attribute_name}) {{");
                sourceCodeBuilder.AppendLine($"\t\tthis.{setter.attribute_name} = {setter.attribute_name};");
                sourceCodeBuilder.AppendLine($"\t}}");
            }
            else if (setter.data_type == "inst_ref")
            {
                //string dataType = MapDataType(setter.data_type);
                sourceCodeBuilder.AppendLine($"\tpublic void set{setter.attribute_name}Ref({setter.related_class_name} {setter.attribute_name}Ref) {{");
                sourceCodeBuilder.AppendLine($"\t\tthis.{setter.attribute_name}Ref = {setter.attribute_name}Ref;");
                sourceCodeBuilder.AppendLine($"\t}}");
            }
            else if (setter.data_type == "inst_ref_set")
            {
                sourceCodeBuilder.AppendLine($"\tpublic void set{setter.attribute_name}RefSet({setter.related_class_name} {setter.attribute_name}) {{");
                sourceCodeBuilder.AppendLine($"\t\tthis.{setter.attribute_name}RefSet[] = {setter.attribute_name};");
                sourceCodeBuilder.AppendLine($"\t}}");
            }
            else if (setter.data_type == "inst_event")
            {
                return;
            }

        }
        private void GenerateGetState(JsonData.Attribute1 getstate)
        {
            if (getstate.data_type == "state")
            {
                sourceCodeBuilder.AppendLine($"\tpublic string GetState() {{");  // bagian ini nya belom
                sourceCodeBuilder.AppendLine($"\t\tthis.state;");
                sourceCodeBuilder.AppendLine($"\t}}\n");
            }
        }

        private void GenerateStateTransitionMethods(List<JsonData.State> states, string status)
        {
            foreach (var state in states)
            {
                if (state.state_event != null)
                {
                    for (int i = 0; i < state.state_event.Length; i++)
                    {
                        string methodName = $"{state.state_event[i]}";

                        sourceCodeBuilder.AppendLine($"\tpublic void {methodName}() {{");
                        sourceCodeBuilder.AppendLine($"\t\tthis.{status} = '{state.state_value}';");
                        sourceCodeBuilder.AppendLine($"\t}}\n");
                    }
                }
            }
        }

        private void Transition(List<JsonData.State> state, string status)
        {
            if (state.All(s => s.transitions == null || !s.transitions.Any()))
            {
                return;
            }


            sourceCodeBuilder.AppendLine($"\tpublic void Transition() {{");
            sourceCodeBuilder.AppendLine($"\t\tswitch (this.{status}) {{");
            foreach (var states in state)
            {
                sourceCodeBuilder.AppendLine($"\t\t\tcase '{states.state_name}':");
                sourceCodeBuilder.AppendLine($"\t\t\t\t{states.action}");

                if (states.transitions != null)
                {
                    foreach (var transition in states.transitions)
                    {
                        string targetState = null;

                        foreach (var statess in state)
                        {
                            if (statess.state_id == transition.target_state_id)
                            {
                                targetState = statess.state_event[0];
                            }
                        }
                        sourceCodeBuilder.AppendLine($"\t\t\t\tif (this.{status} == '{transition.target_state}') {{");
                        sourceCodeBuilder.AppendLine($"\t\t\t\t\t{targetState}();");
                        sourceCodeBuilder.AppendLine($"\t\t\t\t}}");
                    }
                }
                sourceCodeBuilder.AppendLine($"\t\t\t\tbreak;");
            }
            sourceCodeBuilder.AppendLine($"\t\t}}");
            sourceCodeBuilder.AppendLine($"\t}}");
            sourceCodeBuilder.Append($"\n");
        }

        private string StateStatus(JsonData.Attribute1 attributes)
        {
            if (attributes.data_type == "state")
            {
                status = attributes.attribute_name;
            }

            return status;
        }

        private string Target(JsonData.Transition target)
        {
            string targetState = target.target_state;
            return targetState;
        }

        //private void GenerateStateTransitionMethod(List<JsonData.State> states)  // ini juga belomm
        //{
        //    //if (state.state_event != null && state.state_event.Length > 0)
        //    foreach (var state in states)
        //    {
        //        if (state.state_event != null)
        //        {
        //            foreach (var eventName in state.state_event)
        //            {
        //                string methodName = $"{Char.ToUpper(eventName[0])}{eventName.Substring(1)}";

        //                sourceCodeBuilder.AppendLine($"     public void {methodName}() {{");

        //                if (state.transitions != null)
        //                {
        //                    foreach (var transition in state.transitions)
        //                    {
        //                        if (transition != null)
        //                        {
        //                            string targetStateId = transition.target_state_id;
        //                            string targetState = transition.target_state;

        //                            if (!string.IsNullOrEmpty(targetStateId))
        //                            {
        //                                sourceCodeBuilder.AppendLine($"       this.SetStateById({targetStateId});");
        //                            }
        //                        }
        //                    }
        //                }

        //                sourceCodeBuilder.AppendLine($"       this.{state.state_name} = \"{state.state_value}\";");
        //                sourceCodeBuilder.AppendLine($"     }}\n");
        //            }
        //        }

        //string setEvent = state.state_event[0];
        //string onEvent = state.state_event[1];
        //sourceCodeBuilder.AppendLine($"     public void {setEvent}() {{");
        //sourceCodeBuilder.AppendLine($"       this.state = \"{state.state_value}\";");
        //sourceCodeBuilder.AppendLine($"}}\n");

        //sourceCodeBuilder.AppendLine($"     public void {onEvent}() {{");
        //sourceCodeBuilder.AppendLine($"       System.out.println \"status saat ini {state.state_value}\";");
        //sourceCodeBuilder.AppendLine($"}}");
        //}


        //if (state.state_function != null && state.state_function.Length > 0)  // ini juga belomm
        //{
        //    string setFunction = state.state_function[0];
        //    sourceCodeBuilder.AppendLine($"     public void {setFunction}() {{");
        //    sourceCodeBuilder.AppendLine($"       this.state = \"{state.state_value}\";");
        //    sourceCodeBuilder.AppendLine($"}}\n");
        //}
        //}

        //private void GenerateAssocClass()
        //{
        //    sourceCodeBuilder.AppendLine($" public class Association{{");
        //    sourceCodeBuilder.AppendLine($"     public void ((data_type) class1, (data_type) class2) {{");
        //    sourceCodeBuilder.AppendLine($"}}");
        //    sourceCodeBuilder.AppendLine($"}}");
        //    sourceCodeBuilder.AppendLine($"\n");
        //    sourceCodeBuilder.AppendLine($" public class Main{{");
        //    sourceCodeBuilder.AppendLine($"     public static void main(String[] args){{");
        //}
        //private void GenerateObjAssociation(JsonData.Model assoc)
        //{

        //    sourceCodeBuilder.Append($"         Association {assoc.name} = new Association(");

        //    foreach (var association in assoc.@class)
        //    {
        //        sourceCodeBuilder.Append($"\"{association.class_name}\",");
        //    }

        //    sourceCodeBuilder.Length -= 1; // Remove the last character (",")

        //    sourceCodeBuilder.AppendLine($");");
        //}

        //private void GenerateAssoc()
        //{
        //    sourceCodeBuilder.AppendLine($"     }}");
        //    sourceCodeBuilder.AppendLine($"}}");
        //}

        private string MapDataType(string dataType)
        {
            switch (dataType.ToLower())
            {
                case "integer":
                    return "int";
                case "id":
                    return "String (alphanumeric)"; // ini diganti
                case "string":
                    return "String";
                case "bool":
                    return "boolean";
                case "real":
                    return "float";
                case "inst_ref_<timer>":
                    return "TIMER";
                // Add more mappings as needed
                default:
                    return dataType; // For unknown types, just pass through
            }
        }


        private void buttonReset_Click_1(object sender, EventArgs e)
        {
            richTextBox2.Clear();
            msgBox.Clear();
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            string helpMessage = OpenHelp(); // tulis isi help di sini
            MessageBox.Show(helpMessage, "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private string OpenHelp()
        {
            StringBuilder helpMessage = new StringBuilder();

            helpMessage.AppendLine("User Guide for Generating JSON Model into Java Programming Language");
            helpMessage.AppendLine();
            helpMessage.AppendLine("1. Open the desired JSON File by clicking Browse button");
            helpMessage.AppendLine();
            helpMessage.AppendLine("2. Parse the file first by Clicking the Parse button");
            helpMessage.AppendLine();
            helpMessage.AppendLine("3. After the file successfully parsed, click the Generate button to translate it to Java");
            helpMessage.AppendLine();
            helpMessage.AppendLine("4. The Java Code will exist in the right box and click Export button to save it as Java file");
            helpMessage.AppendLine();

            return helpMessage.ToString();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (richTextBox2.TextLength > 0)
            {
                richTextBox2.SelectAll();
                richTextBox2.Copy();
                MessageBox.Show("Successfully Copied!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                MessageBox.Show("Please Translate First!", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!isJsonFileSelected)
            {
                MessageBox.Show("Select JSON file as an input first!", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (richTextBox2.TextLength > 0)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Java files (*.java)|*.java|All files (*.*)|*.*"; // ubah ekstensi output save file
                saveFileDialog.Title = "Save Java File"; // ubah Java

                DialogResult result = saveFileDialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    string javaCode = richTextBox2.Text;

                    File.WriteAllText(saveFileDialog.FileName, javaCode);

                    selectedFilePath = saveFileDialog.FileName;

                    MessageBox.Show("Successfully Saved!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Please Translate First!", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        private void btnVisualize_Click(object sender, EventArgs e)
        {
            MessageBox.Show("We are sorry, this feature is not available right now.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        private void btnSimulate_Click(object sender, EventArgs e)
        {
            MessageBox.Show("We are sorry, this feature is not available right now.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }


    }
}
