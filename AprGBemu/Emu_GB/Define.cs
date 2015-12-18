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

    public enum ScreenPalette
    {
        DarkWhite = 0,
        ClassicGreen = 1
    }

    public enum HardwareType
    {
        GameBoy,
        GameBoyColor,
        SuperGameBoy
    }

    public enum NewDMAType
    {
        General,
        HBlank
    }

    public enum CartridgeType
    {
        ROM_ONLY = 0x0,
        ROM_MBC1 = 0x1,
        ROM_MBC1_RAM = 0x2,
        ROM_MBC1_RAM_BATT = 0x3,
        ROM_MBC2 = 0x5,
        ROM_MBC2_BATT = 0x6,
        ROM_RAM = 0x8,
        ROM_RAM_BATT = 0x9,
        ROM_MMM01 = 0xb,
        ROM_MMM01_SRAM = 0xc,
        ROM_MMM01_SRAM_BATT = 0xd,
        ROM_MBC3_RAM = 0x12,
        ROM_MBC3_RAM_BATT = 0x13,
        ROM_MBC5 = 0x19,
        ROM_MBC5_RAM = 0x1a,
        ROM_MBC5_RAM_BATT = 0x1b,
        ROM_MBC5_RUMBLE = 0x1c,
        ROM_MBC5_RUMBLE_SRAM = 0x1d,
        ROM_MBC5_RUMBLE_SRAM_BATT = 0x1e,
        Pocket_Camera = 0x1f,
        Bandai_TAMA5 = 0xfd,
        Hudson_HuC3 = 0xfe
    }

    public partial class Apr_GB
    {
        //cpu register & flag
        private enum Flag_Status
        {
            clear = 0,
            set = 1,
        }

        //filter type
        const int filter_scalex = 0;
        const int filter_hqx = 1;
        const int filter_xbrz = 2;

        


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

        //for cgb only
        const ushort reg_SVBK_addr = 0xFF70; //SVBK 

        const ushort reg_KEY1_addr = 0xFF4D; //Prepare Speed Switch

        const ushort reg_BGPI_addr = 0xFF68; //Background Palette Index
        const ushort reg_BGPD_addr = 0xFF69; //Background Palette Data
        const ushort reg_SPI_addr = 0xFF6A; //Sprite Palette Index
        const ushort reg_SPD_addr = 0xFF6B; //Sprite Palette Data

        const ushort reg_VBK_addr = 0xFF4F; //VRAM Bank

        const ushort reg_HDMA1_addr = 0xFF51; //New DMA Sources , Heigh
        const ushort reg_HDMA2_addr = 0xFF52; //New DMA Sources , Low
        const ushort reg_HDMA3_addr = 0xFF53; //New DMA Destination , Heigh
        const ushort reg_HDMA4_addr = 0xFF54; //New DMA Destination , Low
        const ushort reg_HDMA5_addr = 0xFF55; //New DMA Length , Mode , Start

        const ushort reg_UND6C_addr = 0xFF6C;
        const ushort reg_UND72_addr = 0xFF72;
        const ushort reg_UND73_addr = 0xFF73;
        const ushort reg_UND74_addr = 0xFF74;
        const ushort reg_UND75_addr = 0xFF75;
        const ushort reg_UND76_addr = 0xFF76;
        const ushort reg_UND77_addr = 0xFF77;

//--
        //SOUND
        const ushort reg_NR10_addr = 0xFF10;
        const ushort reg_NR11_addr = 0xFF11;
        const ushort reg_NR12_addr = 0xFF12;
        const ushort reg_NR13_addr = 0xFF13;
        const ushort reg_NR14_addr = 0xFF14;

        const ushort reg_NR21_addr = 0xFF16;
        const ushort reg_NR22_addr = 0xFF17;
        const ushort reg_NR23_addr = 0xFF18;
        const ushort reg_NR24_addr = 0xFF19;

        const ushort reg_NR30_addr = 0xFF1A;
        const ushort reg_NR31_addr = 0xFF1B;
        const ushort reg_NR32_addr = 0xFF1C;
        const ushort reg_NR33_addr = 0xFF1D;
        const ushort reg_NR34_addr = 0xFF1E;

        const ushort reg_NR41_addr = 0xFF20;
        const ushort reg_NR42_addr = 0xFF21;
        const ushort reg_NR43_addr = 0xFF22;
        const ushort reg_NR44_addr = 0xFF23;

        // GameBoy bootstrap ROM 
        byte[] BootStrapMen;
        int bootsize = 0;

        const int FlagClear = 0;
        const int FlagSet = 1;

        private static readonly byte[] mCycleTable = new byte[] 
		{
			1, 3, 2, 2, 1, 1, 2, 1, 5, 2, 2, 2, 1, 1, 2, 1,
			1, 3, 2, 2, 1, 1, 2, 1, 3, 2, 2, 2, 1, 1, 2, 1,
			3, 3, 2, 2, 1, 1, 2, 1, 3, 2, 2, 2, 1, 1, 2, 1,
			3, 3, 2, 2, 1, 3, 3, 3, 3, 2, 2, 2, 1, 1, 2, 1,
			1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 2, 1,
			1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 2, 1,
			1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 2, 1,
			2, 2, 2, 2, 2, 2, 1, 2, 1, 1, 1, 1, 1, 1, 2, 1,
			1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 2, 1,
			1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 2, 1,
			1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 2, 1,
			1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 2, 1,
			5, 3, 4, 4, 6, 4, 2, 4, 5, 4, 4, 1, 6, 6, 2, 4,
			5, 3, 4, 0, 6, 4, 2, 4, 5, 4, 4, 0, 6, 0, 2, 4,
			3, 3, 2, 0, 0, 4, 2, 4, 4, 1, 4, 0, 0, 0, 2, 4,
			3, 3, 2, 1, 0, 4, 2, 4, 3, 2, 4, 1, 0, 0, 2, 4,
		};

        private static readonly byte[] cbMCycleTable = new byte[]
		{
			2, 2, 2, 2, 2, 2, 4, 2, 2, 2, 2, 2, 2, 2, 4, 2,
			2, 2, 2, 2, 2, 2, 4, 2, 2, 2, 2, 2, 2, 2, 4, 2,
			2, 2, 2, 2, 2, 2, 4, 2, 2, 2, 2, 2, 2, 2, 4, 2,
			2, 2, 2, 2, 2, 2, 4, 2, 2, 2, 2, 2, 2, 2, 4, 2,
			2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2, 2, 2, 2, 3, 2,
			2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2, 2, 2, 2, 3, 2,
			2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2, 2, 2, 2, 3, 2,
			2, 2, 2, 2, 2, 2, 3, 2, 2, 2, 2, 2, 2, 2, 3, 2,
			2, 2, 2, 2, 2, 2, 4, 2, 2, 2, 2, 2, 2, 2, 4, 2,
			2, 2, 2, 2, 2, 2, 4, 2, 2, 2, 2, 2, 2, 2, 4, 2,
			2, 2, 2, 2, 2, 2, 4, 2, 2, 2, 2, 2, 2, 2, 4, 2,
			2, 2, 2, 2, 2, 2, 4, 2, 2, 2, 2, 2, 2, 2, 4, 2,
			2, 2, 2, 2, 2, 2, 4, 2, 2, 2, 2, 2, 2, 2, 4, 2,
			2, 2, 2, 2, 2, 2, 4, 2, 2, 2, 2, 2, 2, 2, 4, 2,
			2, 2, 2, 2, 2, 2, 4, 2, 2, 2, 2, 2, 2, 2, 4, 2,
			2, 2, 2, 2, 2, 2, 4, 2, 2, 2, 2, 2, 2, 2, 4, 2,
		};
    }
}
