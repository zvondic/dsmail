using DSmail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenPop.Mime;
using OpenPop.Pop3;
using OpenPop.Pop3.Exceptions;

namespace DSmail
{
    public class Api
    {
        
        public static void ReceiveMessages(DSmail.Item[] pop3Items)
        {
            int success = 0;
            int fail = 0;
            try
            {
                for (int j = 0; j < pop3Items.Length; j++)
                {

                    DSmail.Item item = pop3Items[j];
                    try
                    {
                        if (!Directory.Exists(item.Path))
                        {
                            Directory.CreateDirectory(item.Path);
                        }
                        Pop3Client pop3Client;
                        pop3Client = new Pop3Client();
                        pop3Client.Connect(item.Server, item.Port, true, item.Timeout, item.Timeout, certificateValidator);
                        pop3Client.Authenticate(item.UserName, item.Passwd);
                        int maxCount = pop3Client.GetMessageCount();
                        int count = item.Count;
                        if (maxCount < count) count = maxCount;
                        for (int i = count; i >= 1; i -= 1)
                        {
                            try
                            {
                                Message msg = pop3Client.GetMessage(i);
                                foreach (var attachment in msg.FindAllAttachments())
                                {
                                    string filePath = Path.Combine(item.Path, attachment.FileName);
                                    Log.Information("Saving " + filePath);
                                    FileStream Stream = new FileStream(filePath, FileMode.Create);
                                    BinaryWriter BinaryStream = new BinaryWriter(Stream);
                                    BinaryStream.Write(attachment.Body);
                                    BinaryStream.Close();
                                }
                                pop3Client.DeleteMessage(i);
                                success++;
                            }
                            catch (Exception e)
                            {
                                Log.Error(
                                    "Message fetching failed: " + e.Message + "\r\n" +
                                    "Stack trace:\r\n" +
                                    e.StackTrace);
                                fail++;
                            }
                        }
                        Log.Information("Mail received!\nSuccesses: " + success + "\nFailed: " + fail + "[Message fetching done]");
                        pop3Client.Disconnect();
                    }
                    catch (InvalidLoginException)
                    {
                        Log.Error("The server did not accept the user credentials![POP3 Server Authentication]");
                    }
                    catch (PopServerNotFoundException)
                    {
                        Log.Error("The server could not be found [POP3 Retrieval]");
                    }
                    catch (PopServerLockedException)
                    {
                        Log.Information("The mailbox is locked. It might be in use or under maintenance. Are you connected elsewhere? [POP3 Account Locked]");
                    }
                    catch (LoginDelayException)
                    {
                        Log.Information("Login not allowed. Server enforces delay between logins. Have you connected recently? [POP3 Account Login Delay]");
                    }
                    catch (Exception e)
                    {
                        Log.Information("Error occurred retrieving mail. " + e.Message + "[POP3 Retrieval]");
                    }
                    finally
                    {
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Unspecfied error: " + e.Message + "\r\n" +
                                "Stack trace:\r\n" +
                                e.StackTrace);
            }
        }

        
        private static bool certificateValidator(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors)
        {
            // We should check if there are some SSLPolicyErrors, but here we simply say that
            // the certificate is okay - we trust it.
            return true;
        }

        
    }
}