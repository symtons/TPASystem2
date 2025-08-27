using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TPASystem2.Documents
{
    public partial class ViewDocuments : System.Web.UI.Page
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CheckUserAccess();
                InitializePage();
                LoadDocuments();
            }
        }

        private void CheckUserAccess()
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
        }

        private void InitializePage()
        {
            litCurrentDate.Text = DateTime.Now.ToString("MMMM dd, yyyy");

            // Load employee information
            LoadEmployeeInfo();
        }

        private void LoadEmployeeInfo()
        {
            if (Session["UserId"] == null) return;

            int userId = Convert.ToInt32(Session["UserId"]);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
                    SELECT e.FirstName + ' ' + e.LastName AS FullName, 
                           e.EmployeeNumber
                    FROM Employees e 
                    INNER JOIN Users u ON e.UserId = u.Id 
                    WHERE u.Id = @UserId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            litEmployeeName.Text = reader["FullName"].ToString();
                            litEmployeeNumber.Text = reader["EmployeeNumber"].ToString();
                        }
                        else
                        {
                            litEmployeeName.Text = Session["Role"]?.ToString() ?? "User";
                            litEmployeeNumber.Text = "N/A";
                        }
                    }
                }
            }
        }

        private void LoadDocuments()
        {
            DataTable documents = GetDocumentsFromFolder();

            if (documents.Rows.Count > 0)
            {
                gvDocuments.DataSource = documents;
                gvDocuments.DataBind();
                gvDocuments.Visible = true;
                pnlNoDocuments.Visible = false;

                litDocumentCount.Text = $"{documents.Rows.Count} document{(documents.Rows.Count != 1 ? "s" : "")} found";
            }
            else
            {
                gvDocuments.Visible = false;
                pnlNoDocuments.Visible = true;
                litDocumentCount.Text = "0 documents found";
            }
        }

        private DataTable GetDocumentsFromFolder()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("DocumentName", typeof(string));
            dt.Columns.Add("OriginalFileName", typeof(string));
            dt.Columns.Add("Category", typeof(string));
            dt.Columns.Add("Section", typeof(string));
            dt.Columns.Add("FileExtension", typeof(string));
            dt.Columns.Add("FileSize", typeof(long));
            dt.Columns.Add("FilePath", typeof(string));
            dt.Columns.Add("CreatedDate", typeof(DateTime));

            try
            {
                // Get the files folder path
                string filesPath = Server.MapPath("~/files/");

                if (Directory.Exists(filesPath))
                {
                    string[] files = Directory.GetFiles(filesPath);
                    int id = 1;

                    foreach (string filePath in files)
                    {
                        FileInfo fileInfo = new FileInfo(filePath);

                        // Apply filters
                        if (!PassesFilters(fileInfo))
                            continue;

                        DataRow row = dt.NewRow();
                        row["Id"] = id++;
                        row["DocumentName"] = Path.GetFileNameWithoutExtension(fileInfo.Name);
                        row["OriginalFileName"] = fileInfo.Name;
                        row["Category"] = DetermineCategory(fileInfo.Extension);
                        row["Section"] = DetermineSection(fileInfo.Name);
                        row["FileExtension"] = fileInfo.Extension.ToUpper().TrimStart('.');
                        row["FileSize"] = fileInfo.Length;
                        row["FilePath"] = filePath;
                        row["CreatedDate"] = fileInfo.CreationTime;

                        dt.Rows.Add(row);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading documents: " + ex.Message, "error");
            }

            return dt;
        }

        private bool PassesFilters(FileInfo fileInfo)
        {
            // Category filter
            if (!string.IsNullOrEmpty(ddlCategoryFilter.SelectedValue))
            {
                string category = DetermineCategory(fileInfo.Extension);
                if (category != ddlCategoryFilter.SelectedValue)
                    return false;
            }

            // Section filter
            if (!string.IsNullOrEmpty(ddlSectionFilter.SelectedValue))
            {
                string section = DetermineSection(fileInfo.Name);
                if (section != ddlSectionFilter.SelectedValue)
                    return false;
            }

            // Search filter
            if (!string.IsNullOrEmpty(txtSearch.Text.Trim()))
            {
                string searchTerm = txtSearch.Text.Trim().ToLower();
                if (!fileInfo.Name.ToLower().Contains(searchTerm))
                    return false;
            }

            return true;
        }

        private string DetermineCategory(string extension)
        {
            extension = extension.ToLower();

            switch (extension)
            {
                case ".pdf":
                    return "POLICIES";
                case ".doc":
                case ".docx":
                    return "FORMS";
                case ".jpg":
                case ".jpeg":
                case ".png":
                    return "ANNOUNCEMENTS";
                case ".xls":
                case ".xlsx":
                    return "HR_DOCUMENTS";
                default:
                    return "TRAINING";
            }
        }

        private string DetermineSection(string fileName)
        {
            fileName = fileName.ToLower();

            if (fileName.Contains("hr") || fileName.Contains("employee"))
                return "HR_SECTION";
            else if (fileName.Contains("policy") || fileName.Contains("procedure"))
                return "MANAGEMENT";
            else if (fileName.Contains("public") || fileName.Contains("announcement"))
                return "PUBLIC_DOCS";
            else
                return "GENERAL";
        }

        protected string FormatFileSize(long bytes)
        {
            if (bytes < 1024) return bytes + " B";
            if (bytes < 1024 * 1024) return (bytes / 1024).ToString("F1") + " KB";
            if (bytes < 1024 * 1024 * 1024) return (bytes / (1024 * 1024)).ToString("F1") + " MB";
            return (bytes / (1024 * 1024 * 1024)).ToString("F1") + " GB";
        }

        protected void gvDocuments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewDocument" || e.CommandName == "DownloadDocument")
            {
                int documentId = Convert.ToInt32(e.CommandArgument);
                string fileName = GetOriginalFileName(documentId);

                if (!string.IsNullOrEmpty(fileName))
                {
                    string filePath = Server.MapPath("~/files/" + fileName);

                    if (File.Exists(filePath))
                    {
                        if (e.CommandName == "ViewDocument")
                        {
                            ViewDocument(filePath, fileName);
                        }
                        else if (e.CommandName == "DownloadDocument")
                        {
                            DownloadDocument(filePath, fileName);
                        }
                    }
                    else
                    {
                        ShowMessage($"File not found: {fileName}", "error");
                    }
                }
                else
                {
                    ShowMessage("Unable to retrieve file information.", "error");
                }
            }
        }

        private string GetOriginalFileName(int documentId)
        {
            // Get the original filename from our data source based on the document ID
            DataTable documents = GetDocumentsFromFolder();

            foreach (DataRow row in documents.Rows)
            {
                if (Convert.ToInt32(row["Id"]) == documentId)
                {
                    return row["OriginalFileName"].ToString();
                }
            }

            return string.Empty;
        }

        private void ViewDocument(string filePath, string fileName)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(filePath);
                string extension = fileInfo.Extension.ToLower();

                // Set modal information
                litModalDocumentName.Text = Path.GetFileNameWithoutExtension(fileName);
                litModalCategory.Text = DetermineCategory(extension);
                litModalSection.Text = DetermineSection(fileName);
                litModalFileSize.Text = FormatFileSize(fileInfo.Length);
                litModalUploadDate.Text = fileInfo.CreationTime.ToString("MMM dd, yyyy");

                // Hide all preview panels first
                pnlPDFPreview.Visible = false;
                pnlImagePreview.Visible = false;
                pnlUnsupportedPreview.Visible = false;

                // Show appropriate preview based on file type
                if (extension == ".pdf")
                {
                    pnlPDFPreview.Visible = true;
                    documentFrame.Src = "~/files/" + fileName;
                }
                else if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".gif")
                {
                    pnlImagePreview.Visible = true;
                    imgPreview.ImageUrl = "~/files/" + fileName;
                    imgPreview.AlternateText = fileName;
                }
                else
                {
                    pnlUnsupportedPreview.Visible = true;
                    ViewState["CurrentDocumentPath"] = filePath;
                    ViewState["CurrentDocumentName"] = fileName;
                }

                // Show modal using JavaScript
                ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "showDocumentModal();", true);
            }
            catch (Exception ex)
            {
                ShowMessage("Error viewing document: " + ex.Message, "error");
            }
        }

        private void DownloadDocument(string filePath, string fileName)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    byte[] fileBytes = File.ReadAllBytes(filePath);
                    string contentType = GetContentType(Path.GetExtension(fileName));

                    Response.Clear();
                    Response.ContentType = contentType;
                    Response.AddHeader("Content-Disposition", $"attachment; filename=\"{fileName}\"");
                    Response.AddHeader("Content-Length", fileBytes.Length.ToString());
                    Response.BinaryWrite(fileBytes);
                    Response.End();
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error downloading document: " + ex.Message, "error");
            }
        }

        private string GetContentType(string extension)
        {
            switch (extension.ToLower())
            {
                case ".pdf": return "application/pdf";
                case ".doc": return "application/msword";
                case ".docx": return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".xls": return "application/vnd.ms-excel";
                case ".xlsx": return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case ".jpg": case ".jpeg": return "image/jpeg";
                case ".png": return "image/png";
                case ".gif": return "image/gif";
                case ".txt": return "text/plain";
                default: return "application/octet-stream";
            }
        }

        protected void ddlCategoryFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDocuments();
        }

        protected void ddlSectionFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDocuments();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadDocuments();
        }

        protected void btnClearFilters_Click(object sender, EventArgs e)
        {
            ddlCategoryFilter.SelectedValue = "";
            ddlSectionFilter.SelectedValue = "";
            txtSearch.Text = "";
            LoadDocuments();
        }

        protected void btnCloseModal_Click(object sender, EventArgs e)
        {
            // Modal will be hidden by JavaScript
            ScriptManager.RegisterStartupScript(this, GetType(), "hideModal", "hideDocumentModal();", true);
        }

        protected void btnModalDownload_Click(object sender, EventArgs e)
        {
            if (ViewState["CurrentDocumentPath"] != null && ViewState["CurrentDocumentName"] != null)
            {
                string filePath = ViewState["CurrentDocumentPath"].ToString();
                string fileName = ViewState["CurrentDocumentName"].ToString();
                DownloadDocument(filePath, fileName);
            }
        }

        protected void btnUploadDocument_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Documents/DocumentUpload.aspx");
        }

        private void ShowMessage(string message, string type)
        {
            lblMessage.Text = message;
            pnlMessages.Visible = true;

            pnlMessages.CssClass = "alert-panel";
            switch (type.ToLower())
            {
                case "success":
                    pnlMessages.CssClass += " alert-success";
                    break;
                case "error":
                    pnlMessages.CssClass += " alert-error";
                    break;
                case "warning":
                    pnlMessages.CssClass += " alert-warning";
                    break;
                default:
                    pnlMessages.CssClass += " alert-info";
                    break;
            }
        }
    }
}