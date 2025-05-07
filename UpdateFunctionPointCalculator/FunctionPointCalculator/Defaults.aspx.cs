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

namespace FunctionPointCalculator
{
    public partial class Defaults : Page
    {
        private Dictionary<string, object> aiResult;

        protected void Page_Load(object sender, EventArgs e)
        {
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

                SaveToDatabase(ufp, vaf, fp, fileSrsUpload.HasFile ? fileSrsUpload.FileName : "", aiResult != null ? JsonConvert.SerializeObject(aiResult) : "");

                ScriptManager.RegisterStartupScript(this, GetType(), "success", "alert('Calculation completed and saved successfully!');", true);
            }
            catch (Exception ex)
            {
                lblFp.Text = $"Error: {ex.Message}";
                ScriptManager.RegisterStartupScript(this, GetType(), "error", $"alert('Error: {ex.Message}');", true);
            }
        }

        private void SaveToDatabase(double ufp, double vaf, double fp, string srsFileName = "", string srsAnalysisResult = "")
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["FunctionPointDB"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO FunctionPointResults (UFP, VAF, FP, CalculationDate, SrsFileName, SrsAnalysisResult) VALUES (@UFP, @VAF, @FP, @Date, @SrsFileName, @SrsAnalysisResult)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UFP", ufp);
                    cmd.Parameters.AddWithValue("@VAF", vaf);
                    cmd.Parameters.AddWithValue("@FP", fp);
                    cmd.Parameters.AddWithValue("@Date", DateTime.Now);
                    cmd.Parameters.AddWithValue("@SrsFileName", srsFileName);
                    cmd.Parameters.AddWithValue("@SrsAnalysisResult", srsAnalysisResult);

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
                        // Chuyển hình ảnh thành base64 để gửi đến Gemini API
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

                // Thêm API Key vào query string
                apiUrl = $"{apiUrl}?key={apiKey}";

                // Tạo prompt yêu cầu AI phân tích SRS
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

                // Tạo danh sách các phần (parts) cho yêu cầu
                var parts = new List<object>();

                // Thêm phần prompt vào parts
                parts.Add(new { text = prompt });

                // Nếu là file văn bản (PDF/TXT), thêm nội dung văn bản
                if (!string.IsNullOrEmpty(srsText))
                {
                    parts.Add(new { text = "SRS Text:\n" + srsText });
                }

                // Nếu là hình ảnh (JPG), thêm dữ liệu hình ảnh base64
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

                // Tạo request body theo định dạng của Gemini API
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

                // Gửi yêu cầu POST đến Gemini API
                var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResult = await response.Content.ReadAsStringAsync();

                    // Phân tích phản hồi từ Gemini API
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
    }
}