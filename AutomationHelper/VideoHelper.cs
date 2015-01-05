using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WMEncoderLib;

namespace AutomationHelper
{
    public class VideoHelper
    {
        private WMEncoder encoder;
        private string _savetoPath;

        public VideoHelper(string saveToPath)
        {
            if (string.IsNullOrEmpty(saveToPath))
                _savetoPath = string.Format(@"{0}\{1}.wmv", Environment.CurrentDirectory, TextHelper.RandomString(10));
            else
                _savetoPath = saveToPath;

            try
            {
                encoder = new WMEncoder();

                IWMEncSourceGroup2 SrcGrp;
                IWMEncSourceGroupCollection SrcGrpColl;
                SrcGrpColl = encoder.SourceGroupCollection;
                SrcGrp = (IWMEncSourceGroup2)SrcGrpColl.Add("SG_1");
                IWMEncVideoSource2 SrcVid;
                SrcVid = (IWMEncVideoSource2)SrcGrp.AddSource(WMENC_SOURCE_TYPE.WMENC_VIDEO);
                SrcVid.SetInput("ScreenCapture1", "ScreenCap", "");
                IWMEncProfileCollection ProColl;
                IWMEncProfile Pro;
                int i;
                long lLength;
                ProColl = encoder.ProfileCollection;
                lLength = ProColl.Count;

                for (i = 0; i < lLength; i++)
                {
                    Pro = ProColl.Item(i);
                    if (Pro.Name == "Screen Video High (CBR)")
                    {
                        SrcGrp.set_Profile((IWMEncProfile)Pro);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0}\r\nPlease install WMEncoder.exe http://www.microsoft.com/en-us/download/details.aspx?id=17792. \r\n And install WMEncoder Patch http://download.microsoft.com/download/0/3/D/03D35C05-67DA-40E0-9E45-3EA0CA6329A4/WindowsMedia9-KB929182-INTL.exe.", ex.Message));
            }
            
        }

        public void Start()
        {
            if (encoder != null)
            {
                IWMEncFile File;
                File = encoder.File;
                File.LocalFileName = _savetoPath;
                encoder.Start();
            }                 
        }

        public void Stop()
        {
            if (encoder != null)
                encoder.Stop();
        }
    }
}
