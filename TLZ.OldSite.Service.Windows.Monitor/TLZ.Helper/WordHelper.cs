using Novacode;
using System.IO;
using System.Net;

namespace TLZ.Helper
{
    public class WordHelper
    {
        private DocX _document;
        /// <summary>
        /// 
        /// </summary>
        public WordHelper Create()
        {
            using (var ms = new MemoryStream())
            {
                _document = DocX.Create(ms);
            }
            return this;
        }
        public WordHelper Top(float top)
        {
            _document.MarginTop = top;
            return this;
        }
        public WordHelper Left(float left)
        {
            _document.MarginLeft = left;
            return this;
        }
        public WordHelper Right(float right)
        {
            _document.MarginRight = right;
            return this;
        }
        public WordHelper Bottom(float bottom)
        {
            _document.MarginBottom = bottom;
            return this;
        }

        public void Save(string path)
        {
            _document.SaveAs(path);
        }
        public Stream Save()
        {
            //using (var ms = new MemoryStream())
            //{
            var ms = new MemoryStream();
                _document.SaveAs(ms);
                return ms;
           // }
        }

        public Table CreateTable(int row, int column)
        {
            return _document.InsertTable(row, column);
        }

        public void InsertPictureToCell(Table dt, int rowIndex, int cellIndex, int width, int height, string url)
        {
            var p = dt.Rows[rowIndex].Cells[cellIndex].Paragraphs[0];
            using (var ms = new MemoryStream())
            {
                var request = WebRequest.Create(url);
                var response = request.GetResponse();
                var stream = response.GetResponseStream();
                const int bufferLength = 1024;
                int actual;
                byte[] buffer = new byte[bufferLength];
                while ((actual = stream.Read(buffer, 0, bufferLength)) > 0)
                {
                    ms.Write(buffer, 0, actual);
                }
                ms.Position = 0;
                Novacode.Image img = _document.AddImage(ms);
                var pic = img.CreatePicture(width, height);
                p.InsertPicture(pic);
                p.Alignment = Alignment.center;
            }
        }

        public void InsertParagraph()
        {
            _document.InsertParagraph();
        }
    }
}
