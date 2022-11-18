using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;

namespace NNLibrary
{
    public class Network
    {
        int[] configuration;
        Matrix[] bias;
        Matrix[] neuron;
        Matrix[] weight;
        Matrix[] dw;
        Matrix[] t;
        Matrix[] dt;
        double alpha;

        public Network(int[] config, double alpha)
        {
            configuration = config; 
            neuron = new Matrix[configuration.Length];
            t = new Matrix[configuration.Length - 1];
            weight = new Matrix[configuration.Length - 1];
            dt = new Matrix[configuration.Length - 1];
            dw = new Matrix[configuration.Length - 1];
            bias = new Matrix[configuration.Length - 1];
            this.alpha = alpha; configuration = config;
            for (int i = 0; i < configuration.Length; i++)
            {
                neuron[i] = new Matrix(1, configuration[i]);
                if (i < configuration.Length - 1)
                    weight[i] = new Matrix(configuration[i], configuration[i + 1]);
                if (i > 0)
                    bias[i - 1] = new Matrix(1, configuration[i]);
            }
            //for (int i = 0; i < bias.Length; i++)
            //    bias[i].Random();
            for (int i = 0; i < weight.Length; i++)
                weight[i].Random();
            //weight.CopyTo(dw, 0);
            //for (int i = 1; i < neuron.Length; i++)
            //    t[i - 1] = neuron[i];
            //t.CopyTo(dt, 0);
        }

        public Matrix forward(Matrix input)
        {
            input = 1.0 / 255.0 * input;
            neuron[0] = Matrix.Sigmoid(input);
            for (int i = 0; i < configuration.Length - 1; i++)
            {
                t[i] = neuron[i] * weight[i] + bias[i];
                neuron[i + 1] = Matrix.Sigmoid(t[i]);
            }
            return neuron[neuron.Length - 1];
            //neuron[neuron.Length - 1] = Matrix.softmax(neuron[neuron.Length - 1]);
            //output = neuron[neuron.Length - 1];
            //output = Matrix.softmax(neuron[neuron.Length - 1]); ;
            //Console.WriteLine("{0}; {1}", output.GetElement(0,0), output.GetElement(0, 1));
        }

        public void backward(Matrix result)
        {
            neuron[neuron.Length - 1] = neuron[neuron.Length - 1] - result;
            dt[dt.Length - 1] = neuron[neuron.Length - 1] & Matrix.DSigmoid(t[t.Length - 1]);
            for (int i = neuron.Length - 2; i >= 0; i--)
            {
                dw[i] = neuron[i].Transpose() * dt[i];
                if (i == 0)
                    break;
                //dt[i - 1] = (weight[i] * dt[i].Transpose()).Transpose() & Matrix.DSigmoid(t[i - 1]);
                dt[i - 1] = dt[i] * weight[i].Transpose() & Matrix.DSigmoid(t[i - 1]);//////////
            }
            for (int i = 0; i < weight.Length; i++)
            {
                weight[i] = weight[i] - alpha * dw[i];
                bias[i] = bias[i] - alpha * dt[i];
            }
        }

        public void save(string file_name)
        {
            using (StreamWriter strwr = new StreamWriter(@"trained.csv", false))
            {
                for (int i = 0; i < weight.Length; i++)
                {
                    for (int j = 0; j < weight[i].Row; j++)
                    {
                        for (int k = 0; k < weight[i].Column; k++)
                        {
                            strwr.Write("{0}:", weight[i].GetElement(j, k));
                        }
                    }
                }
                for (int i = 0; i < bias.Length; i++)
                {
                    for (int j = 0; j < bias[i].Row; j++)
                    {
                        for (int k = 0; k < bias[i].Column; k++)
                        {
                            if (i == bias.Length - 1 && j == bias[i].Row - 1 && k == bias[i].Column - 1)
                                strwr.Write("{0}", bias[i].GetElement(j, k));
                            else
                                strwr.Write("{0}:", bias[i].GetElement(j, k));
                        }
                    }
                }
            }
        }

  public void Upload(string file_name)
        {
            using (TextFieldParser tfp = new TextFieldParser(file_name))
            {
                tfp.TextFieldType = FieldType.Delimited;
                tfp.SetDelimiters(":");

                string[] fields = tfp.ReadFields();
                Console.WriteLine(fields.Length);
                int counter = 0;
                for (int i = 0; i < weight.Length; i++)
                {
                    for (int j = 0; j < weight[i].Row; j++)
                    {
                        for (int k = 0; k < weight[i].Column; k++)
                        {
                            weight[i].EditElement(j, k, Convert.ToDouble(fields[counter]));
                            counter++;
                        }
                    }
                }
                for (int i = 0; i < bias.Length; i++)
                {
                    for (int j = 0; j < bias[i].Row; j++)
                    {
                        for (int k = 0; k < bias[i].Column; k++)
                        {
                            bias[i].EditElement(j, k, Convert.ToDouble(fields[counter]));
                            counter++;
                        }
                    }
                }

            }
        }


        public void train(string training_file_name)
        {
            Matrix matrix = new Matrix(28, 28);
            double result;
            using (TextFieldParser tfp = new TextFieldParser(training_file_name))
            {
                tfp.TextFieldType = FieldType.Delimited;
                tfp.SetDelimiters(",");
                int iter = 0;
                while (!tfp.EndOfData)
                {
                    //Console.WriteLine("Iteration = {0}", iter); iter++;
                    //if (iter % 1000 == 0)
                    //Console.Beep();
                    //if (iter == 1000)
                    //    break;iter++;
                    string[] fields = tfp.ReadFields();
                    result = Convert.ToDouble(fields[0]);
                    {
                        for (int I = 0; I < 28; I++)
                        {
                            for (int J = 0; J < 28; J++)
                            {
                                matrix.EditElement(I, J, Convert.ToDouble(fields[J + 28 * I + 1]));
                            }
                        }
                    }
                    training_without_convoulution(matrix, result);
                }
            }
            save(training_file_name);
            for(int i = 0;i<10;i++)
            Console.Beep();
        }

        public void training_without_convoulution(Matrix m, double result)
        {
            Matrix mout = new Matrix(1, configuration[configuration.Length-1]);
            Matrix mres = new Matrix(1, configuration[configuration.Length - 1]);
            mres.EditElement(0, Convert.ToInt32(result), 1);
            m = m.ToVectorOfRows_horizontal();
            forward(m);
            mout = Matrix.softmax(neuron[neuron.Length - 1]); /////////////
            //for (int i = 0; i < 10; i++)
            //{
            //    //Console.Write("{0} - {1:P1}%; | ", i, mout.GetElement(0, i));
            //}
            /////////////////////
            Matrix ERROR = (mout - mres) & (mout - mres);
            double error = 0;
            for (int i = 0; i < configuration[configuration.Length - 1]; i++)
                error += ERROR.GetElement(0, i);
            /////////////////////
            //Console.WriteLine("RESULT - {0}", result);
            //Console.WriteLine("ERROR - {0}", error);
            //Console.WriteLine("----------------------------------------------------------------");
            backward(mres);
        }

    }
}
