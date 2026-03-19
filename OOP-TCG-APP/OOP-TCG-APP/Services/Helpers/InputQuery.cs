namespace TCGAPP
{
    public class InputQuery
    {
        private static Dictionary<string, string> _rawQuery = new Dictionary<string, string>();
        public static Dictionary<string, string> Run<T>(Endpoint endpoint)
        {
            _rawQuery = new Dictionary<string, string>();

            if (endpoint.Parameters == null)
            {
                return _rawQuery;
            }
            Console.WriteLine($"Input Query Parameters");
            foreach (var param in endpoint.Parameters.Parameters)
            {
                var qp = param.Value;
                string input;

                while (true)
                {
                    if (qp.Options != null && qp.Options.Count > 0)
                    {
                        Console.WriteLine($"{qp.Label}:");

                        for (int i = 0; i < qp.Options.Count; i++)
                        {
                            Console.WriteLine($"[{i + 1}] {qp.Options[i]}");
                        }

                        Console.Write("Choose option (or leave blank): ");
                        input = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(input))
                        {
                            if (qp.IsRequired == true)
                            {
                                Console.WriteLine("This field is required.\n");
                                continue;
                            }

                            break;
                        }

                        if (int.TryParse(input, out int choice) && choice >= 1 && choice <= qp.Options.Count)
                        {
                            _rawQuery[param.Key] = qp.Options[choice - 1]!.ToString()!;
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Invalid selection.\n");
                            continue;
                        }
                    }
                    else
                    {
                        Console.Write($"{qp.Label}: ");
                        input = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(input))
                        {
                            if (qp.IsRequired)
                            {
                                Console.WriteLine("This field is required.\n");
                                continue;
                            }

                            break;
                        }

                        _rawQuery[param.Key] = input!;
                        break;
                    }

                }
            }
            return _rawQuery;
        }
    }
}
