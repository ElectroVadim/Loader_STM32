using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace bootloader_Csharp
{
    class FlashLoaderSTM32
    {
        private SerialPort serialPort1;

        public FlashLoaderSTM32(String File, String Com, int baudRate)
        {
            serialPort1 = new SerialPort();

            serialPort1.PortName = Com;// "COM3"; //Указываем наш порт
            serialPort1.BaudRate = baudRate;// 1200; //указываем скорость.
            serialPort1.DataBits = 8;
            serialPort1.DataReceived += OnDataRecive; // подпись на событие

            File_read = File;

            File_read_buffer = readFileBinToArray(File_read);
            //serialPort1.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived); //Для приема данных нам нужно создать обработчик события EventHandler для "SerialDataReceivedEventHandler":

            //serialPort1.Open(); //Открываем порт.
        }
        //--------------------------------------------------------------------
        //public Action<int> OnRecivePacket;
        //private SerialPort _port;
        private int position = 0;
        private byte[] buffer = {0};
        //--------------------------------------------------------------------
        private int count_data_write = 128;
        private string File_read;// = @"c:\\file.bin";
        private byte[] File_read_buffer = { };
        private byte[] start_address = { 0, 0, 0, 0 };
        private int start_address_int = 0x08000000;
        private byte[] start_data_packet = { 0x31, 0xCE };
        private static byte[] data_packet_go = { 0x7F }; // количество отправляемых байт (127)
        private static byte[] data_packet_last = { 0 };
        //private static int current_data_packet = 0;



        private byte[] conect = { 0x7F, 0x02, 0xFD, 0x11, 0xEE, 0x1F, 0xFF, 0xF7, 0xE0, 0xF7, 0x01, 0xFE, 0x00, 0xFF, 0x11, 0xEE, 0x1F, 0xFF, 0xF8, 0x00, 0x18, 0x0F, 0xF0 };
        //private byte[] number_ = { 0x31, 0xCE, 0x08, 0x00, 0x00, 0x00, 0x08, 0x7F };
        //private byte[] checksum = { 0xE9, 0x11, 0x00, 0x08, 0xE9, 0x11, 0x00, 0x08, 0xE9, 0x11, 0x00, 0x08, 0xE9, 0x11, 0x00, 0x08, 0xE9, 0x11, 0x00, 0x08, 0xE9, 0x11, 0x00, 0x08, 0xE9, 0x11, 0x00, 0x08, 0xE9, 0x11, 0x00, 0x08, 0xE9, 0x11, 0x00, 0x08, 0xE9, 0x11, 0x00, 0x08, 0xE9, 0x11, 0x00, 0x08, 0xE9, 0x11, 0x00, 0x08, 0xE9, 0x11, 0x00, 0x08, 0xE9, 0x11, 0x00, 0x08, 0xE9, 0x11, 0x00, 0x08, 0xE9, 0x11, 0x00, 0x08, 0xE9, 0x11, 0x00, 0x08, 0xE9, 0x11, 0x00, 0x08, 0xE9, 0x11, 0x00, 0x08, 0xE9, 0x11, 0x00, 0x08, 0xE9, 0x11, 0x00, 0x08, 0xE9, 0x11, 0x00, 0x08, 0xE9, 0x11, 0x00, 0x08, 0xE9, 0x11, 0x00, 0x08, 0xE9, 0x11, 0x00, 0x08, 0xE9, 0x11, 0x00, 0x08, 0xE9, 0x11, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        private byte[] buff = { };
        private byte[] buff1 = { };

        
        public void Loader_start()
        {

            // buff = lastPacketsWrite(conect,0);
            
           // WritePaketsByte(data_answer_checksum_endbyte(lastPacketsWrite(File_read_buffer, 40), 40));
           

            int count = 0;

            serialPort1.Open(); //Открываем порт.
            //Console.WriteLine(readAnswerbootloader());
            // buff1 = data_answer_checksum_endbyte(checksum);
            WriteByte(conect);
            Thread.Sleep(100);

            for (int y = 0; y < (counter_packets(File_read_buffer)); y++)
            {
                for (int i = 0; i < 3; i++)
                {
                    switch (i)
                    {
                        case 0:

                            if (WritePaketsByte(start_data_packet))
                            {
                                Console.WriteLine("етап 1");
                            }
                            else { i = 0; }
                            break;       
                        case 1:
                            byte[] addressByte = i_to_byte(start_address_int);

                            // WritePaketsByte(answer_checksum_endbyte(addressByte));
                            Thread.Sleep(100);
                            if (WritePaketsByte(answer_checksum_endbyte(addressByte)))
                            {
                                Console.WriteLine("етап 2");
                            }
                            else { i = 0; }
                            break;
                        case 2:
                            // WritePaketsByte(answer_checksum_endbyte(whichPacketsWrite(readFileBinToArray(@"c:\\file.bin"), count)));
                            //WritePaketsByte(data_packet_go);

                            if ((count-2) == (counter_packets(File_read_buffer)) )
                            {
                                if (WritePaketsByte(data_answer_checksum_endbyte(lastPacketsWrite(File_read_buffer, count), count)) )
                                {
                                    Console.WriteLine("етап 4 прошит");
                                }
                                else { i = 0;}
                            }
                            else
                            {
                                if (WritePaketsByte(data_answer_checksum_endbyte(whichPacketsWrite(File_read_buffer, count), count)) )
                                {
                                    start_address_int = start_address_int + count_data_write;
                                    
                                    Console.WriteLine("етап 3");
                                    Console.WriteLine(count++);
                                }
                                else { i = 0;}
                            }
                            break;

                        default:
                            Console.WriteLine("не работает нифига");
                            break;
                    }
                }
            }

            Console.ReadKey();
            serialPort1.Close(); //Закрываем порт
        }

        //-------------------------------------------------------------------------------------------

        private byte[] whichPacketsWrite(byte[] BYTE, int number_packets) // номер прошиваемого пакета
        {
            int curent_byte = number_packets * count_data_write;

            byte[] buff_write = new byte[count_data_write];

            for (int i = 0; i < count_data_write; i++)
            {
                buff_write[i] = BYTE[i + curent_byte];
            }
            return buff_write;
        }
        //-----------------------------------------------------------------------------------------------
        private byte[] lastPacketsWrite(byte[] BYTE, int number_packets) // номер последнего пакета
        {
            int curent_byte = number_packets * count_data_write;

            data_packet_last[0] = (byte)((BYTE.Length) - curent_byte);

            byte[] buff_write = new byte[(byte)((BYTE.Length) - curent_byte)];

            for (int i = 0; i < (BYTE.Length); i++)
            {
                buff_write[i] = BYTE[i + curent_byte];
            }

            //byte[] buff_write = new byte[count_data_write];
            //for (int i = 0; i < count_data_write; i++)
            //{
            //    buff_write[i] = BYTE[i + curent_byte];
            //}
            return buff_write;
        }
        //--------------------------------------------------------------------------------------------

        private byte[] i_to_byte(int number)  //конвертация инт в масив байтов
        {
            int intValue = number;
            byte[] intBytes = BitConverter.GetBytes(intValue);
            Array.Reverse(intBytes);

            byte[] result = new byte[3];

            result = intBytes;

            return result;
        }
        //-----------------------------------------------------------------------------------------      
        private bool writeDataByte(byte[] buffByte)
        {

            return true;
        }
        //---------------------------------------------------------------------------------------
        private bool readAnswerbootloader()
        { 
            if (buffer[0] == 0x79)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
//-------------------------------------------------------
        private bool ireadAnswerbootloader()
        {

            //byte[] readBuff = new byte[];

            byte read = 1;

            while (read == 1)
            {
                read = (byte)serialPort1.ReadByte();
            }

            if (read == 0x79)
            {
                return true;
            } else
            {
                return false;
            }
            //serialPort1.ReadByte();
        }
 //-------------------------------------------------------------------------------------------------------
        
        private void OnDataRecive(object serialPort1, SerialDataReceivedEventArgs e)    // событие по чтению байтов из компорта 
        {
            SerialPort _sender = ((SerialPort)serialPort1);
            int length = _sender.BytesToRead;

            if (length > 0)
            {
                buffer = new byte[length];
                _sender.Read(buffer, position, length);
                position += length;
                if (position >= buffer.Length)
                {
                    position = 0;
                }
                //if (position == 30)
                //{
                //    if (OnRecivePacket != null)
                //    {
                //        OnRecivePacket(12);
                //    }
                //}
            }
        }
    ///------------------------------------------------------------------------------------------------------
    private byte[] readFileBinToArray(String pathIn)
        {
            //string pathIn = @"c:\\file.bin";
            //  pathOut = @"...";

            using (var fileIn = File.Open(pathIn, FileMode.Open, FileAccess.Read))
            //using (var fileOut = File.Open(pathOut, FileMode.Create, FileAccess.Write))
            {
                int bufferReadSize = 4 * 1024 * 1024;         // Количество читаемых байт
                buff = new byte[fileIn.Length]; // Нежелательно, если файл будет очень большим
                int offset = 0;
                                               // Чтение всего файла в buff
                do
                {
                    if ((offset + bufferReadSize) > buff.Length)
                        bufferReadSize = buff.Length - offset;

                    offset += fileIn.Read(buff, offset, bufferReadSize);
                } while (offset < buff.Length);
                //int aay = (int)buff.Length;
                //Console.WriteLine(aay); //(Convert.ToString(buff[0], 16)));
            }
                return buff;
        }

///---------------------------------------------------------------------------------------------------------
       private int counter_packets(byte[] buff)    //количество пакетов которые нужно передать         
        {
            int packets = 0;

            float counter = (float)buff.Length / count_data_write;

            if (counter > (buff.Length / count_data_write))
            {
                packets = (buff.Length / count_data_write) + 1;
            }
            else
            {
                packets = buff.Length / count_data_write;
            }

            return packets;
        }

////////////////////////////////////////////////////////////////////////////////////////////

        byte[] answer_checksum_endbyte(byte[] xor_sum)
        {
            byte answer_checksum = 0;

            byte[] answer_bytes_checksum = new byte[xor_sum.Length + 1];

            for (int i = 0; i < xor_sum.Length; i++)
            {
                answer_checksum ^= xor_sum[i];
            }
            for(int i = 0; i < xor_sum.Length; i++)
            {
                answer_bytes_checksum[i] = xor_sum[i];
            }
            answer_bytes_checksum[xor_sum.Length] = answer_checksum;
            
            return answer_bytes_checksum;
        }
////////////////////////////////////////////////////////////////////////////////////////
        byte[] data_answer_checksum_endbyte(byte[] xor_sum,int count)
        {
            int summa = xor_sum.Length;

            byte answer_checksum = 0;

            byte[] answer_bytes_checksum = new byte[summa + 2];


            if (count >= counter_packets(File_read_buffer))
            {
                answer_checksum ^= data_packet_last[0];
            }
            else
            {
                answer_checksum ^= data_packet_go[0];
            }
            answer_bytes_checksum[0] = data_packet_go[0];

            for (int i = 0; i < summa; i++)
            {
                answer_checksum ^= xor_sum[i];
            }
            for (int i = 1; i < (summa + 1); i++)
            {
                answer_bytes_checksum[i] = xor_sum[i-1];
            }
            answer_bytes_checksum[summa + 1] = answer_checksum;

            return answer_bytes_checksum;
        }
        
//----------------------------------------------------------------------------------------
        private bool WritePaketsByte(byte[] BYTE)
        {
                for (int i = 0; i < BYTE.Length; i++)
                {
                    Thread.Sleep(50);

                    serialPort1.Write(BYTE, i, 1);

                    Thread.Sleep(50);
                }
                if (readAnswerbootloader())
                 {
                    return true;
                 }else{
                    return false;
                 }
        }


        private void WriteByte(byte[] BYTE)
        {
            // ----------------------------------------------------------------------------

            for (int i = 0; i < BYTE.Length; i++)
            {
                Thread.Sleep(50);

                serialPort1.Write(BYTE, i, 1);

                Thread.Sleep(50);

            }
        }

        








        //////////////////////////////////////////////////////////////////////////////////////    

        //void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        //{
        //    Thread.Sleep(500);
        //    string data = serialPort1.ReadLine();
        //    // Привлечение делегата на потоке UI, и отправка данных, которые
        //    // были приняты привлеченным методом.
        //    // ---- Метод "si_DataReceived" будет выполнен в потоке UI,
        //    // который позволит заполнить текстовое поле TextBox.
        //    this.BeginInvoke(new SetTextDeleg(si_DataReceived),
        //                     new object[] { data });
        //}

        //private void si_DataReceived(string data)
        //{
        //    data = data.Trim();
        //}

//////////////////////////////////////////////////////////////////////////////////


        

        //    int intb = 0x08000000 + 128;

        //    int intValue = intb;
        //    byte[] intBytes = BitConverter.GetBytes(intValue);
        //    Array.Reverse(intBytes);
        //    byte[] result = intBytes;

        //    for (int i = 0; i < conect.Length; i++)
        //    {
        //        Thread.Sleep(50);
        //        serialPort1.Write(conect, i, 1);
        //        Thread.Sleep(50);

        //    }



        //    int curent_byte = 0;

        //    for (int y = 0; y < counter_packets(BYTE); y++)             
        //    {
        //        byte[] buff_write = new byte[128];

        //        for (int i = 0; i < 128; i++)
        //        {
        //            buff_write[i] = BYTE[i + curent_byte];
        //        }

        //        serialPort1.Write(buff_write, 0, 128);

        //        curent_byte = curent_byte + 128;
        //        intb = intb + 128;

        //        Thread.Sleep(50);
        //    }
        //    //------------------------------------------------------------------------------



        //    serialPort1.Write(BYTE, data_number, 128);

        //    data_number = data_number + 128;

        //    data_address = 0x08000000 + data_number;

        //   // serialPort1.Write(number_, 0, 1);
        //    Thread.Sleep(50);
        //   // serialPort1.Write(number_, 1, 1);
        //    Thread.Sleep(50);
        //   // serialPort1.Write(number_, 1, 1);


        //    for (int i = 0; i < result.Length; i++)
        //    {
        //        intBytes[1] ^= result[1];

        //    }

        //    serialPort1.Write(BYTE, data_number, 128);

        //}

    }
}
