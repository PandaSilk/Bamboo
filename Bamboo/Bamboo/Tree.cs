using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Net;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading;
using System.Timers;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web;
using System.Xml.Linq;
using System.Xml;
//using Microsoft.Exchange.WebServices.Data;
//must install & reference EWS SDK found here http://www.microsoft.com/en-us/download/details.aspx?id=42022 to use this.

using System.Management;
using System.Security.Permissions;
using Microsoft.Win32;
using System.Net.NetworkInformation;
using Microsoft.SqlServer.Server;

namespace Bamboo {

#region "About Me!"
// This class/region section must remain in the library under the License Terms. For more info see the ReadME.txt
public static class About { 
    public static string Version = @"1.0.0";
    public static string Author = @"Christian Rosen";
    public static string Email = @"Panda.Silk@gmail.com";
    public static string Website = @"https://github.com/PandaSilk";

    public static string PrintMe() { 
    return "Version: \t"+Version + "\nAuthor: \t"+Author+"\nE-mail: \t"+Email+"\nWebSite: \t"+Website;
    }
}
#endregion

// Class to handle logging requirements to a database or flatfile
public class Loggers {

    private bool Initialized = false;
    private string FileName = @"";
    private string FilePath = AppDomain.CurrentDomain.BaseDirectory;

    public void Initialize(out string Err) { 
    Err = null;
    }

    public void UpdateLogFlatFile(string Event, string Message, out string Err) {
    if (Initialized != true) { Err = "Logger.Err: Must be initialized."; return; }
    FileStream FS = new FileStream(FilePath + FileName, FileMode.OpenOrCreate | FileMode.Append, FileAccess.Write);
    StreamWriter SW = new StreamWriter(FS);
    SW.Write(DateTime.Now.ToString() + "\t" + Event + "\t" + Message + "\r\n");
    SW.Close();
    FS.Close();
    Err = null;
    }

}

// Class to handle sending emails with attackments
public class Emails {

}

// Class to handle interactions with databases and database functions
public class Databases { 

}

// Class to handle interactions with flatfiles
public class Files { 

    public void ReadExcelFile(string Filename, ref DataTable tmpTableData, string Sheetname, string Range, bool HeaderRow, out string ErrMessage, out string sResult) { 
    try {
    ErrMessage = null;
    if (Range == "") { Range = "A1:A1"; }
    string Conn = @"Provider=Microsoft.ACE.OLEDB.12.0; Data Source='" + Filename + @"';Extended Properties=""Excel 12.0;HDR=" + HeaderRow.ToString().ToUpper() + @";""";
    string Query = @"SELECT * FROM [" + Sheetname + @"$" + Range + @"]";
    OleDbDataAdapter adapter = new OleDbDataAdapter(Query, Conn);
    DataSet ds = new DataSet();
    adapter.TableMappings.Add("data", "datatable");
    adapter.Fill(ds, "datatable");
    DataTable dt = ds.Tables[0];
    tmpTableData = dt;
    sResult = dt.Rows.Count.ToString();
    } catch (Exception ex) { ErrMessage = ex.Message.ToString(); sResult = null; }
    }

}

// Class to handle interactions with strings
public class Strings { 

}

// Class to handle interaction with threads/multithreading
public class Threads { 
    public delegate void DoWorkDelegate(object sender, DoWorkEventArgs e);
    private BackgroundWorker Thread = null;
    private bool _Running = false;
    private int _Progress = 0;
    private string _Result = "";

    private void Thread_WorkStart(object sender, DoWorkEventArgs e) {
    _Running = true;
    }

    private void Thread_WorkChanged(object sender, ProgressChangedEventArgs e) {
    _Progress = e.ProgressPercentage;
    }

    private void Thread_WorkCompleted(object sender, RunWorkerCompletedEventArgs e) {
    _Running = false;
    }

    public void CheckThread(out bool tRunning, out int tProgress, out string tResult) { 
    tRunning = _Running;
    tProgress = _Progress;
    tResult = _Result;
    }

    public void CancelThread() { 
    Thread.CancelAsync();
    }

    public void CreateThread(Delegate tMethod, object[] Params) {
    BackgroundWorker Thread = new BackgroundWorker();
    //Thread.DoWork += new DoWorkEventHandler(tMethod);
    Thread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Thread_WorkCompleted);
    Thread.ProgressChanged += new ProgressChangedEventHandler(Thread_WorkChanged);
    Thread.RunWorkerAsync(Params);
    }
}

// Class to handle interactions with shelling to applications with commandlines
public class Shells { 

    public void ShellTo(string sCommand, string sCommandParameters, string sWorkingDirectory, out string ErrMessage, out string sResult) { 
    try {
	System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
	pProcess.StartInfo.FileName = sCommand;
	pProcess.StartInfo.Arguments = sCommandParameters;
	pProcess.StartInfo.UseShellExecute = false;
	pProcess.StartInfo.RedirectStandardOutput = true;   
	pProcess.StartInfo.WorkingDirectory = sWorkingDirectory;
	pProcess.Start();
	string sOutput = pProcess.StandardOutput.ReadToEnd();
    pProcess.WaitForExit(1);
    ErrMessage = null;
    sResult = sOutput;
    return;
    } catch (Exception ex) { ErrMessage = ex.Message.ToString(); sResult = null; return; }
    }

}

// Class to handle interactions with active directory
public class ActiveDirectoryMore {
    
    public string GetProperties(ResultPropertyValueCollection item) {
    string tmpResult = "";
    if (item.Count > 0) { tmpResult = item[0].ToString(); }
    else { tmpResult = ""; }
    return tmpResult;
    }

    public bool LDAP_ExistsServer(string tServer, string tDomainPath, out string ErrMessage, out string sResult) {
    try {
    ErrMessage = null;
    DirectoryEntry entry = new DirectoryEntry(@"LDAP://" + tDomainPath.ToLower().Trim());
    DirectorySearcher mySearcher = new DirectorySearcher(entry);
    mySearcher.Filter = (@"(&(objectClass=computer)(operatingsystem=windows*server*)(Name=" + tServer.ToLower().Trim() + "))");
    sResult = mySearcher.FindAll().Count.ToString();
    if (mySearcher.FindAll().Count == 1) { return true; } else { return false; }
    } catch (Exception ex) { ErrMessage = ex.Message.ToString(); sResult = null; return false; }
    }

    public ArrayList LDAP_GetUserDetails(string tUsername, string tDomainPath, out string ErrMessage, out string sResult) {
    try {
    ErrMessage = null;
    ArrayList tArray = new ArrayList();
    DirectoryEntry deEntry = new DirectoryEntry("LDAP://" + tDomainPath.ToLower().Trim());
    DirectorySearcher deSearch = new DirectorySearcher(deEntry);
    deSearch.Filter = "(&(objectClass=user)(anr=" + tUsername + ")(|(userAccountControl=512)(userAccountControl=66048)))";
    sResult = deSearch.FindAll().Count.ToString();
    if ( deSearch.FindAll().Count > 0 ) {
    tArray.Add(GetProperties(deSearch.FindAll()[0].Properties["name"]));
    tArray.Add(GetProperties(deSearch.FindAll()[0].Properties["mail"]));
    tArray.Add(GetProperties(deSearch.FindAll()[0].Properties["telephoneNumber"]));
    tArray.Add(GetProperties(deSearch.FindAll()[0].Properties["title"]));
    tArray.Add(GetProperties(deSearch.FindAll()[0].Properties["sAMAccountName"]));
    }
    return tArray;
    } catch (Exception ex) { ErrMessage = ex.Message.ToString(); sResult = null; return null; }
    }

    public ArrayList DNS_GetIpAddresses(string tHostnameOrURL, bool InterNetworkOnly, out string ErrMessage, out string sResult) {
    try {
    ErrMessage = null;
    ArrayList tArray = new ArrayList();
    IPAddress[] Addresses = (IPAddress[])Dns.GetHostEntry(tHostnameOrURL).AddressList;
    sResult = Addresses.Length.ToString();
    foreach (IPAddress Address in Addresses) {
    if (InterNetworkOnly == true) { 
    if (Address.AddressFamily == AddressFamily.InterNetwork) { tArray.Add(Address); }
    } else { 
    tArray.Add(Address);
    }
    }
    return tArray;
    } catch (Exception ex) { ErrMessage = ex.Message.ToString(); sResult = null; return null; }
    }

}

// Class to handle impersinations
public class Impersonate {

    private IntPtr TokenHandle = new IntPtr(0);
    private IntPtr dupeTokenHandle = new IntPtr(0);
    private WindowsImpersonationContext impersonatedUser;
    private const int LOGON32_LOGON_INTERACTIVE = 2;
    private const int LOGON32_PROVIDER_DEFAULT = 0;
    public bool IsImpersonated = false;

    [DllImport("advapi32.dll")]
    public static extern int LogonUserA(String lpszUserName, String lpszDomain, String lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);
    [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int DuplicateToken(IntPtr hToken, int impersonationLevel, ref IntPtr hNewToken);
    [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool RevertToSelf();
    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    public static extern bool CloseHandle(IntPtr handle);

    public void ImpersonateStart(String userName, String domain, String password) {
    IsImpersonated = false;
    WindowsIdentity tempWindowsIdentity;

    if (RevertToSelf()) {
    if (LogonUserA(userName, domain, password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref TokenHandle) != 0) {
    if (DuplicateToken(TokenHandle, 2, ref dupeTokenHandle) != 0) {
    tempWindowsIdentity = new WindowsIdentity(dupeTokenHandle);
    impersonatedUser = tempWindowsIdentity.Impersonate();
    if (impersonatedUser != null) { CloseHandle(TokenHandle); CloseHandle(dupeTokenHandle); }
    }
    }
    }

    if (TokenHandle != IntPtr.Zero) { CloseHandle(TokenHandle); }
    if (dupeTokenHandle != IntPtr.Zero) { CloseHandle(dupeTokenHandle); }
    string tmpDomainUser = domain.ToUpper() + @"\" + userName.ToUpper();
    string tmpWinUser = WindowsIdentity.GetCurrent().Name.ToUpper();
    if (tmpDomainUser == tmpWinUser) { IsImpersonated = true; } else { IsImpersonated = false; }
    }

    public void ImpersonateStop() { 
    if (impersonatedUser != null) {
    impersonatedUser.Undo();
    if (!System.IntPtr.Equals(TokenHandle, IntPtr.Zero)) { CloseHandle(TokenHandle); }
    }
    IsImpersonated = false;
    }

}

// Class to handle interactions with registry
public class WinRegistry {
	public const uint LOCAL_MACHINE = 0x80000002;
	public const uint CLASSES_ROOT = 0x80000000;
	public const uint CURRENT_USER = 0x80000001;
	public const uint USERS = 0x80000003;
	public const uint CURRENT_CONFIG = 0x80000005;
	public const uint DYN_DATA = 0x80000006;
}

}