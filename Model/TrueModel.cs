using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EditorModel
{
    public class TrueModel : Subject
    {
        List<MapModel> mapModels;

        public List<MapModel> MapModels
        {
            get { return mapModels; }
            set { mapModels = value; }
        }

        static TrueModel instance;

        public static TrueModel Instance
        {
            get
            {
                if (instance == null)
                    instance = new TrueModel();
                return instance;
            }
        }

        private TrueModel()
        {
            mapModels = new List<MapModel>();
        }
    }
}
