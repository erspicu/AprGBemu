namespace AprEmu.GB
{
    public enum KeyMap
    {
        GB_btn_A = 1,
        GB_btn_B = 2,
        GB_btn_SELECT = 3,
        GB_btn_START = 4,
        GB_btn_RIGHT = 5,
        GB_btn_LEFT = 6,
        GB_btn_UP = 7,
        GB_btn_DOWN = 8
    }

    public partial class Apr_GB
    {
        //cpu register & flag
        public enum Flag_Status
        {
            clear = 0,
            set = 1,
        }

        //I/O Ports address define (sound &  serial transfer data not supported now)
        const ushort reg_P1_addr = 0xFF00; //joy pad info
        const ushort reg_DIV_addr = 0xFF04;// Divider Register
        const ushort reg_TIMA_addr = 0xFF05; //timer counter
        const ushort reg_TMA_addr = 0xFF06; //timer modulo
        const ushort reg_TAC_addr = 0xFF07; //timer control
        const ushort reg_IF_addr = 0xFF0F; // Interrupt Flag
        const ushort reg_LCDC_addr = 0xFF40; // LCD control
        const ushort reg_STAT_addr = 0xFF41; //LCDC  Status
        const ushort reg_SCY_addr = 0xFF42; //Scroll Y
        const ushort reg_SCX_addr = 0xFF43; // Scroll X
        const ushort reg_LY_addr = 0xFF44; // LCDC Y-Coordinate
        const ushort reg_LYC_addr = 0xFF45; //LY compare
        const ushort reg_DMA_addr = 0xFF46; // DMA Transfer and start addr
        const ushort reg_BGP_addr = 0xFF47; // BG & Window Palette Data
        const ushort reg_OBP0_addr = 0xFF48; // Object Palette 0 Data
        const ushort reg_OBP1_addr = 0xFF49; // Object Palette 1 Data
        const ushort reg_WY_addr = 0xFF4A; // window Y Position
        const ushort reg_WX_addr = 0xFF4B; // window X Position 
        const ushort reg_DisBootStrap_addr = 0xFF50; //disable bios rom
        const ushort reg_IE_addr = 0xFFFF; // Interrupt Enable

        // GameBoy bootstrap ROM 
        private byte[] BootStrap_DMG = new byte[256];
    }
}
