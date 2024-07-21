namespace App_Push_Consummer.Model.Queue
{
    public class QueueModel
    {
        public int queue_type { get; set; } // phân biệt các data nhận về lấy từ queue
        public string data_receiver { get; set; }   // data queue từ nơi khác push về
    }
}
