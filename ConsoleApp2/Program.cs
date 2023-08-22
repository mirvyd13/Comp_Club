using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ComputerClub computerClub = new ComputerClub(8);
            computerClub.Work();
        }
    }

    class ComputerClub
    {
        private int _money = 0;
        private List<Computer> _computers = new List<Computer>();
        private Queue<Client> _clients = new Queue<Client>();

        public ComputerClub(int computersCount)
        {
            Random random = new Random();
            for (int i = 0; i < computersCount; i++)
            {
                _computers.Add(new Computer(random.Next(5, 15)));
            }
            CreateNewClient(25, random);
        }

        public void CreateNewClient(int count, Random random)
        {
            for (int i = 0; i < count; i++)
            {
                _clients.Enqueue(new Client(random.Next(100, 251), random));
            }

        }
        public void Work()
        {
            while (_clients.Count > 0)
            {
                Client newClient = _clients.Dequeue();
                Console.WriteLine($"Баланс компьютерного клуба {_money} руб. Ждем нового клиента");
                Console.WriteLine($"У вас новый клиент, и он хочет купить {newClient.DesiredMinutes} минут");
                ShowAllComputersState();
                Console.WriteLine($"Вы предлагаете ему компьютер под номером:");
                string userInput = Console.ReadLine();
                if (int.TryParse(userInput, out int computerNumper))
                {
                    computerNumper -= 1;
                    if (computerNumper >= 0 && computerNumper < _computers.Count)
                    {
                        if (_computers[computerNumper]._isTaken)
                        {
                            Console.WriteLine("вы пытаетесь посадить клиента за компьютер который уже занят,  клиент ушел");
                        }
                        else
                        {
                            if (newClient.CheckSolvency(_computers[computerNumper]))
                            {
                                Console.WriteLine("Клиент пересчитал деньги и сел за компьютер" + computerNumper + 1);
                                _money += newClient.Pay();
                                _computers[computerNumper].BecomeTaken(newClient);
                            }
                            else
                            {
                                Console.WriteLine("У клиента не хватило денег и он ушел");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Вы сами не знаете за какой компьютер посадить клиента, клиент ушел");
                    }
                }
                else
                {
                    CreateNewClient(1, new Random());
                    Console.WriteLine("Неверный ввод, повторите");
                }
                Console.WriteLine("нажмите любую клавишу");
                Console.ReadKey();
                Console.Clear();
                SpendOneMinutes();
            }
        }
        private void ShowAllComputersState()
        {
            Console.WriteLine("\nСписок всех компьютеров:");
            for (int i = 0; i < _computers.Count; i++)
            {
                Console.Write(i + 1 + "-");
                _computers[i].ShowState();

            }
        }
        private void SpendOneMinutes()
        {
            foreach (var computers in _computers)
            {
                computers.SpendOneMinute();
            }
        }

        class Computer
        {

            private Client _client;
            private int _minutesRemining;
            public bool _isTaken
            {
                get
                { return _minutesRemining > 0; }
            }
            public int PricePerMinute { get; private set; }
            public Computer(int priceperminute)
            {
                PricePerMinute = priceperminute;
            }

            public void BecomeTaken(Client client)
            {
                _client = client;
                _minutesRemining = _client.DesiredMinutes;
            }

            public void BecomeEmpty()
            {
                _client = null;
            }

            public void SpendOneMinute()
            {
                _minutesRemining--;
            }

            public void ShowState()
            {
                if (_isTaken)
                {
                    Console.WriteLine($"Компьютер занят, осталось минут: {_minutesRemining}");
                }
                else
                {
                    Console.WriteLine($"компрьютер свободен, цена за минуту:{PricePerMinute}");
                }
            }
        }

        class Client
        {
            private int _money;
            private int _moneyToPay;
            public int DesiredMinutes { get; private set; }

            public Client(int money, Random random)
            {
                _money = money;
                DesiredMinutes = random.Next(10, 30);
            }
            public bool CheckSolvency(Computer computer)
            {
                _moneyToPay = DesiredMinutes * computer.PricePerMinute;
                if (_money >= _moneyToPay)
                {
                    return true;
                }
                else
                {
                    _moneyToPay = 0;
                    return false;
                }
            }
            public int Pay()
            {
                _money -= _moneyToPay;
                return _moneyToPay;
            }
        }
    }
}
