using System;

namespace Tedd.House1.Client.Esp32
{
    internal static class Constants
    {
        public const byte MemoryAccessOrder_MY  = 0b10000000;
        public const byte MemoryAccessOrder_MX  = 0b01000000;
        public const byte MemoryAccessOrder_MV  = 0b00100000;
        public const byte MemoryAccessOrder_ML  = 0b00010000;
        public const byte MemoryAccessOrder_RGB = 0b00001000;
        public const byte MemoryAccessOrder_BGR = 0b00000000;
        public const byte MemoryAccessOrder_MH  = 0b00000100;

        public const byte MemoryAccessOrder_Normal = 0b00000000;
        public const byte MemoryAccessOrder_UpsideDown = MemoryAccessOrder_MV;
        public const byte MemoryAccessOrder_Mirrored = MemoryAccessOrder_MX;
        public const byte MemoryAccessOrder_UpsideDownMirrored = MemoryAccessOrder_MX | MemoryAccessOrder_MV;
        public const byte MemoryAccessOrder_Rotated = MemoryAccessOrder_MY | MemoryAccessOrder_MV;
        public const byte MemoryAccessOrder_RotatedMirrored = MemoryAccessOrder_MY;
        public const byte MemoryAccessOrder_RotatedUpsideDown = MemoryAccessOrder_MY | MemoryAccessOrder_MX;
        public const byte MemoryAccessOrder_RotatedUpsideDownMirrored = MemoryAccessOrder_MY | MemoryAccessOrder_MX | MemoryAccessOrder_MV;

        public static byte PixelFormat_12Bit = 0b011;
        public static byte PixelFormat_16Bit = 0b101;
        public static byte PixelFormat_18Bit = 0b110;

        /// <summary>
        /// Document referred to https://www.rockbox.org/wiki/pub/Main/SonyNWZE370/ILI9163.pdf
        /// </summary>
        public static class Commands
        {
            /// <summary>No Operation.</summary>
            /// <remarks>0 parameters.</remarks>
            public const byte nop = 0x00;

            /// <summary>Software Reset.</summary>
            /// <remarks>0 parameters.</remarks>
            public const byte soft_reset = 0x01;

            /// <summary>Get the red component of the pixel at (0, 0).</summary>
            /// <remarks>1 parameter.</remarks>
            public const byte get_red_channel = 0x06;

            /// <summary>Get the green component of the pixel at (0, 0).</summary>
            /// <remarks>1 parameter.</remarks>
            public const byte get_green_channel = 0x07;

            /// <summary>Get the blue component of the pixel at (0, 0).</summary>
            /// <remarks>1 parameter.</remarks>
            public const byte get_blue_channel = 0x08;

            /// <summary>Get the current pixel format.</summary>
            /// <remarks>1 parameter.</remarks>
            public const byte get_pixel_format = 0x0C;

            /// <summary>Get the current power mode.</summary>
            /// <remarks>1 parameter.</remarks>
            public const byte get_power_mode = 0x0A;

            /// <summary>Get the frame memory to the display panel read order.</summary>
            /// <remarks>1 parameter.</remarks>
            public const byte get_address_mode = 0x0B;

            /// <summary>Get the current display mode from the peripheral.</summary>
            /// <remarks>1 parameter.</remarks>
            public const byte get_display_mode = 0x0D;

            /// <summary>Get display module signaling mode.</summary>
            /// <remarks>1 parameter.</remarks>
            public const byte get_signal_mode = 0x0E;

            /// <summary>Get Peripheral Self-Diagnostic Result</summary>
            /// <remarks>1 parameter.</remarks>
            public const byte get_diagnostic_result = 0x0F;

            /// <summary>Power for the display panel is off.</summary>
            /// <remarks>0 parameters.</remarks>
            public const byte enter_sleep_mode = 0x10;

            /// <summary>Power for the display panel is on.</summary>
            /// <remarks>0 parameters.</remarks>
            public const byte exit_sleep_mode = 0x11;

            /// <summary>Part of the display area is used for image display.</summary>
            /// <remarks>0 parameters.</remarks>
            public const byte enter_partial_mode = 0x12;

            /// <summary>The whole display area is used for image display.</summary>
            /// <remarks>0 parameters.</remarks>
            public const byte enter_normal_mode = 0x13;

            /// <summary>Displayed image colors are not inverted.</summary>
            /// <remarks>0 parameters.</remarks>
            public const byte exit_invert_mode = 0x20;

            /// <summary>Displayed image colors are inverted.</summary>
            /// <remarks>0 parameters.</remarks>
            public const byte enter_invert_mode = 0x21;

            /// <summary>Selects the gamma curve used by the display device.</summary>
            /// <remarks>1 parameter.</remarks>
            public const byte set_gamma_curve = 0x26;

            /// <summary>Blanks the display device.</summary>
            /// <remarks>0 parameters.</remarks>
            public const byte set_display_off = 0x28;

            /// <summary>Show the image on the display device.</summary>
            /// <remarks>0 parameters.</remarks>
            public const byte set_display_on = 0x29;

            /// <summary>Set the column extent.</summary>
            /// <remarks>4 parameters.</remarks>
            public const byte set_column_address = 0x2A;

            /// <summary>Set the page extent.</summary>
            /// <remarks>4 parameters.</remarks>
            public const byte set_page_address = 0x2B;

            /// <summary>Transfer image data from the Host Processor to the peripheral starting at the location provided by set_column_address and set_page_address.</summary>
            /// <remarks>Variable parameters.</remarks>
            public const byte write_memory_start = 0x2C;

            /// <summary>Fills the peripheral look-up table with the provided data.</summary>
            /// <remarks>Variable parameters.</remarks>
            public const byte write_LUT = 0x2D;

            /// <summary>Transfer image data from the peripheral to the Host Processor interface starting at the location provided by set_column_address and set_page_address.</summary>
            /// <remarks>Variable parameters.</remarks>
            public const byte read_memory_start = 0x2E;

            /// <summary>Defines the partial display area on the display device.</summary>
            /// <remarks>4 parameters.</remarks>
            public const byte set_partial_area = 0x30;

            /// <summary>Defines the vertical scrolling and fixed area on display device.</summary>
            /// <remarks>6 parameters.</remarks>
            public const byte set_scroll_area = 0x33;

            /// <summary>Synchronization information is not sent from the display module to the host processor.</summary>
            /// <remarks>0 parameters.</remarks>
            public const byte set_tear_off = 0x34;

            /// <summary>Synchronization information is sent from the display module to the host processor at the start of VFP.</summary>
            /// <remarks>1 parameter.</remarks>
            public const byte set_tear_on = 0x35;

            /// <summary>Set the read order from frame memory to the display panel.</summary>
            /// <remarks>1 parameter.</remarks>
            public const byte set_address_mode = 0x36;

            /// <summary>Defines the vertical scrolling starting point.</summary>
            /// <remarks>2 parameters.</remarks>
            public const byte set_scroll_start = 0x37;

            /// <summary>Full color depth is used on the display panel.</summary>
            /// <remarks>0 parameters.</remarks>
            public const byte exit_idle_mode = 0x38;

            /// <summary>Reduced color depth is used on the display panel.</summary>
            /// <remarks>0 parameters.</remarks>
            public const byte enter_idle_mode = 0x39;

            /// <summary>Defines how many bits per pixel are used in the interface.</summary>
            /// <remarks>1 parameter.</remarks>
            public const byte set_pixel_format = 0x3A;

            /// <summary>Transfer image information from the Host Processor interface to the peripheral from the last written location.</summary>
            /// <remarks>Variable parameters.</remarks>
            public const byte write_memory_continue = 0x3C;

            /// <summary>Read image data from the peripheral continuing after the last read_memory_continue or read_memory_start.</summary>
            /// <remarks>Variable parameters.</remarks>
            public const byte read_memory_continue = 0x3E;

            /// <summary>Synchronization information is sent from the display module to the host processor when the display device refresh reaches the provided scanline.</summary>
            /// <remarks>2 parameters.</remarks>
            public const byte set_tear_scanline = 0x44;

            /// <summary>Get the current scanline.</summary>
            /// <remarks>2 parameters.</remarks>
            public const byte get_scanline = 0x45;

            /// <summary>Read ID1</summary>
            public const byte ReadID1 = 0xDa;

            /// <summary>Read ID2</summary>
            public const byte ReadID2 = 0xDB;

            /// <summary>Read ID3</summary>
            public const byte ReadID3 = 0xDC;

            /// <summary>
            /// This read byte returns 24-bit display identification information.
            /// The 1st Parameter is dummy read.
            /// The 2nd Parameter (ID17 to ID10): LCD module’s manufacture ID.
            /// The 3rd Parameter(ID27 to ID20): LCD module/driver version ID
            /// The 4th Parameter(ID37 to ID30): LCD module/driver version ID
            /// Note: Commands RDID1/2/3(Dah, DBh, DCh) read data correspond to the parameters 2,3,4 of command 04h, respectively.
            ///
            /// 4 parameters.
            /// 
            /// See page 80.
            /// </summary>
            public const byte Read_Display_Identification_Information = 0x04;
            /// <summary>
            /// This command indicates the current status of the display as described in the table, see page 81.
            ///
            /// 5 parameters.
            ///
            /// See page 81.
            /// </summary>
            public const byte Read_Display_Status = 0x09;
            /// <summary>
            /// This command indicates the current status of the display as described in the table below:
            /// Bit | Description                 | Value
            /// D7  | Booster Voltage Status      | “1”=Booster on, “0”=Booster off
            /// D6  | Idle Mode On/Off            | “1”=Idle Mode On, “0”=Idle Mode Off
            /// D5  | Partial Mode On/Off         | “1”=Partial Mode on, “0”=Partial Mode Off
            /// D4  | Sleep In/Out                | “1”=Sleep Out, “0”=Sleep In
            /// D3  | Display Normal Mode On/Off  | “1”=Normal Display, “0”=Partial Display
            /// D2  | Display On/Off              | “1”=Display On, “0”=Display Off
            /// D1  | Not Defined                 | Set to ‘0’
            /// D0  | Not Defined                 | Set to ‘0’
            ///
            /// 2 parameters.
            /// 
            /// See page 84.
            /// </summary>
            public const byte Read_Display_Power_Mode = 0x0A;
            /// <summary>
            /// This command indicates the current status of the display as described in the table below:
            /// Bit | Description                               | Value
            /// D7  | Page Address Order                        | “1”=Decrement, “0”=Increment
            /// D6  | Column Address Order                      | “1”=Decrement, “0”=Increment
            /// D5  | Page/Column Order                         | “1”=Row/column exchange(MV= 1) “0”=Normal(MV= 0)
            /// D4  | Line Address Order                        | “1”=LCD Refresh Bottom to Top “0”=LCD Refresh Top to Bottom
            /// D3  | RGB/BGR Order                             | “1”=BGR, “0”=RGB
            /// D2  | Display Data Latch Order                  | “1”=LCD Refresh right to left “0”=LCD Refresh left to right
            /// D1  | Switching between Segment outputs and RAM | Set to ‘0’
            /// D0  | Switching between Common outputs and RAM  | Set to ‘0’
            ///
            /// 2 parameters.
            ///
            /// See page 85.
            /// </summary>
            public const byte Read_Display_MADCTL = 0x0B;
            /// <summary>
            /// This command indicates the current status of the display as described in the table below:
            /// Bit                 | Description Value
            /// D3                  |                                  | ”0” (Not used)
            /// IFPF2               | Control Interface Color Format   | “011”=12 bit/pixel
            /// IFPF 1              | Control Interface Color Format   | “101”=16 bit/pixel
            /// IFPF 0              | Control Interface Color Format   | “110”=18 bit/pixel
            ///                     |                                  | The others = not defined
            ///
            /// 2 parameters.
            /// 
            /// See page 86.
            /// </summary>
            public const byte Read_Display_Pixel_Format = 0x0C;
            /// <summary>
            /// Bit | Description                 | Value
            /// D7  | Vertical Scrolling On/Off   | “1”=Vertical scrolling is On, “0”=Vertical scrolling is Off
            /// D6  | Horizontal Scrolling On/Off | “0”(Not used)
            /// D5  | Inversion On/Off            | “1”=Inversion is On, “0”=Inversion is Off
            /// D4  | All Pixels On               | “0” (Not used)
            /// D3  | All Pixel Off               | “0” (Not used)
            /// D2  | Gamma Curve Selection       | “000”=GC0; “001”=GC1; “010”=GC2; “011”=GC3; “100” to “111” = Not defined
            /// D1  | Gamma Curve Selection       | 
            /// D0  | Gamma Curve Selection       |
            ///
            /// 2 parameters.
            /// 
            /// See page 87.
            /// </summary>
            public const byte Read_Display_Image_Mode = 0x0D;
            /// <summary>
            /// This command indicates the current status of the display as described in the table below:
            /// Bit | Description                 | Value
            /// D7  | Tearing Effect Line On/Off  | “1”=On, “0”=Off
            /// D6  | Tearing Effect Line Mode    | “0”=mode1, “1”=mode2
            /// D1  | Not Used                    | “1”=On, “0”=Off
            /// D0  | Not Used                    | “1”=On, “0”=Off
            ///
            /// 2 parameters.
            /// 
            /// See page 88.
            /// </summary>
            public const byte Read_Display_Signal_Mode1 = 0x0E;
            /// <summary>
            /// This command indicates the current status of the display as described in the table below:
            /// Bit | Description                | Value
            /// D7  | Register Loading Detection |
            /// D6  | Functionality Detection    |
            /// D5  | Not Used                   | “0”
            /// D4  | Not Used                   | “0”
            /// D3  | Not Used                   | “0”
            /// D2  | Not Used                   | “0”
            /// D1  | Not Used                   | “0”
            /// D0  | Not Used                   | “0”
            ///
            /// 2 parameters.
            /// 
            /// See page 89.
            /// </summary>
            public const byte Read_Display_Signal_Mode2 = 0x0F;
            /// <summary>
            /// This command causes the LCD module to enter the minimum power consumption mode.
            /// In this mode e.g.the DC/DC converter is stopped, Internal oscillator is stopped, and panel scanning is stopped.
            /// MCU interface and memory are still working and the memory keeps its contents.
            /// 
            /// No parameters.
            /// 
            /// See page 90.
            /// </summary>
            /// <remarks>
            /// This command has no effect when module is already in sleep in mode. Sleep In Mode can only be left by the Sleep Out Command(11h). It will be necessary to wait 5msec before sending next command; this is to allow time for the supply voltages and clock circuits to stabilize.It will be necessary to wait 120msec after sending Sleep Out command (when in Sleep In Mode) before Sleep In command can be sent.
            /// </remarks>
            public const byte Sleep_In = 0x10;
            /// <summary>
            /// This command turns off sleep mode.
            /// In this mode e.g.the DC/DC converter is enabled, Internal oscillator is started, and panel scanning is started.
            /// 
            /// No parameters.
            /// 
            /// See page 91.
            /// </summary>
            /// <remarks>
            /// This command has no effect when module is already in sleep out mode. Sleep
            /// Out Mode can only be left by the Sleep In Command(10h).
            /// It will be necessary to wait 5msec before sending next command; this is to allow time for the supply voltages and clock circuits to stabilize.
            /// The display module loads all display supplier’s factory default values to the registers during this 5msec and there cannot be any abnormal visual effect on the display image if factory default and register values are same when this load is done and when the display module is already Sleep Out –mode.
            /// The display module is doing self-diagnostic functions during this 5msec.It will be necessary to wait 120msec after sending Sleep In command (when in Sleep Out mode) before Sleep Out command can be sent.
            /// This command has no effect when module is already in sleep out mode.
            /// Sleep Out Mode can only be left by HW Reset, Software Reset(01h), Sleep In(10h), or a NMI event trigger. 
            /// </remarks>
            public const byte Sleep_Out = 0x11;
            /// <summary>
            /// This command turns on partial mode. The partial mode is described by the Partial Area command (30h).
            /// To leave Partial mode, the Normal Display On command(13h) should be written.
            /// X = Don’t care
            /// Note: If a command is written in a frame cycle, the command becomes effective from the next frame.
            /// 
            /// No parameters.
            /// 
            /// See page 93.
            /// </summary>
            /// <remarks>This command has no effect during Partial mode is active.</remarks>
            public const byte Partial_Mode_On = 0x12;
            /// <summary>
            ///This command returns the display to normal mode.
            /// Normal display mode on means Partial mode off and Scroll mode Off.
            /// Exit from NORON by the Partial mode On command(12h)
            /// X = Don’t care
            /// Note: If a command is written in a frame cycle, the command becomes effective from the next frame.
            /// 
            /// No parameters.
            /// 
            /// See page 94.
            /// </summary>
            /// <remarks>This command has no effect when Normal Display mode is active.</remarks>
            public const byte Normal_Display_Mode_On = 0x13;

            /// <summary>
            /// This command is used to recover from display inversion mode.
            /// This command makes no change of contents of frame memory.
            /// This command does not change any other status.
            /// Memory Display Panel
            /// X = don’t care
            /// No parameters.
            ///
            /// See page 95.
            /// </summary>
            /// <remarks>This command has no effect when module is already in inversion off mode.</remarks>
            public const byte Display_Inversion_Off = 0x20;
            /// <summary>
            /// This command is used to enter into display inversion mode.
            /// This command makes no change of contents of frame memory.Every bit is inverted from the frame memory to the display.
            /// This command does not change any other status.
            /// To exit from Display inversion On, the Display Inversion Off command(20h) should be written.
            /// Memory Display Panel
            /// X = don’t care
            ///
            /// No parameters.
            ///
            /// See page 96.
            /// </summary>
            /// <remarks>This command has no effect when module is already in inversion on mode.</remarks>
            public const byte Display_Inversion_On = 0x21;
            /// <summary>
            /// This command is used to select the desired Gamma curve for the current display. A maximum of 4 fixed gamma curves can be selected.The curves are defined Gamma Curve Correction Power Supply Circuit.The curve is selected by setting the appropriate bit in the parameter as described in the Table:
            /// GC[7..0] | Parameter | Curve Selected
            /// 01h      | GC0       | Gamma Curve 1
            /// 02h      | GC1       | Gamma Curve 2
            /// 04h      | GC2       | Gamma Curve 3
            /// 08h      | GC3       | Gamma Curve 4
            /// Note: All other values are undefined.
            /// X = don’t care
            ///
            /// 1 parameter.
            ///
            /// See page 97.
            /// </summary>
            /// <remarks>Values of GC[7..0] not shown in table above are invalid and will not change the current selected Gamma curve until valid value is received.</remarks>
            public const byte Gamma_Set = 0x26;
            /// <summary>
            /// This command is used to enter into DISPLAY OFF mode. In this mode, the output from Frame Memory is disabled and blank page inserted.
            /// This command makes no change of contents of frame memory.
            /// This command does not change any other status.
            /// There will be no abnormal visible effect on the display.
            /// Exit from this command by Display On(29h)
            /// 
            /// No parameters.
            ///
            /// See page 98.
            /// </summary>
            /// <remarks>This command has no effect when module is already in display off mode.</remarks>
            public const byte Display_Off = 0x28;
            // To be continued
            public const byte Display_On = 0x29;

            /// <summary>
            /// 
            /// </summary>
            public const byte Power_Control1 = 0xC0;
            public const byte Power_Control2 = 0xC1;
            /// <summary>Frame Rate Control(In normal mode/Full colors)</summary>
            public const byte FrameRateControl = 0xB1;
            public const byte VCOM_Control = 0xC5;
            public const byte Display_Inversion_Control = 0xB4;
            public const byte GAM_R_SEL = 0xF2;
            public const byte Positive_Gamma_Correction_Setting = 0xE0;
            public const byte Negative_Gamma_Correction_Setting = 0xE1;

            public const byte Memory_Access_Control = 0x36;
        }




    }
}