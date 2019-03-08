using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Command_Interface
{

    public enum StatusKeys
    {
        RECORDING,
        REPLAY,
        CURRENTSCENE,
        STREAMING,
        CURRENTPROFILE,
        FILENAMEFORMAT
    }

    /// <summary>
    /// Holds the data for the current OBS status
    /// </summary>
    class OBSStatus
    {
        private Dictionary<string, ItemStatus> _statusCol;
        private string _formatString;


        public struct ItemStatus
        {
            public bool Enabled;
            public string Name;
            public string Status;
            private string _customFormat;
            private string _key;

            public ItemStatus(string _name, string _status = "", bool _enabled = false)
            {
                Name = _name;
                _key = "";
                Status = _status;
                Enabled = _enabled;
                _customFormat = "";
                Key = _name;
            }

            public string Key
            {
                get
                {
                    if (!_key.StartsWith("%"))
                        _key = $"%{_key}";
                    return _key;
                }
                set
                {
                    if (!value.StartsWith("%"))
                        _key = $"%{value}";
                    else
                        _key = value;
                }
            }

            public override string ToString()
            {

                if (_customFormat.Contains("%value"))
                    return _customFormat.Replace("%value", Status);
                else
                    return Status;
            }

            public string ToString(string _fmt)
            {
                if (_fmt.Contains("%value"))
                    _customFormat = _fmt;
                return ToString();
            }
        }

        public OBSStatus(string formatStr = "")
        {
            _statusCol = new Dictionary<string, ItemStatus>();
            Enum.GetValues(typeof(StatusKeys)).Cast<StatusKeys>().ToList().ForEach(k => {
                _statusCol.Add(k.ToString(), new ItemStatus(k.ToString()));
            });
            _formatString = formatStr;
        }

        public void changeStatus(ItemStatus _item)
        {
            if (!_statusCol.ContainsKey(_item.Name))
            {
                _statusCol.Add(_item.Name, _item);
            }
            else
                _statusCol[_item.Name] = _item;
        }

        public void changeStatus(string _item, string _status)
        {
            changeStatus(new ItemStatus(_item, _status));
        }

        public override string ToString()
        {
            string retVal = _formatString;
            _statusCol.Keys.ToList().ForEach(k => {
                var status = _statusCol[k];
                retVal = retVal.Replace(k, (status.Enabled ? status.ToString() : "Stat Disabled"));
            });
            return retVal;
        }

    }

    public static class StatusKeyExtensions
    {
        public static string ToString(this StatusKeys _key)
        {
            string keyStr = "";
            switch (_key)
            {
                case StatusKeys.RECORDING:
                    keyStr = "%RECORDING";
                    break;
                case StatusKeys.REPLAY:
                    keyStr = "%REPLAY";
                    break;
                case StatusKeys.CURRENTSCENE:
                    keyStr = "%CURRENTSCENE";
                    break;
                case StatusKeys.STREAMING:
                    keyStr = "%STREAMING";
                    break;
                case StatusKeys.CURRENTPROFILE:
                    keyStr = "%CURRENTPROFILE";
                    break;
                case StatusKeys.FILENAMEFORMAT:
                    keyStr = "%FILENAMEFORMAT";
                    break;
                default:
                    break;
            }
            return keyStr;
        }
    }
}
