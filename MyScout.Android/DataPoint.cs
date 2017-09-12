using Android.Content;
using Android.Text;
using Android.Views;
using Android.Widget;
using System;

namespace MyScout.Android
{
    public class DataPoint
    {
        // Variables/Constants
        public string Name;
        public Type DataType;

        // Constructors
        public DataPoint() { }
        public DataPoint(string name, Type dataType)
        {
            Name = name;
            DataType = dataType;
        }

        // Methods
        public View GetGUIWidget(Context context)
        {
            if (DataType == typeof(bool))
            {
                return new CheckBox(context)
                {
                    Text = Name
                };
            }
            else
            {
                bool isNumber = (DataType == typeof(byte) || DataType == typeof(int) ||
                    DataType == typeof(float) || DataType == typeof(double) ||
                    DataType == typeof(long));

                bool isDecimal = (isNumber &&
                    (DataType == typeof(float) || DataType == typeof(double)));

                // TODO: Add a label with the name instead of just assigning a hint
                // TODO: Look into more touch-screen-friendly ways of doing number boxes
                var textBx = new EditText(context)
                {
                    Hint = Name,
                    InputType = (!isNumber) ? InputTypes.ClassText : InputTypes.ClassNumber |
                        ((!isDecimal) ? InputTypes.NumberFlagSigned :
                        InputTypes.NumberFlagDecimal | InputTypes.NumberFlagSigned)
                };

                if (isNumber)
                {
                    // TODO: Set filters to limit maximum values that can be given
                    //textBx.SetFilters(new IInputFilter[] { });
                }

                return textBx;
            }
        }
    }
}