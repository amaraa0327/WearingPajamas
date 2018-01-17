using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuSign
{
    class Program
    {
        public readonly string Temperature;
        private int[] Input;
        private Clothes[] Output;

        public Program(string pTemperature)
        {
            this.Temperature = pTemperature;
        }

        public void SetInput(int[] pInput)
        {
            this.Input = pInput;
            this.GenerateOutput();
        }

        private void GenerateOutput()
        {
            this.Output = new Clothes[this.Input.Length];

            for (int i = 0; i < this.Input.Length; i++)
            {
                //using factory method
                this.Output[i] = this.CreateClothesItemFactory(this.Input[i]);
            }
        }

        private Clothes CreateClothesItemFactory(int pCommand)
        {
            Clothes result;
            switch (pCommand)
            {
                case 1: result = new Footwear(this.Temperature); break;
                case 2: result = new Headwear(this.Temperature); break;
                case 3: result = new Socks(this.Temperature); break;
                case 4: result = new Shirt(this.Temperature); break;
                case 5: result = new Jacket(this.Temperature); break;
                case 6: result = new Pants(this.Temperature); break;
                case 7: result = new LeaveHouse(); break;
                default: result = new TakeOffPajamas(); break;
            }
            return result;
        }

        public void PrintOutput()
        {
            StringBuilder output = new StringBuilder();
            HashSet<int> puttedElements = new HashSet<int>();
            for (int i = 0; i < this.Output.Length; i++)
            {
                //rule of Taking of pajamas
                if (i == 0 && this.Output[i].Command != 8)
                {
                    output.Append("fail, ");
                    break;
                }
                //rule of Only one piece of clothing
                if (puttedElements.Contains(this.Output[i].Command))
                {
                    output.Append("fail, ");
                    break;
                }
                //rule of That Socks must be put on before shoes and Pants must be put on before shoes
                if ((this.Output[i].Command == 3 || this.Output[i].Command == 6) && (puttedElements.Contains(1)))
                {
                    output.Append("fail, ");
                    break;
                }
                //rule of That The shirt must be put on before the headwear or jacket
                if (this.Output[i].Command == 4 && (puttedElements.Contains(2) || puttedElements.Contains(5)))
                {
                    output.Append("fail, ");
                    break;
                }
                //rule of That You cannot leave the house until all items of clothing are on (except socks and a jacket when it’s hot)
                if ((this.Output[i].Command == 7) && ((this.Temperature.Equals("COLD") && puttedElements.Count < 7) || (this.Temperature.Equals("HOT") && puttedElements.Count < 5)))
                {
                    output.Append("fail, ");
                    break;
                }

                output.Append(this.Output[i].Value);
                output.Append(", ");
                puttedElements.Add(this.Output[i].Command);
                //rule of That if it is hot, don't need socks and jacket
                if (this.Output[i].Value.Equals("fail"))
                {
                    break;
                }
            }

            if (output.ToString().Length > 1)
                output.Remove(output.ToString().Length - 2, 2);

            Console.WriteLine(output.ToString());
        }

        static void Execute(string userInput)
        {
            #region [This bunch of codes are reading input and convert them compatible type of program]

            string[] inputStr = userInput.Split(new string[] { ", " }, StringSplitOptions.None);
            string[] inputTemper;
            int[] inputInt = new int[inputStr.Length];
            string temperature;
            if (inputStr.Length > 0)
            {
                inputTemper = inputStr[0].Split(' ');

                if (inputTemper.Length == 2)
                {
                    temperature = inputTemper[0];
                    inputStr[0] = inputTemper[1];
                }
                else
                    throw new Exception("Invalid input");
            }
            else
            {
                throw new Exception("Invalid input");
            }
            for (int i = 0; i < inputStr.Length; i++)
            {
                inputInt[i] = Convert.ToInt32(inputStr[i]);
            }

            #endregion

            //Start to execute program
            Program programOfWearing = new Program(temperature);

            programOfWearing.SetInput(inputInt);

            programOfWearing.PrintOutput();

            Console.ReadLine();
            //End of execution
        }
        static void Main(string[] args)
        {
            #region [Some test cases]
            string[] testInputs = new string[] { "HOT 8, 6, 4, 2, 1, 7", "COLD 8, 6, 3, 4, 2, 5, 1, 7", "HOT 8, 6, 6", "HOT 8, 6, 3", "COLD 8, 6, 3, 4, 2, 5, 7", "COLD 6" };

            foreach (string input in testInputs)
            {
                Console.WriteLine("Result of " + "[" + input + "]:");
                Execute(input);
            }
            #endregion

            #region [Or using user input]
            //string userInput = Console.ReadLine();

            //Execute(userInput);
            #endregion
        }
    }

    public interface ClothesItem
    {
        void InitValue();
    }

    public abstract class Clothes : ClothesItem
    {
        public string Temperature;
        public string Description;
        public string Value;
        public readonly int Command;
        public Clothes(string pTemperature, int pCommand)
        {
            this.Temperature = pTemperature;
            this.Command = pCommand;
            InitValue();
        }

        public abstract void InitValue();
    }

    public class Footwear : Clothes
    {
        public Footwear(string pTemperature) : base(pTemperature, 1)
        {
            this.Description = "Put on footwear";
        }

        public override void InitValue()
        {
            if (base.Temperature == "HOT")
                this.Value = "sandals";
            if (base.Temperature == "COLD")
                this.Value = "boots";
        }
    }

    public class Headwear : Clothes
    {
        public Headwear(string pTemperature) : base(pTemperature, 2)
        {
            this.Description = "Put on headwear";
        }

        public override void InitValue()
        {
            if (base.Temperature == "HOT")
                this.Value = "sun visor";
            if (base.Temperature == "COLD")
                this.Value = "hat";
        }
    }

    public class Socks : Clothes
    {
        public Socks(string pTemperature) : base(pTemperature, 3)
        {
            this.Description = "Put on socks";
        }

        public override void InitValue()
        {
            if (base.Temperature == "HOT")
                this.Value = "fail";
            if (base.Temperature == "COLD")
                this.Value = "socks";
        }
    }

    public class Shirt : Clothes
    {
        public Shirt(string pTemperature) : base(pTemperature, 4)
        {
            this.Description = "Put on shirt";
        }

        public override void InitValue()
        {
            if (base.Temperature == "HOT")
                this.Value = "t-shirt";
            if (base.Temperature == "COLD")
                this.Value = "shirt";
        }
    }

    public class Jacket : Clothes
    {
        public Jacket(string pTemperature) : base(pTemperature, 5)
        {
            this.Description = "Put on jacket";
        }

        public override void InitValue()
        {
            if (base.Temperature == "HOT")
                this.Value = "fail";
            if (base.Temperature == "COLD")
                this.Value = "jacket";
        }
    }

    public class Pants : Clothes
    {
        public Pants(string pTemperature) : base(pTemperature, 6)
        {
            this.Description = "Put on pants";
        }

        public override void InitValue()
        {
            if (base.Temperature == "HOT")
                this.Value = "short";
            if (base.Temperature == "COLD")
                this.Value = "pants";
        }
    }

    public class LeaveHouse : Clothes
    {
        public int pCommand;
        public LeaveHouse() : base("", 7)
        {
            this.Description = "Leave houses";
        }

        public override void InitValue()
        {
            this.Value = "leaving house";
        }
    }

    public class TakeOffPajamas : Clothes
    {
        public int pCommand;
        public TakeOffPajamas() : base("", 8)
        {
            this.Description = "Take off pajamas";
            this.pCommand = 8;
        }

        public override void InitValue()
        {
            this.Value = "Removing PJs";
        }
    }
}
