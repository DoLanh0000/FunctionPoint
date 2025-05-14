using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Data.SqlClient;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Data;
using System.Web.Services;
using System.Web.Script.Services;

namespace FunctionPointCalculator
{
    public partial class Defaults : Page
    {
        private Dictionary<string, object> aiResult;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                gvHistory.DataKeyNames = new string[] { "Id" };
            }
        }

        protected void BtnCalculate_Click(object sender, EventArgs e)
        {
            try
            {
                int eiLow = string.IsNullOrEmpty(txtEiLow.Text) ? 0 : int.Parse(txtEiLow.Text);
                int eiAvg = string.IsNullOrEmpty(txtEiAvg.Text) ? 0 : int.Parse(txtEiAvg.Text);
                int eiHigh = string.IsNullOrEmpty(txtEiHigh.Text) ? 0 : int.Parse(txtEiHigh.Text);

                int eoLow = string.IsNullOrEmpty(txtEoLow.Text) ? 0 : int.Parse(txtEoLow.Text);
                int eoAvg = string.IsNullOrEmpty(txtEoAvg.Text) ? 0 : int.Parse(txtEoAvg.Text);
                int eoHigh = string.IsNullOrEmpty(txtEoHigh.Text) ? 0 : int.Parse(txtEoHigh.Text);

                int eqLow = string.IsNullOrEmpty(txtEqLow.Text) ? 0 : int.Parse(txtEqLow.Text);
                int eqAvg = string.IsNullOrEmpty(txtEqAvg.Text) ? 0 : int.Parse(txtEqAvg.Text);
                int eqHigh = string.IsNullOrEmpty(txtEqHigh.Text) ? 0 : int.Parse(txtEqHigh.Text);

                int ilfLow = string.IsNullOrEmpty(txtIlfLow.Text) ? 0 : int.Parse(txtIlfLow.Text);
                int ilfAvg = string.IsNullOrEmpty(txtIlfAvg.Text) ? 0 : int.Parse(txtIlfAvg.Text);
                int ilfHigh = string.IsNullOrEmpty(txtIlfHigh.Text) ? 0 : int.Parse(txtIlfHigh.Text);

                int eifLow = string.IsNullOrEmpty(txtEifLow.Text) ? 0 : int.Parse(txtEifLow.Text);
                int eifAvg = string.IsNullOrEmpty(txtEifAvg.Text) ? 0 : int.Parse(txtEifAvg.Text);
                int eifHigh = string.IsNullOrEmpty(txtEifHigh.Text) ? 0 : int.Parse(txtEifHigh.Text);

                double ufp = (eiLow * 3 + eiAvg * 4 + eiHigh * 6) +
                             (eoLow * 4 + eoAvg * 5 + eoHigh * 7) +
                             (eqLow * 3 + eqAvg * 4 + eqHigh * 6) +
                             (ilfLow * 7 + ilfAvg * 10 + ilfHigh * 15) +
                             (eifLow * 5 + eifAvg * 7 + eifHigh * 10);

                int f1 = string.IsNullOrEmpty(txtF1.Text) ? 0 : int.Parse(txtF1.Text);
                int f2 = string.IsNullOrEmpty(txtF2.Text) ? 0 : int.Parse(txtF2.Text);
                int f3 = string.IsNullOrEmpty(txtF3.Text) ? 0 : int.Parse(txtF3.Text);
                int f4 = string.IsNullOrEmpty(txtF4.Text) ? 0 : int.Parse(txtF4.Text);
                int f5 = string.IsNullOrEmpty(txtF5.Text) ? 0 : int.Parse(txtF5.Text);
                int f6 = string.IsNullOrEmpty(txtF6.Text) ? 0 : int.Parse(txtF6.Text);
                int f7 = string.IsNullOrEmpty(txtF7.Text) ? 0 : int.Parse(txtF7.Text);
                int f8 = string.IsNullOrEmpty(txtF8.Text) ? 0 : int.Parse(txtF8.Text);
                int f9 = string.IsNullOrEmpty(txtF9.Text) ? 0 : int.Parse(txtF9.Text);
                int f10 = string.IsNullOrEmpty(txtF10.Text) ? 0 : int.Parse(txtF10.Text);
                int f11 = string.IsNullOrEmpty(txtF11.Text) ? 0 : int.Parse(txtF11.Text);
                int f12 = string.IsNullOrEmpty(txtF12.Text) ? 0 : int.Parse(txtF12.Text);
                int f13 = string.IsNullOrEmpty(txtF13.Text) ? 0 : int.Parse(txtF13.Text);
                int f14 = string.IsNullOrEmpty(txtF14.Text) ? 0 : int.Parse(txtF14.Text);

                double vaf = 0.65 + 0.01 * (f1 + f2 + f3 + f4 + f5 + f6 + f7 + f8 + f9 + f10 + f11 + f12 + f13 + f14);

                double fp = ufp * vaf;

                lblUfp.Text = $"Unadjusted Function Point (UFP): {ufp:F2}";
                lblVaf.Text = $"Value Adjustment Factor (VAF): {vaf:F2}";
                lblFp.Text = $"Function Point (FP): {fp:F2}";

                SaveToDatabase(ufp, vaf, fp, fileSrsUpload.HasFile ? fileSrsUpload.FileName : "", aiResult != null ? JsonConvert.SerializeObject(aiResult) : "",
                    eiLow, eiAvg, eiHigh, eoLow, eoAvg, eoHigh, eqLow, eqAvg, eqHigh, ilfLow, ilfAvg, ilfHigh, eifLow, eifAvg, eifHigh,
                    f1, f2, f3, f4, f5, f6, f7, f8, f9, f10, f11, f12, f13, f14);

                ScriptManager.RegisterStartupScript(this, GetType(), "success", "alert('Calculation completed and saved successfully!');", true);
            }
            catch (Exception ex)
            {
                lblFp.Text = $"Error: {ex.Message}";
                ScriptManager.RegisterStartupScript(this, GetType(), "error", $"alert('Error: {ex.Message}');", true);
            }
        }

        private void SaveToDatabase(double ufp, double vaf, double fp, string srsFileName = "", string srsAnalysisResult = "",
            int eiLow = 0, int eiAvg = 0, int eiHigh = 0, int eoLow = 0, int eoAvg = 0, int eoHigh = 0,
            int eqLow = 0, int eqAvg = 0, int eqHigh = 0, int ilfLow = 0, int ilfAvg = 0, int ilfHigh = 0,
            int eifLow = 0, int eifAvg = 0, int eifHigh = 0,
            int f1 = 0, int f2 = 0, int f3 = 0, int f4 = 0, int f5 = 0, int f6 = 0, int f7 = 0,
            int f8 = 0, int f9 = 0, int f10 = 0, int f11 = 0, int f12 = 0, int f13 = 0, int f14 = 0)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["FunctionPointDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO FunctionPointResults (UFP, VAF, FP, CalculationDate, SrsFileName, SrsAnalysisResult,
                    EiLow, EiAvg, EiHigh, EoLow, EoAvg, EoHigh, EqLow, EqAvg, EqHigh, IlfLow, IlfAvg, IlfHigh, EifLow, EifAvg, EifHigh,
                    F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12, F13, F14)
                    VALUES (@UFP, @VAF, @FP, @Date, @SrsFileName, @SrsAnalysisResult,
                    @EiLow, @EiAvg, @EiHigh, @EoLow, @EoAvg, @EoHigh, @EqLow, @EqAvg, @EqHigh, @IlfLow, @IlfAvg, @IlfHigh, @EifLow, @EifAvg, @EifHigh,
                    @F1, @F2, @F3, @F4, @F5, @F6, @F7, @F8, @F9, @F10, @F11, @F12, @F13, @F14)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UFP", ufp);
                    cmd.Parameters.AddWithValue("@VAF", vaf);
                    cmd.Parameters.AddWithValue("@FP", fp);
                    cmd.Parameters.AddWithValue("@Date", DateTime.Now);
                    cmd.Parameters.AddWithValue("@SrsFileName", srsFileName);
                    cmd.Parameters.AddWithValue("@SrsAnalysisResult", srsAnalysisResult);
                    cmd.Parameters.AddWithValue("@EiLow", eiLow);
                    cmd.Parameters.AddWithValue("@EiAvg", eiAvg);
                    cmd.Parameters.AddWithValue("@EiHigh", eiHigh);
                    cmd.Parameters.AddWithValue("@EoLow", eoLow);
                    cmd.Parameters.AddWithValue("@EoAvg", eoAvg);
                    cmd.Parameters.AddWithValue("@EoHigh", eoHigh);
                    cmd.Parameters.AddWithValue("@EqLow", eqLow);
                    cmd.Parameters.AddWithValue("@EqAvg", eqAvg);
                    cmd.Parameters.AddWithValue("@EqHigh", eqHigh);
                    cmd.Parameters.AddWithValue("@IlfLow", ilfLow);
                    cmd.Parameters.AddWithValue("@IlfAvg", ilfAvg);
                    cmd.Parameters.AddWithValue("@IlfHigh", ilfHigh);
                    cmd.Parameters.AddWithValue("@EifLow", eifLow);
                    cmd.Parameters.AddWithValue("@EifAvg", eifAvg);
                    cmd.Parameters.AddWithValue("@EifHigh", eifHigh);
                    cmd.Parameters.AddWithValue("@F1", f1);
                    cmd.Parameters.AddWithValue("@F2", f2);
                    cmd.Parameters.AddWithValue("@F3", f3);
                    cmd.Parameters.AddWithValue("@F4", f4);
                    cmd.Parameters.AddWithValue("@F5", f5);
                    cmd.Parameters.AddWithValue("@F6", f6);
                    cmd.Parameters.AddWithValue("@F7", f7);
                    cmd.Parameters.AddWithValue("@F8", f8);
                    cmd.Parameters.AddWithValue("@F9", f9);
                    cmd.Parameters.AddWithValue("@F10", f10);
                    cmd.Parameters.AddWithValue("@F11", f11);
                    cmd.Parameters.AddWithValue("@F12", f12);
                    cmd.Parameters.AddWithValue("@F13", f13);
                    cmd.Parameters.AddWithValue("@F14", f14);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        protected void BtnExport_Click(object sender, EventArgs e)
        {
            Document document = new Document();
            MemoryStream memoryStream = new MemoryStream();
            PdfWriter.GetInstance(document, memoryStream);
            document.Open();

            document.Add(new Paragraph("Function Point Report", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16)));
            document.Add(new Paragraph($"Date: {DateTime.Now}", FontFactory.GetFont(FontFactory.HELVETICA, 12)));
            document.Add(new Paragraph($"SRS File: {(fileSrsUpload.HasFile ? fileSrsUpload.FileName : "N/A")}", FontFactory.GetFont(FontFactory.HELVETICA, 12)));
            document.Add(new Paragraph(" "));
            document.Add(new Paragraph(lblUfp.Text, FontFactory.GetFont(FontFactory.HELVETICA, 12)));
            document.Add(new Paragraph(lblVaf.Text, FontFactory.GetFont(FontFactory.HELVETICA, 12)));
            document.Add(new Paragraph(lblFp.Text, FontFactory.GetFont(FontFactory.HELVETICA, 12)));
            document.Close();

            Response.ContentType = "application/pdf";
            Response.AppendHeader("Content-Disposition", "attachment; filename=FP_Report.pdf");
            Response.BinaryWrite(memoryStream.ToArray());
            Response.End();
        }

        protected void BtnReset_Click(object sender, EventArgs e)
        {
            txtEiLow.Text = "";
            txtEiAvg.Text = "";
            txtEiHigh.Text = "";
            txtEoLow.Text = "";
            txtEoAvg.Text = "";
            txtEoHigh.Text = "";
            txtEqLow.Text = "";
            txtEqAvg.Text = "";
            txtEqHigh.Text = "";
            txtIlfLow.Text = "";
            txtIlfAvg.Text = "";
            txtIlfHigh.Text = "";
            txtEifLow.Text = "";
            txtEifAvg.Text = "";
            txtEifHigh.Text = "";

            txtF1.Text = "";
            txtF2.Text = "";
            txtF3.Text = "";
            txtF4.Text = "";
            txtF5.Text = "";
            txtF6.Text = "";
            txtF7.Text = "";
            txtF8.Text = "";
            txtF9.Text = "";
            txtF10.Text = "";
            txtF11.Text = "";
            txtF12.Text = "";
            txtF13.Text = "";
            txtF14.Text = "";

            lblUfp.Text = "";
            lblVaf.Text = "";
            lblFp.Text = "";
            aiResult = null;
        }

        protected async void BtnAnalyzeSrs_Click(object sender, EventArgs e)
        {
            try
            {
                if (fileSrsUpload.HasFile)
                {
                    string fileExtension = System.IO.Path.GetExtension(fileSrsUpload.FileName).ToLower();
                    string srsText = "";
                    string imageBase64 = null;

                    if (fileExtension == ".pdf")
                    {
                        srsText = ExtractTextFromPdf(fileSrsUpload.PostedFile.InputStream);
                    }
                    else if (fileExtension == ".txt")
                    {
                        using (StreamReader reader = new StreamReader(fileSrsUpload.PostedFile.InputStream))
                        {
                            srsText = reader.ReadToEnd();
                        }
                    }
                    else if (fileExtension == ".jpg" || fileExtension == ".jpeg")
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            fileSrsUpload.PostedFile.InputStream.CopyTo(ms);
                            byte[] imageBytes = ms.ToArray();
                            imageBase64 = Convert.ToBase64String(imageBytes);
                        }
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('Please upload a PDF, TXT, or JPG file.');", true);
                        return;
                    }

                    aiResult = await AnalyzeSrsWithAI(srsText, imageBase64);
                    FillFormWithAiResults(aiResult);

                    ScriptManager.RegisterStartupScript(this, GetType(), "success", "alert('SRS analyzed successfully! Please review the results.');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "error", "alert('Please upload an SRS document.');", true);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error", $"alert('Error: {ex.Message}');", true);
            }
        }

        private string ExtractTextFromPdf(Stream pdfStream)
        {
            StringBuilder text = new StringBuilder();
            using (PdfReader reader = new PdfReader(pdfStream))
            {
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                }
            }
            return text.ToString();
        }

        private async Task<Dictionary<string, object>> AnalyzeSrsWithAI(string srsText, string imageBase64 = null)
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent";
                string apiKey = System.Configuration.ConfigurationManager.AppSettings["AiApiKey"];

                if (string.IsNullOrEmpty(apiKey))
                {
                    throw new Exception("AI API Key is missing in Web.config. Please configure the 'AiApiKey' in the appSettings section.");
                }

                apiUrl = $"{apiUrl}?key={apiKey}";

                string prompt = @"Analyze the following Software Requirements Specification (SRS) and extract Function Point components (External Inputs (EI), External Outputs (EO), External Inquiries (EQ), Internal Logical Files (ILF), External Interface Files (EIF)) with their complexity (Low, Average, High) and Value Adjustment Factors (F1-F14). Ensure the response is in valid JSON format as shown below. Do not include any additional text, explanations, or markdown outside the JSON.

Output format:
{
  ""EI"": { ""Low"": 0, ""Average"": 0, ""High"": 0 },
  ""EO"": { ""Low"": 0, ""Average"": 0, ""High"": 0 },
  ""EQ"": { ""Low"": 0, ""Average"": 0, ""High"": 0 },
  ""ILF"": { ""Low"": 0, ""Average"": 0, ""High"": 0 },
  ""EIF"": { ""Low"": 0, ""Average"": 0, ""High"": 0 },
  ""VAF"": { ""F1"": 0, ""F2"": 0, ""F3"": 0, ""F4"": 0, ""F5"": 0, ""F6"": 0, ""F7"": 0, ""F8"": 0, ""F9"": 0, ""F10"": 0, ""F11"": 0, ""F12"": 0, ""F13"": 0, ""F14"": 0 }
}";

                var parts = new List<object>();
                parts.Add(new { text = prompt });

                if (!string.IsNullOrEmpty(srsText))
                {
                    parts.Add(new { text = "SRS Text:\n" + srsText });
                }

                if (!string.IsNullOrEmpty(imageBase64))
                {
                    parts.Add(new
                    {
                        inlineData = new
                        {
                            mimeType = "image/jpeg",
                            data = imageBase64
                        }
                    });
                }

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = parts.ToArray()
                        }
                    }
                };

                var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResult = await response.Content.ReadAsStringAsync();
                    var responseObject = JsonConvert.DeserializeObject<dynamic>(jsonResult);
                    string aiResponseText = responseObject.candidates[0].content.parts[0].text;

                    aiResponseText = aiResponseText.Trim();
                    if (aiResponseText.StartsWith("```json"))
                    {
                        aiResponseText = aiResponseText.Substring(7);
                        aiResponseText = aiResponseText.Substring(0, aiResponseText.LastIndexOf("```"));
                        aiResponseText = aiResponseText.Trim();
                    }

                    return JsonConvert.DeserializeObject<Dictionary<string, object>>(aiResponseText);
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    throw new Exception($"AI API request failed: {response.ReasonPhrase}. Details: {errorMessage}");
                }
            }
        }

        private void FillFormWithAiResults(Dictionary<string, object> aiResult)
        {
            try
            {
                var ei = JsonConvert.DeserializeObject<Dictionary<string, int>>(aiResult["EI"].ToString());
                var eo = JsonConvert.DeserializeObject<Dictionary<string, int>>(aiResult["EO"].ToString());
                var eq = JsonConvert.DeserializeObject<Dictionary<string, int>>(aiResult["EQ"].ToString());
                var ilf = JsonConvert.DeserializeObject<Dictionary<string, int>>(aiResult["ILF"].ToString());
                var eif = JsonConvert.DeserializeObject<Dictionary<string, int>>(aiResult["EIF"].ToString());
                var vaf = JsonConvert.DeserializeObject<Dictionary<string, int>>(aiResult["VAF"].ToString());

                txtEiLow.Text = ei.ContainsKey("Low") ? ei["Low"].ToString() : "0";
                txtEiAvg.Text = ei.ContainsKey("Average") ? ei["Average"].ToString() : "0";
                txtEiHigh.Text = ei.ContainsKey("High") ? ei["High"].ToString() : "0";

                txtEoLow.Text = eo.ContainsKey("Low") ? eo["Low"].ToString() : "0";
                txtEoAvg.Text = eo.ContainsKey("Average") ? eo["Average"].ToString() : "0";
                txtEoHigh.Text = eo.ContainsKey("High") ? eo["High"].ToString() : "0";

                txtEqLow.Text = eq.ContainsKey("Low") ? eq["Low"].ToString() : "0";
                txtEqAvg.Text = eq.ContainsKey("Average") ? eq["Average"].ToString() : "0";
                txtEqHigh.Text = eq.ContainsKey("High") ? eq["High"].ToString() : "0";

                txtIlfLow.Text = ilf.ContainsKey("Low") ? ilf["Low"].ToString() : "0";
                txtIlfAvg.Text = ilf.ContainsKey("Average") ? ilf["Average"].ToString() : "0";
                txtIlfHigh.Text = ilf.ContainsKey("High") ? ilf["High"].ToString() : "0";

                txtEifLow.Text = eif.ContainsKey("Low") ? eif["Low"].ToString() : "0";
                txtEifAvg.Text = eif.ContainsKey("Average") ? eif["Average"].ToString() : "0";
                txtEifHigh.Text = eif.ContainsKey("High") ? eif["High"].ToString() : "0";

                txtF1.Text = vaf.ContainsKey("F1") ? vaf["F1"].ToString() : "0";
                txtF2.Text = vaf.ContainsKey("F2") ? vaf["F2"].ToString() : "0";
                txtF3.Text = vaf.ContainsKey("F3") ? vaf["F3"].ToString() : "0";
                txtF4.Text = vaf.ContainsKey("F4") ? vaf["F4"].ToString() : "0";
                txtF5.Text = vaf.ContainsKey("F5") ? vaf["F5"].ToString() : "0";
                txtF6.Text = vaf.ContainsKey("F6") ? vaf["F6"].ToString() : "0";
                txtF7.Text = vaf.ContainsKey("F7") ? vaf["F7"].ToString() : "0";
                txtF8.Text = vaf.ContainsKey("F8") ? vaf["F8"].ToString() : "0";
                txtF9.Text = vaf.ContainsKey("F9") ? vaf["F9"].ToString() : "0";
                txtF10.Text = vaf.ContainsKey("F10") ? vaf["F10"].ToString() : "0";
                txtF11.Text = vaf.ContainsKey("F11") ? vaf["F11"].ToString() : "0";
                txtF12.Text = vaf.ContainsKey("F12") ? vaf["F12"].ToString() : "0";
                txtF13.Text = vaf.ContainsKey("F13") ? vaf["F13"].ToString() : "0";
                txtF14.Text = vaf.ContainsKey("F14") ? vaf["F14"].ToString() : "0";
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to parse AI response: " + ex.Message);
            }
        }

        protected void BtnViewHistory_Click(object sender, EventArgs e)
        {
            LoadCalculationHistory();
            gvHistory.Visible = true;

            // Load chart data and pass it to client
            string chartData = GetChartDataInternal();
            ScriptManager.RegisterStartupScript(this, GetType(), "chartData", $"window.chartData = {chartData};", true);
        }

        private void LoadCalculationHistory()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["FunctionPointDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT Id, UFP, VAF, FP, CalculationDate, SrsFileName FROM FunctionPointResults ORDER BY CalculationDate DESC";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        gvHistory.DataSource = dt;
                        gvHistory.DataBind();
                    }
                    conn.Close();
                }
            }
        }

        private string GetChartDataInternal()
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["FunctionPointDB"].ConnectionString;
                List<Dictionary<string, object>> results = new List<Dictionary<string, object>>();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT Id, FP, CalculationDate FROM FunctionPointResults ORDER BY CalculationDate ASC";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                results.Add(new Dictionary<string, object>
                                {
                                    { "Id", reader["Id"] },
                                    { "FP", Convert.ToDouble(reader["FP"]) },
                                    { "CalculationDate", reader["CalculationDate"].ToString() }
                                });
                            }
                        }
                        conn.Close();
                    }
                }

                return JsonConvert.SerializeObject(results);
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { error = ex.Message });
            }
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string GetChartData()
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["FunctionPointDB"].ConnectionString;
                List<Dictionary<string, object>> results = new List<Dictionary<string, object>>();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT Id, FP, CalculationDate FROM FunctionPointResults ORDER BY CalculationDate ASC";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                results.Add(new Dictionary<string, object>
                                {
                                    { "Id", reader["Id"] },
                                    { "FP", Convert.ToDouble(reader["FP"]) },
                                    { "CalculationDate", reader["CalculationDate"].ToString() }
                                });
                            }
                        }
                        conn.Close();
                    }
                }

                return JsonConvert.SerializeObject(results);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.StatusCode = 500;
                return JsonConvert.SerializeObject(new { error = ex.Message });
            }
        }

        protected void gvHistory_RowEditing(object sender, GridViewEditEventArgs e)
        {
            int id = Convert.ToInt32(gvHistory.DataKeys[e.NewEditIndex].Value);
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["FunctionPointDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT EiLow, EiAvg, EiHigh, EoLow, EoAvg, EoHigh, EqLow, EqAvg, EqHigh, 
                                IlfLow, IlfAvg, IlfHigh, EifLow, EifAvg, EifHigh, 
                                F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12, F13, F14 
                                FROM FunctionPointResults WHERE Id = @Id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtEiLow.Text = reader["EiLow"] != DBNull.Value ? reader["EiLow"].ToString() : "0";
                            txtEiAvg.Text = reader["EiAvg"] != DBNull.Value ? reader["EiAvg"].ToString() : "0";
                            txtEiHigh.Text = reader["EiHigh"] != DBNull.Value ? reader["EiHigh"].ToString() : "0";

                            txtEoLow.Text = reader["EoLow"] != DBNull.Value ? reader["EoLow"].ToString() : "0";
                            txtEoAvg.Text = reader["EoAvg"] != DBNull.Value ? reader["EoAvg"].ToString() : "0";
                            txtEoHigh.Text = reader["EoHigh"] != DBNull.Value ? reader["EoHigh"].ToString() : "0";

                            txtEqLow.Text = reader["EqLow"] != DBNull.Value ? reader["EqLow"].ToString() : "0";
                            txtEqAvg.Text = reader["EqAvg"] != DBNull.Value ? reader["EqAvg"].ToString() : "0";
                            txtEqHigh.Text = reader["EqHigh"] != DBNull.Value ? reader["EqHigh"].ToString() : "0";

                            txtIlfLow.Text = reader["IlfLow"] != DBNull.Value ? reader["IlfLow"].ToString() : "0";
                            txtIlfAvg.Text = reader["IlfAvg"] != DBNull.Value ? reader["IlfAvg"].ToString() : "0";
                            txtIlfHigh.Text = reader["IlfHigh"] != DBNull.Value ? reader["IlfHigh"].ToString() : "0";

                            txtEifLow.Text = reader["EifLow"] != DBNull.Value ? reader["EifLow"].ToString() : "0";
                            txtEifAvg.Text = reader["EifAvg"] != DBNull.Value ? reader["EifAvg"].ToString() : "0";
                            txtEifHigh.Text = reader["EifHigh"] != DBNull.Value ? reader["EifHigh"].ToString() : "0";

                            txtF1.Text = reader["F1"] != DBNull.Value ? reader["F1"].ToString() : "0";
                            txtF2.Text = reader["F2"] != DBNull.Value ? reader["F2"].ToString() : "0";
                            txtF3.Text = reader["F3"] != DBNull.Value ? reader["F3"].ToString() : "0";
                            txtF4.Text = reader["F4"] != DBNull.Value ? reader["F4"].ToString() : "0";
                            txtF5.Text = reader["F5"] != DBNull.Value ? reader["F5"].ToString() : "0";
                            txtF6.Text = reader["F6"] != DBNull.Value ? reader["F6"].ToString() : "0";
                            txtF7.Text = reader["F7"] != DBNull.Value ? reader["F7"].ToString() : "0";
                            txtF8.Text = reader["F8"] != DBNull.Value ? reader["F8"].ToString() : "0";
                            txtF9.Text = reader["F9"] != DBNull.Value ? reader["F9"].ToString() : "0";
                            txtF10.Text = reader["F10"] != DBNull.Value ? reader["F10"].ToString() : "0";
                            txtF11.Text = reader["F11"] != DBNull.Value ? reader["F11"].ToString() : "0";
                            txtF12.Text = reader["F12"] != DBNull.Value ? reader["F12"].ToString() : "0";
                            txtF13.Text = reader["F13"] != DBNull.Value ? reader["F13"].ToString() : "0";
                            txtF14.Text = reader["F14"] != DBNull.Value ? reader["F14"].ToString() : "0";
                        }
                    }
                    conn.Close();
                }
            }

            gvHistory.EditIndex = e.NewEditIndex;
            LoadCalculationHistory();
            ScriptManager.RegisterStartupScript(this, GetType(), "info", "alert('Please edit the values in the UFP and VAF tables, then click Calculate FP to update the record.');", true);
        }

        protected void gvHistory_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvHistory.EditIndex = -1;
            LoadCalculationHistory();
        }

        protected void gvHistory_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int id = Convert.ToInt32(gvHistory.DataKeys[e.RowIndex].Value);
            string srsFileName = (gvHistory.Rows[e.RowIndex].Cells[5].Controls[0] as TextBox).Text;

            try
            {
                int eiLow = string.IsNullOrEmpty(txtEiLow.Text) ? 0 : int.Parse(txtEiLow.Text);
                int eiAvg = string.IsNullOrEmpty(txtEiAvg.Text) ? 0 : int.Parse(txtEiAvg.Text);
                int eiHigh = string.IsNullOrEmpty(txtEiHigh.Text) ? 0 : int.Parse(txtEiHigh.Text);

                int eoLow = string.IsNullOrEmpty(txtEoLow.Text) ? 0 : int.Parse(txtEoLow.Text);
                int eoAvg = string.IsNullOrEmpty(txtEoAvg.Text) ? 0 : int.Parse(txtEoAvg.Text);
                int eoHigh = string.IsNullOrEmpty(txtEoHigh.Text) ? 0 : int.Parse(txtEoHigh.Text);

                int eqLow = string.IsNullOrEmpty(txtEqLow.Text) ? 0 : int.Parse(txtEqLow.Text);
                int eqAvg = string.IsNullOrEmpty(txtEqAvg.Text) ? 0 : int.Parse(txtEqAvg.Text);
                int eqHigh = string.IsNullOrEmpty(txtEqHigh.Text) ? 0 : int.Parse(txtEqHigh.Text);

                int ilfLow = string.IsNullOrEmpty(txtIlfLow.Text) ? 0 : int.Parse(txtIlfLow.Text);
                int ilfAvg = string.IsNullOrEmpty(txtIlfAvg.Text) ? 0 : int.Parse(txtIlfAvg.Text);
                int ilfHigh = string.IsNullOrEmpty(txtIlfHigh.Text) ? 0 : int.Parse(txtIlfHigh.Text);

                int eifLow = string.IsNullOrEmpty(txtEifLow.Text) ? 0 : int.Parse(txtEifLow.Text);
                int eifAvg = string.IsNullOrEmpty(txtEifAvg.Text) ? 0 : int.Parse(txtEifAvg.Text);
                int eifHigh = string.IsNullOrEmpty(txtEifHigh.Text) ? 0 : int.Parse(txtEifHigh.Text);

                double ufp = (eiLow * 3 + eiAvg * 4 + eiHigh * 6) +
                             (eoLow * 4 + eoAvg * 5 + eoHigh * 7) +
                             (eqLow * 3 + eqAvg * 4 + eqHigh * 6) +
                             (ilfLow * 7 + ilfAvg * 10 + ilfHigh * 15) +
                             (eifLow * 5 + eifAvg * 7 + eifHigh * 10);

                int f1 = string.IsNullOrEmpty(txtF1.Text) ? 0 : int.Parse(txtF1.Text);
                int f2 = string.IsNullOrEmpty(txtF2.Text) ? 0 : int.Parse(txtF2.Text);
                int f3 = string.IsNullOrEmpty(txtF3.Text) ? 0 : int.Parse(txtF3.Text);
                int f4 = string.IsNullOrEmpty(txtF4.Text) ? 0 : int.Parse(txtF4.Text);
                int f5 = string.IsNullOrEmpty(txtF5.Text) ? 0 : int.Parse(txtF5.Text);
                int f6 = string.IsNullOrEmpty(txtF6.Text) ? 0 : int.Parse(txtF6.Text);
                int f7 = string.IsNullOrEmpty(txtF7.Text) ? 0 : int.Parse(txtF7.Text);
                int f8 = string.IsNullOrEmpty(txtF8.Text) ? 0 : int.Parse(txtF8.Text);
                int f9 = string.IsNullOrEmpty(txtF9.Text) ? 0 : int.Parse(txtF9.Text);
                int f10 = string.IsNullOrEmpty(txtF10.Text) ? 0 : int.Parse(txtF10.Text);
                int f11 = string.IsNullOrEmpty(txtF11.Text) ? 0 : int.Parse(txtF11.Text);
                int f12 = string.IsNullOrEmpty(txtF12.Text) ? 0 : int.Parse(txtF12.Text);
                int f13 = string.IsNullOrEmpty(txtF13.Text) ? 0 : int.Parse(txtF13.Text);
                int f14 = string.IsNullOrEmpty(txtF14.Text) ? 0 : int.Parse(txtF14.Text);

                double vaf = 0.65 + 0.01 * (f1 + f2 + f3 + f4 + f5 + f6 + f7 + f8 + f9 + f10 + f11 + f12 + f13 + f14);

                double fp = ufp * vaf;

                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["FunctionPointDB"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"UPDATE FunctionPointResults 
                                     SET UFP = @UFP, VAF = @VAF, FP = @FP, SrsFileName = @SrsFileName,
                                         EiLow = @EiLow, EiAvg = @EiAvg, EiHigh = @EiHigh,
                                         EoLow = @EoLow, EoAvg = @EoAvg, EoHigh = @EoHigh,
                                         EqLow = @EqLow, EqAvg = @EqAvg, EqHigh = @EqHigh,
                                         IlfLow = @IlfLow, IlfAvg = @IlfAvg, IlfHigh = @IlfHigh,
                                         EifLow = @EifLow, EifAvg = @EifAvg, EifHigh = @EifHigh,
                                         F1 = @F1, F2 = @F2, F3 = @F3, F4 = @F4, F5 = @F5,
                                         F6 = @F6, F7 = @F7, F8 = @F8, F9 = @F9, F10 = @F10,
                                         F11 = @F11, F12 = @F12, F13 = @F13, F14 = @F14
                                     WHERE Id = @Id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@UFP", ufp);
                        cmd.Parameters.AddWithValue("@VAF", vaf);
                        cmd.Parameters.AddWithValue("@FP", fp);
                        cmd.Parameters.AddWithValue("@SrsFileName", srsFileName);
                        cmd.Parameters.AddWithValue("@EiLow", eiLow);
                        cmd.Parameters.AddWithValue("@EiAvg", eiAvg);
                        cmd.Parameters.AddWithValue("@EiHigh", eiHigh);
                        cmd.Parameters.AddWithValue("@EoLow", eoLow);
                        cmd.Parameters.AddWithValue("@EoAvg", eoAvg);
                        cmd.Parameters.AddWithValue("@EoHigh", eoHigh);
                        cmd.Parameters.AddWithValue("@EqLow", eqLow);
                        cmd.Parameters.AddWithValue("@EqAvg", eqAvg);
                        cmd.Parameters.AddWithValue("@EqHigh", eqHigh);
                        cmd.Parameters.AddWithValue("@IlfLow", ilfLow);
                        cmd.Parameters.AddWithValue("@IlfAvg", ilfAvg);
                        cmd.Parameters.AddWithValue("@IlfHigh", ilfHigh);
                        cmd.Parameters.AddWithValue("@EifLow", eifLow);
                        cmd.Parameters.AddWithValue("@EifAvg", eifAvg);
                        cmd.Parameters.AddWithValue("@EifHigh", eifHigh);
                        cmd.Parameters.AddWithValue("@F1", f1);
                        cmd.Parameters.AddWithValue("@F2", f2);
                        cmd.Parameters.AddWithValue("@F3", f3);
                        cmd.Parameters.AddWithValue("@F4", f4);
                        cmd.Parameters.AddWithValue("@F5", f5);
                        cmd.Parameters.AddWithValue("@F6", f6);
                        cmd.Parameters.AddWithValue("@F7", f7);
                        cmd.Parameters.AddWithValue("@F8", f8);
                        cmd.Parameters.AddWithValue("@F9", f9);
                        cmd.Parameters.AddWithValue("@F10", f10);
                        cmd.Parameters.AddWithValue("@F11", f11);
                        cmd.Parameters.AddWithValue("@F12", f12);
                        cmd.Parameters.AddWithValue("@F13", f13);
                        cmd.Parameters.AddWithValue("@F14", f14);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }

                lblUfp.Text = $"Unadjusted Function Point (UFP): {ufp:F2}";
                lblVaf.Text = $"Value Adjustment Factor (VAF): {vaf:F2}";
                lblFp.Text = $"Function Point (FP): {fp:F2}";

                gvHistory.EditIndex = -1;
                LoadCalculationHistory();
                ScriptManager.RegisterStartupScript(this, GetType(), "success", "alert('Record updated successfully!');", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error", $"alert('Error updating record: {ex.Message}');", true);
            }
        }

        protected void gvHistory_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int id = Convert.ToInt32(gvHistory.DataKeys[e.RowIndex].Value);

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["FunctionPointDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM FunctionPointResults WHERE Id = @Id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }

            LoadCalculationHistory();
            ScriptManager.RegisterStartupScript(this, GetType(), "success", "alert('Record deleted successfully!');", true);
        }
    }
}