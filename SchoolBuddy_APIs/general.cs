using System.Net.Mail;
using System.Net;
using System.Text;
using OtpNet;
using SchoolBuddy_APIs.Models.Master.Students;
using SchoolBuddy_APIs.Models.App;
using OfficeOpenXml;
using System.Data;
using System.Net.Http.Headers;


namespace SchoolBuddy_APIs
{
    public class general
    {
        public  string SendEmail(string MailFrom, string MailTo,string parent_name) //sendMail.SendEmail("ticket@atlantasys.in", "tanyasuyalatl@gmail.com");
        {
            try
            {
                string otp = otpgenerator("JBSWY3DPEHPK3PXP");
                MailMessage mailmessage = new MailMessage(MailFrom, MailTo);
              

                string Subject = $"{parent_name},here's your OTP to verify your email address on SchoolBuddy";
               
                //string heading1 = "<div><h2 style=text-align:center;padding:10px>Confirm verification code</h2></div>";
                string body = @"<div >
                                <div>Confirm verification code</div>
                                <div >
                                <p>Hey "+parent_name+@",
                                Please enter the following code on 
                                the page where you asked for OTP on schoolbuddy:
                                </p>
                                <div style=""text-align:center;border:1px solid"">
                                <p style=""font-size:18px;""><b>"+ otp +@"</b>
                                </p>
                                </div>
                                </div>
                                </div>
                                <br/>
                                <div style=""text-align:center;"">
                                <p> Questions or help? Contact us at xyz@gmail.com
                                </p>
                                <p>
                                Atlanta India &copy; 2024 Schoolbuddy
                                </p>

                                </div>
                                ";
                mailmessage.Subject = Subject;
                mailmessage.Body = body;
                mailmessage.IsBodyHtml= true;
                SmtpClient smtpclient = new SmtpClient("smtp.gmail.com", 587);
                smtpclient.EnableSsl = true;

                NetworkCredential basicCredential1 = new
                NetworkCredential(MailFrom, "Atl@135#@");
                smtpclient.UseDefaultCredentials = false;
                smtpclient.Credentials = basicCredential1;
                try
                {
                    smtpclient.Send(mailmessage);
                }

                catch (Exception ex)
                {
                    throw ex;
                }





                return otp;
            }
            catch (Exception ex)
            {
                return "0";
            }

        }
        public bool generalsendemail(string MailFrom, string MailTo, string message,string contact) //sendMail.SendEmail("ticket@atlantasys.in", "tanyasuyalatl@gmail.com");
        {
            try
            {
                
                MailMessage mailmessage = new MailMessage(MailFrom, MailTo);

                string Subject = $"Query regarding Schoolbuddy";

                //string heading1 = "<div><h2 style=text-align:center;padding:10px>Confirm verification code</h2></div>";
                string body = $@"<div >
                                <div >
                                <p> {message}
                                </p>
                                </div>
                                </div>
                                <br/>
                                <div>
                                <p> Sender Contact No : {contact}
                                </p>
                                <p>
                                Atlanta India &copy; 2024 Schoolbuddy
                                </p>

                                </div>
                                ";
                mailmessage.Subject = Subject;
                mailmessage.Body = body;
                mailmessage.IsBodyHtml = true;
                SmtpClient smtpclient = new SmtpClient("smtppro.zoho.com", 587);
                smtpclient.EnableSsl = true;

                NetworkCredential basicCredential1 = new
                NetworkCredential(MailFrom, "Atl@135#@");
                smtpclient.UseDefaultCredentials = false;
                smtpclient.Credentials = basicCredential1;
                try
                {
                    smtpclient.Send(mailmessage);
                    return true;
                }

                catch (Exception ex)
                { 
                    return false;
                }
                
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public string otpgenerator(string secretKey)
        {
            var secretkeyinbytes = Base32Encoding.ToBytes(secretKey);
            var totp = new Totp(secretkeyinbytes);
            string OTP = totp.ComputeTotp();
            return OTP;
        }


        public string insert_student(getstudent students,string parent_id)
        {
            string query = $"insert into bs_student_master_backup (parent_id,admission_no,student_name,dob,class,section,";
            query += $"father_name,mobile_no1,street,rf_id,email,added_on,sys_user_id) ";
            query += $"values ";
            query += $"('{parent_id}','{students.admission_no}','{students.student_name}','{students.birth}',";
            query += $"'{students.class_}','{students.division}','{students.parent_name}','{students.mobile_no1}',";
            query += $"'{students.street}','{students.rf_id}','{students.email}',";
            query += $"getdate(),'{students.user_id}');";
            return query;
        }


        public async Task<string> SendSms(string parent_name,  string contact, string sender_id)
        {

            //string receivedData = string.Empty;

            try
            {
                string otp = otpgenerator("JBSWY3DPEHPK3PXP");
                bool result = await callsendsmsapi(parent_name,otp,contact,sender_id);
                if(result)
                {
                    return otp;
                }

                return "0";

            }
            catch (Exception ex)
            {
                return "0";
            }
        }

        public async Task<bool> callsendsmsapi(string parent_name,string otp,string number,string sender_id)
        {
            //string Message = $"Hey {parent_name}, your code is {otp}. Please enter the provided code on the page where you asked for OTP on schoolbuddy.For Inqueried Contact - +911149039798/799/718 Email- support@atlantasys.com -Atlanta Systems";
            string template_id = "1107171799923740250";
            string username = "Atlanta-Systems";
            string password = "Sandeep@123";
            string Message = $"Dear Sir/Mam Vehicle No. - {otp}. Your vehicle VLTD is not functional on CDAC Portal, So please contact to your nearest RFC and get this issue fixed. Contact - +911149039798/799/718 Email- support@atlantasys.com -Atlanta Systems";

            // Constructing the JSON payload
            string jsonPayload = "{ \"from\":\"" + sender_id + "\",\"peid\":\"1101423770000011614\", \"to\":\""+number+"\", \"text\":\""+Message+"\", \"regional\": { \"indiaDlt\": { \"principalEntityId\": \"1101423770000011614\", \"contentTemplateId\": \""+template_id+"\" }}}";

            // Encoding credentials for basic authentication
            string authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));

            // HTTP client setup
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Setting up HTTP POST request
                    client.DefaultRequestHeaders.Add("accept", "application/json");
                    client.DefaultRequestHeaders.Add("authorization", $"Basic {authHeader}");
                    //client.DefaultRequestHeaders.Add("content-type", "application/json");

                    // Sending POST request
                    HttpResponseMessage response = await client.PostAsync("http://api.infobip.com/sms/1/text/single", new StringContent(jsonPayload, Encoding.UTF8, "application/json"));

                    // Reading response
                    string responseBody = await response.Content.ReadAsStringAsync();

                    // Handling response
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("SMS sent successfully.");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"Failed to send SMS. Status code: {response.StatusCode}. Error: {responseBody}");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    return false;
                }
            }
        }



        public static async Task SendSmsAsync(string[] data, string vehicle)
        {
            HttpClient client = new HttpClient();
            string number = "91" + data[0];
            //string number = "918755633016";

            // Message content
            string message = $"Dear Sir/Mam Vehicle No. - {vehicle}  sent Your vehicle VLTD is showing polling status 'NOT CONNECTED' on VLTD Odisha Transport Portal, So please connect the device as soon as possible or if it's already connected then please contact to your nearest RFC and get this issue fixed. Contact - +911149039798/799/718 Email- support@atlantasys.com -Atlanta Systems Pvt Ltd -Atlanta Systems";
            // Template ID
            string templateId = "1107172491692872175";

            // Basic authentication
            var username = "Atlanta-Systems";
            var password = "Sandeep@123";
            var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
            var authHeader = Convert.ToBase64String(byteArray);

            // Prepare the request
            var json = new
            {
                from = "TSTTAV",
                peid = "1101423770000011614",
                to = number,
                text = message,
                regional = new
                {
                    indiaDlt = new
                    {
                        principalEntityId = "1101423770000011614",
                        contentTemplateId = templateId
                    }
                }
            };

            var jsonString = System.Text.Json.JsonSerializer.Serialize(json);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeader);

            // Send the request
            HttpResponseMessage response = await client.PostAsync("http://api.infobip.com/sms/1/text/single", content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"{number} Done");
            }
            else
            {
                Console.WriteLine($"Error sending SMS: {response.StatusCode}");
                var errorMessage = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response: {errorMessage}");
            }
        }








        public bool saveListToExcel(List<error> errors,string route_id)
        {
            try
            {
                string filename_ = route_id + DateTime.Now.ToString("ddMMyyyymmhhss")+".xlsx";
                string folderpath = Path.Combine(Directory.GetCurrentDirectory());
                string filePath = Path.Combine(folderpath, filename_);

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Errors");

                    // Add headers
                    worksheet.Cells[1, 1].Value = "S.No";
                    worksheet.Cells[1, 2].Value = "Student Name";
                    worksheet.Cells[1, 3].Value = "Admission No";
                    worksheet.Cells[1, 4].Value = "Stop Id";
                    worksheet.Cells[1, 5].Value = "RF_ID";
                    worksheet.Cells[1, 6].Value = "Reason";


                    // Add data
                    int sno = 0;
                    int row = 2;
                    foreach (var error in errors)
                    {
                        worksheet.Cells[row, 1].Value = sno + 1;
                        worksheet.Cells[row, 2].Value = error.student_name;
                        worksheet.Cells[row, 3].Value = error.admission_no;
                        worksheet.Cells[row, 4].Value = error.Stopid;
                        worksheet.Cells[row, 5].Value = error.rfid;
                        worksheet.Cells[row, 6].Value = error.reason_for_not_linked;

                        row++;
                        sno++;
                    }

                    // Save the Excel package to the specified file path
                    FileInfo fileInfo = new FileInfo(filePath);
                    package.SaveAs(fileInfo);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
          
        }


        //public double GetDistance1(string slat, string slng, string elat, string elon)
        //{
        //    try
        //    {
        //        var sCoord = new GeoCoordinate(Convert.ToDouble(slat), Convert.ToDouble(slng));
        //        var eCoord = new GeoCoordinate(Convert.ToDouble(elat), Convert.ToDouble(elon));
        //        var dis = Math.Round(general.GetDistanceTo(eCoord), 2);

        //        return dis;
        //    }
        //    catch { return 0.0; }
        //}
        public bool saveListToExcel(List<error> errors)
        {
            try
            {
                string filename_ =  DateTime.Now.ToString("ddMMyyyy");
                string folderpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "errors");
                string filePath = Path.Combine(folderpath, filename_);

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Errors");

                    // Add headers
                    worksheet.Cells[1, 1].Value = "S.No";
                    worksheet.Cells[1, 2].Value = "Student Name";
                    worksheet.Cells[1, 3].Value = "Admission No";
                    worksheet.Cells[1, 4].Value = "Stop Id";
                    worksheet.Cells[1, 5].Value = "RF_ID";
                    worksheet.Cells[1, 6].Value = "Reason";

                    // Add data
                    int sno = 0;
                    int row = 2;
                    foreach (var error in errors)
                    {
                        worksheet.Cells[row, 1].Value = sno + 1;
                        worksheet.Cells[row, 2].Value = error.student_name;
                        worksheet.Cells[row, 3].Value = error.admission_no;
                        worksheet.Cells[row, 4].Value = error.Stopid;
                        worksheet.Cells[row, 5].Value = error.rfid;
                        worksheet.Cells[row, 6].Value = error.reason_for_not_linked;
                        row++;
                        sno++;
                    }

                    // Save the Excel package to the specified file path
                    FileInfo fileInfo = new FileInfo(filePath);
                    package.SaveAs(fileInfo);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        //public static double GetDistanceTo(GeoCoordinate other)
        //{
        //    if (double.IsNaN(Latitude) || double.IsNaN(Longitude) || double.IsNaN(other.Latitude) || double.IsNaN(other.Longitude))
        //    {
        //        throw new ArgumentException(SR.GetString("Argument_LatitudeOrLongitudeIsNotANumber"));
        //    }

        //    double num = double.NaN;
        //    double num2 = Latitude * (Math.PI / 180.0);
        //    double num3 = Longitude * (Math.PI / 180.0);
        //    double num4 = other.Latitude * (Math.PI / 180.0);
        //    double num5 = other.Longitude * (Math.PI / 180.0);
        //    double num6 = num5 - num3;
        //    double num7 = num4 - num2;
        //    double num8 = Math.Pow(Math.Sin(num7 / 2.0), 2.0) + Math.Cos(num2) * Math.Cos(num4) * Math.Pow(Math.Sin(num6 / 2.0), 2.0);
        //    double num9 = 2.0 * Math.Atan2(Math.Sqrt(num8), Math.Sqrt(1.0 - num8));
        //    return 6376500.0 * num9;
        //}



    }



}
