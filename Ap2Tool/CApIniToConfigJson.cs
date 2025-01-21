using ApCommon;
using GenLib;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ap2Tool
{
    internal class CApIniToConfigJson
    {

        public bool ConvertApIniToConfigJson(string strApIniFilePath, string strConfigJsonFilePath,
            out string strErrMsg)
        {
            strErrMsg = string.Empty;

            CApIni apIni;
            if (!LoadApIni(strApIniFilePath, out apIni, out strErrMsg))
                return false;

            CConfig config = new CConfig();
            StringBuilder sbLog = new StringBuilder();
            config.BuildTree(sbLog);

            SetConfig(apIni, config);

            config.SetFileChanged();
            string strJson = CJson.GetJsonStr(config);

            if (File.Exists(strConfigJsonFilePath))
                File.Delete(strConfigJsonFilePath);

            File.WriteAllText(strConfigJsonFilePath, strJson, Encoding.UTF8);

            return true;
        }


        void SetConfig(CApIni apIni, CConfig config)
        {
            #region General -----

            string? strResourceIni = apIni.GetValue("atm-resources_ini");
            if (!string.IsNullOrEmpty(strResourceIni))
            {
                string strFileName = Path.GetFileNameWithoutExtension(strResourceIni);
                string strFileNameNew = strFileName + ".txt";
                config.SetValue(EConfigId.ResourceFileName, strFileNameNew);
            }
            #endregion

            #region Operation -----

            // 자동절단 모드

            EAutoCutMode autoCutMode = EAutoCutMode.None;
            bool? bAutoCut = apIni.GetValueBool("atm-auto_cutting_yn");
            bool? bAutoCutEachOrder = apIni.GetValueBool("atm-auto_cut_each_order_yn");

            if (bAutoCut.HasValue && bAutoCut.Value)
            {
                autoCutMode = (bAutoCutEachOrder.HasValue && bAutoCutEachOrder.Value) ? EAutoCutMode.Order : EAutoCutMode.Batch;
                config.SetValue(EConfigId.AutoCutMode, autoCutMode);
            }

            // 대체카세트 모드

            EAltCstMode altCstMode = EAltCstMode.None;
            bool? bUseAltCst = apIni.GetValueBool("atm-use_alt_cst_yn");
            bool? bAltCstAuto = apIni.GetValueBool("atm-alt_cst_auto_yn");

            if (bUseAltCst.HasValue)
            {
                if (bUseAltCst.Value)
                    altCstMode = (bAltCstAuto.HasValue && bAltCstAuto.Value) ? EAltCstMode.Auto : EAltCstMode.Manual;

                config.SetValue(EConfigId.AltCstMode, altCstMode);
            }

            // 전송순으로 조제

            bool? bKeepOrderSeq = apIni.GetValueBool("atm-keep_order_yn");
            if (bKeepOrderSeq.HasValue)
                config.SetValue(EConfigId.KeepOrderRecvSeq, bKeepOrderSeq.Value);

            // FSP/MDU처방 자동 시작

            bool? bFspMduAutoStart = apIni.GetValueBool("atm-fsp_mdu_auto_start_yn");
            if (bFspMduAutoStart.HasValue)
                config.SetValue(EConfigId.FspMduAutoStart, bFspMduAutoStart.Value);

            // 대기중인 처방이 긴급 FSP/MDU대기중인 처방보다 우선

            bool? bPrecede = apIni.GetValueBool("atm-SB_ORD_PRECEDE_URG_FM_ORD_YN");
            if (bPrecede.HasValue)
                config.SetValue(EConfigId.StandbyOrderPrecedeUrgentFspMduOrder, bPrecede.Value);

            // 진료과별로 조제

            bool? bGroupDptmt = apIni.GetValueBool("atm-KEEP_CLINIC_PART_ORDER_YN");
            if (bGroupDptmt.HasValue)
                config.SetValue(EConfigId.GroupOrdersByDptmt, bGroupDptmt.Value);

            // 진료과별로 공포 삽입

            bool? bDptmtEmptyPack = apIni.GetValueBool("atm-ADD_PACK_BETWEEN_CLINIC_DEPT_YN");
            if (bDptmtEmptyPack.HasValue)
                config.SetValue(EConfigId.DptmtEmptyPackCnt, bDptmtEmptyPack.Value ? "1" : "0");

            // 처방목록 저장

            bool? bSaveOrderList = apIni.GetValueBool("atm-save_order_list_yn");
            if (bSaveOrderList.HasValue)
                config.SetValue(EConfigId.SaveOrderList, bSaveOrderList.Value);

            // 알람음성 출력

            bool? bPlayAlarmSound = apIni.GetValueBool("atm-PLAY_ALARM_VOICE_YN");
            if (bPlayAlarmSound.HasValue)
                config.SetValue(EConfigId.PlayAlarmSound, bPlayAlarmSound.Value);

            #endregion


            #region Server -----

            string? strServerIpAddr = apIni.GetValue("server-ip_addr");
            if (!string.IsNullOrEmpty(strServerIpAddr))
                config.SetValue(EConfigId.AtmsSvrIpAddr, strServerIpAddr);

            #endregion


            #region Machine -----

            // 장비번호

            int? iApNum = apIni.GetValueInt("atm-atm_num");
            if (iApNum.HasValue)
                config.SetValue(EConfigId.ApNum, iApNum);

            // 장비 타입

            //bool? bRduMachine = apIni.GetValueBool("atm-rdu_machine_yn");
            //if (bRduMachine.HasValue)
            //{
            //    EMachineType machineType = EMachineType.ITPS;

            //    if (bRduMachine.Value)
            //        machineType = EMachineType.RDU;

            //    config.SetValue(EConfigId.MachineType, machineType);
            //}

            // 중부 모듈 움직임

            bool? bMidPartMove = apIni.GetValueBool("atm-mid_part_move_yn");
            if (bMidPartMove.HasValue)
                config.SetValue(EConfigId.MiddlePartOpenSensor, bMidPartMove.Value);

            // 하부 모듈 움직임

            bool? bLowPartMove = apIni.GetValueBool("atm-low_part_move_yn");
            if (bLowPartMove.HasValue)
                config.SetValue(EConfigId.LowerPartOpenSensor, bLowPartMove.Value);

            // FSP/MDU 약품이송벨트 제어 모드

            string? strMedBeltCtrlBrd = apIni.GetValue("atm-med_belt_ctrl_brd");
            if (!string.IsNullOrEmpty(strMedBeltCtrlBrd))
            {
                strMedBeltCtrlBrd = strMedBeltCtrlBrd.ToLowerInvariant();

                EMedBeltCtrlMode medBeltCtrlMode = EMedBeltCtrlMode.None;

                if (strMedBeltCtrlBrd == "sen")
                    medBeltCtrlMode = EMedBeltCtrlMode.Sen;

                config.SetValue(EConfigId.MedBeltCtrlMode, medBeltCtrlMode);
            }

            // FSP 약품 이송 벨트

            bool? bFspMedBelt = apIni.GetValueBool("atm-fsp_belt_yn");
            if (bFspMedBelt.HasValue)
                config.SetValue(EConfigId.FspMedBelt, bFspMedBelt.Value);

            // MDU 약품 이송 벨트

            bool? bMduMedBelt = apIni.GetValueBool("atm-mdu_belt_yn");
            if (bMduMedBelt.HasValue)
                config.SetValue(EConfigId.MduMedBelt, bMduMedBelt.Value);

            // LED 전광판 사용

            bool? bUseLedDisp = apIni.GetValueBool("atm-use_dis_yn");
            if (bUseLedDisp.HasValue)
                config.SetValue(EConfigId.LedDispExist, bUseLedDisp.Value);

            #endregion

            #region CassetteBase -----

            bool? bUseRfCst = apIni.GetValueBool("atm-use_rf_cst_yn");
            if (bUseRfCst.HasValue)
            {
                ECassetteRecognitionType casRecogType = ECassetteRecognitionType.Switch;

                if (bUseRfCst.Value)
                    casRecogType = ECassetteRecognitionType.RF;

                config.SetValue(EConfigId.CassetteRecognitionType, casRecogType);
            }

            int? iCstDspsDelayBaseTime = apIni.GetValueInt("atm-MS_MOVE_CST_DSPS_DELAY_BASE_TIME");
            if (iCstDspsDelayBaseTime.HasValue)
            {
                int? iCstDspsDelay = apIni.GetValueInt("atm-MS_MOVE_CST_DSPS_DELAY");
                if (iCstDspsDelay.HasValue)
                {
                    int iCstBefDelayLayer = 1;
                    if (iCstDspsDelay.Value > 300)
                        iCstBefDelayLayer = 2;
                    int iCstBefDelay = iCstDspsDelay.Value;
                    config.SetValue(EConfigId.CstDispenseBeforeDelayLayer, iCstBefDelayLayer);
                    config.SetValue(EConfigId.CstDispenseBeforeDelay, iCstBefDelay);
                }
            }
            #endregion

            #region Drawer Type -----

            bool? bDrawerMove = apIni.GetValueBool("atm-cst_draw_move_yn");

            if (bDrawerMove.HasValue)
            {
                EDrawerType drawerType = EDrawerType.Normal;

                if (bDrawerMove.Value)
                    drawerType = EDrawerType.Normal;
                else
                    drawerType = EDrawerType.None;

                bool? bElecDrawer = apIni.GetValueBool("atm-use_electric_drawer_yn");
                // 전동드로워이면
                if (bElecDrawer.HasValue && bElecDrawer.Value)
                    drawerType = EDrawerType.AD;

                config.SetValue(EConfigId.DrawerType, drawerType);
            }

            int? iDrawerLayerCnt = apIni.GetValueInt("atm-draw_layer_cnt");
            if (iDrawerLayerCnt.HasValue)
                config.SetValue(EConfigId.DrawerLayers, iDrawerLayerCnt.Value);

            int? iDrawerColumnCnt = apIni.GetValueInt("atm-draw_column_cnt");
            if (iDrawerColumnCnt.HasValue)
                config.SetValue(EConfigId.DrawerColumns, iDrawerColumnCnt.Value);

            string? strFixedDrawerList = apIni.GetValue("atm-fixed_draw_list");
            if (!string.IsNullOrEmpty(strFixedDrawerList))
                config.SetValue(EConfigId.DummyDrawers, strFixedDrawerList);

            #endregion

            #region FSP -----

            int? iFspCnt = apIni.GetValueInt("atm-fsp_cnt");
            if (iFspCnt.HasValue)
                config.SetValue(EConfigId.FspCnt, iFspCnt.Value);

            bool? bSmartFsp = apIni.GetValueBool("atm-use_smart_fsp_yn");
            if (bSmartFsp.HasValue)
            {
                EFspType fspType = EFspType.Normal;
                if (bSmartFsp.Value)
                    fspType = EFspType.Smart;
                config.SetValue(EConfigId.FspType, fspType);
            }

            bool? bFspDrawerAuto = apIni.GetValueBool("atm-fsp_draw_auto_yn");
            if (bFspDrawerAuto.HasValue)
                config.SetValue(EConfigId.FspDrawerTypeAuto, bFspDrawerAuto.Value);

            #endregion

            #region MDU -----

            int? iMduCellCnt = apIni.GetValueInt("atm-mdu_cell_cnt");
            bool? bUseDoubleMdu = apIni.GetValueBool("atm-use_double_mdu_yn");
            bool? bUseIndividualMdu = apIni.GetValueBool("atm-use_individual_mdu_yn");
            bool? bUseIdsMdu = apIni.GetValueBool("atm-use_position_mdu_yn");
            bool? bMduTrayAuto = apIni.GetValueBool("atm-mdu_auto_yn");
            bool? bUseRfMdu = apIni.GetValueBool("atm-use_rf_mdu_yn");
            bool? bUseRfOutside = apIni.GetValueBool("atm-use_check_rf_mdu_tag_from_module_outside_yn");

            if (iMduCellCnt.HasValue)
            {
                string strMduCellGrid = "3x8";
                EMduMechType mduMechType = EMduMechType.Screw;

                if (iMduCellCnt == 48)
                {
                    if (bUseDoubleMdu.HasValue && bUseDoubleMdu.Value && 
                        bUseIndividualMdu.HasValue && bUseIndividualMdu.Value)
                        strMduCellGrid = "3x8";
                    else
                        strMduCellGrid = "3x16";
                }
                else if (iMduCellCnt == 60)
                {
                    if (bUseDoubleMdu.HasValue && bUseDoubleMdu.Value &&
                        bUseIndividualMdu.HasValue && bUseIndividualMdu.Value)
                        strMduCellGrid = "3x10";
                    else
                    {
                        strMduCellGrid = "6x10";
                        mduMechType = EMduMechType.Basket;
                    }
                }
                else if (iMduCellCnt == 30)
                    strMduCellGrid = "3x10";
                else if (iMduCellCnt == 24)
                    strMduCellGrid = "3x8";

                int iMduCnt = 1;
                EMduTrayType mduTrayType = EMduTrayType.Auto;

                if (iMduCellCnt > 0)
                {
                    if (bMduTrayAuto.HasValue && bMduTrayAuto.Value)
                        mduTrayType = EMduTrayType.Auto;
                    else
                        mduTrayType = EMduTrayType.Manual;

                    iMduCnt = (bUseDoubleMdu.HasValue && bUseDoubleMdu.Value) ? 2 : 1;
                }
                else
                {
                    mduTrayType = EMduTrayType.None;
                    iMduCnt = 0;
                }

                EMduRfType mduRfType = EMduRfType.None;

                if (bUseRfMdu.HasValue && bUseRfMdu.Value)
                {
                    mduRfType = (bUseRfOutside.HasValue && bUseRfOutside.Value) ? EMduRfType.Outside : EMduRfType.Inside;
                }

                config.SetValue(EConfigId.MduCnt, iMduCnt);
                config.SetValue(EConfigId.MduCellGrid, strMduCellGrid);
                config.SetValue(EConfigId.MduMechType, mduMechType);
                config.SetValue(EConfigId.MduTrayType, mduTrayType);
                config.SetValue(EConfigId.MduRfType, mduRfType);
            }

            #endregion

            #region Middle Shutter -----

            // 중간셔터 약품배출 후 안정화시간

            int? iMSDropDelay = apIni.GetValueInt("atm-ms_drop_delay");
            if (iMSDropDelay.HasValue)
                config.SetValue(EConfigId.MidShutDropStabTime, iMSDropDelay.Value);

            // 중간셔터 약품배출 전 지연시간

            int? iMSBeforeDelay = apIni.GetValueInt("atm-ms_move_delay");
            if (iMSBeforeDelay.HasValue)
                config.SetValue(EConfigId.MidShutMoveBeforeDelay, iMSBeforeDelay.Value);

            // 중간셔터 사용 여부와 제어 모드

            bool? bNoMSExist = apIni.GetValueBool("atm-no_cst_ms_yn");

            if (bNoMSExist.HasValue)
            {
                bool bMSExist = !bNoMSExist.Value;
                bool? bNoMSBoardExist = apIni.GetValueBool("atm-no_ms_brd_yn");
                int? iMSCtrlBrd = apIni.GetValueInt("atm-ms_ctrl_brd");

                EMiddleShutterCtrlMode midShutCtrlMode = EMiddleShutterCtrlMode.MidShut;

                if (bMSExist)
                {
                    if (iMSCtrlBrd > 0)
                    {
                        if (iMSCtrlBrd == 2)
                            midShutCtrlMode = EMiddleShutterCtrlMode.McuMs;
                        else if (iMSCtrlBrd == 3)
                            midShutCtrlMode = EMiddleShutterCtrlMode.McuSeal;
                        else
                            midShutCtrlMode = EMiddleShutterCtrlMode.MidShut;
                    }
                    else
                    {
                        if (bNoMSBoardExist.HasValue && bNoMSBoardExist.Value)
                            midShutCtrlMode = EMiddleShutterCtrlMode.McuSeal;
                        else
                            midShutCtrlMode = EMiddleShutterCtrlMode.MidShut;
                    }
                }
                else
                    midShutCtrlMode = EMiddleShutterCtrlMode.None;

                config.SetValue(EConfigId.MiddleShutterExist, bMSExist);
                config.SetValue(EConfigId.MiddleShutterCtrlMode, midShutCtrlMode);
            }

            #endregion

            #region Communication -----

            int? iCstCommPort = apIni.GetValueInt("atm-cst_comm_port");
            if (iCstCommPort.HasValue)
            {
                string strCommPort = "com" + iCstCommPort.Value;
                config.SetValue(EConfigId.CstCommPortName, strCommPort);
            }

            int? iMcuCommPort = apIni.GetValueInt("atm-mcu_comm_port");
            if (iMcuCommPort.HasValue)
            {
                string strCommPort = "com" + iMcuCommPort.Value;
                config.SetValue(EConfigId.McuCommPortName, strCommPort);
            }

            int? iMcuBaudRate = apIni.GetValueInt("atm-mcu_baud_rate");
            if (iMcuBaudRate.HasValue)
                config.SetValue(EConfigId.McuCommBaudRate, iMcuBaudRate.Value);

            bool? bMcuNewPacket = apIni.GetValueBool("atm-use_mcu_new_packet_yn");
            if (bMcuNewPacket.HasValue)
                config.SetValue(EConfigId.McuCommPacketType, 2);

            int? iPrtCommPort = apIni.GetValueInt("atm-prt_comm_port");
            if (iPrtCommPort.HasValue)
            {
                string strCommPort = "com" + iPrtCommPort.Value;
                config.SetValue(EConfigId.PrtCommPortName, strCommPort);
            }

            int? iPrtBaudRate = apIni.GetValueInt("atm-prt_baud_rate");
            if (iPrtBaudRate.HasValue)
                config.SetValue(EConfigId.PrtCommBaudRate, iPrtBaudRate.Value);

            int? iDisCommPort = apIni.GetValueInt("atm-dis_comm_port");
            if (iDisCommPort.HasValue)
            {
                string strCommPort = "com" + iDisCommPort.Value;
                config.SetValue(EConfigId.DisCommPortName, strCommPort);
            }

            #endregion

            #region Worksheet Printer -----

            bool? bPrinter = apIni.GetValueBool("atm-fsp_printer_yn");
            if (bPrinter.HasValue)
                config.SetValue(EConfigId.WorksheetPrtExist, bPrinter.Value);

            bool? bPrtImageMode = apIni.GetValueBool("atm-fsp_prt_image_yn");
            if (bPrtImageMode.HasValue)
                config.SetValue(EConfigId.PrtImageMode, bPrtImageMode.Value);

            int? iPrtFeedLineCnt = apIni.GetValueInt("atm-fsp_prt_feed_lines");
            if (iPrtFeedLineCnt.HasValue)
                config.SetValue(EConfigId.PrtFeedLineCnt, iPrtFeedLineCnt.Value);

            bool? bPrtImageUserDefine = apIni.GetValueBool("atm-FSP_PRT_IMAGE_UDEFINE_YN");
            if (bPrtImageUserDefine.HasValue)
                config.SetValue(EConfigId.PrtImageUserDefine, bPrtImageUserDefine.Value);

            string? strPrtImageFont = apIni.GetValue("atm-fsp_prt_image_font");
            if (!string.IsNullOrEmpty(strPrtImageFont))
                config.SetValue(EConfigId.PrtImageFont, strPrtImageFont);

            bool? bPrtImageFontBold = apIni.GetValueBool("atm-FSP_PRT_IMAGE_FONT_BOLD_YN");
            if (bPrtImageFontBold.HasValue)
                config.SetValue(EConfigId.PrtImageFontBold, bPrtImageFontBold.Value);

            int? iPrtImageFontWidth = apIni.GetValueInt("atm-FSP_PRT_IMAGE_FONT_SIZE_W");
            if (iPrtImageFontWidth.HasValue)
                config.SetValue(EConfigId.PrtImageFontSizeW, iPrtImageFontWidth.Value);

            int? iPrtImageFontHeight = apIni.GetValueInt("atm-FSP_PRT_IMAGE_FONT_SIZE_H");
            if (iPrtImageFontHeight.HasValue)
                config.SetValue(EConfigId.PrtImageFontSizeH, iPrtImageFontHeight.Value);

            int? iPrtImageLineGap = apIni.GetValueInt("atm-FSP_PRT_IMAGE_LINE_GAP");
            if (iPrtImageLineGap.HasValue)
                config.SetValue(EConfigId.PrtImageLineGap, iPrtImageLineGap.Value);

            #endregion


            #region Technical -----

            // 인쇄-실링간 포수

            int? iOffsetPackCnt = apIni.GetValueInt("atm-offset_pack_cnt");
            if (iOffsetPackCnt.HasValue)
                config.SetValue(EConfigId.PackMoveSealCnt, iOffsetPackCnt.Value);

            // 처방시작시 추가 실링 모드

            string? strAddSeal = apIni.GetValue("atm-add_seal_bef_start");
            if (!string.IsNullOrEmpty(strAddSeal))
            {
                strAddSeal = strAddSeal.ToLowerInvariant();

                EAddSealBeforeStartOrder addSeal = EAddSealBeforeStartOrder.Never;

                if (strAddSeal == "need")
                    addSeal = EAddSealBeforeStartOrder.Need;
                else if (strAddSeal == "always")
                    addSeal = EAddSealBeforeStartOrder.Always;

                config.SetValue(EConfigId.AddSealBeforeStartOrder, addSeal);
            }

            int? iPollAddr = apIni.GetValueInt("atm-cst_poll_addr");
            if (iPollAddr.HasValue)
                config.SetValue(EConfigId.CstPollDevAddr, iPollAddr.Value);

            // 절단 후 첫번째 실링에 대한 포장지 없음 검사 안함
            
            bool? bSkipCheckPackingPaperEmpty = apIni.GetValueBool("atm-skip_paper_chk_first_pack_aft_cut_yn");
            if (bSkipCheckPackingPaperEmpty.HasValue)
                config.SetValue(EConfigId.SkipCheckPackingPaperEmptyOnFirstSealAfterCut, bSkipCheckPackingPaperEmpty.Value);

            // 하무 문 열림 무시

            bool? bIgnoreLowerDoorOpen = apIni.GetValueBool("atm-ignore_door_open_yn");
            if (bIgnoreLowerDoorOpen.HasValue)
                config.SetValue(EConfigId.LowerDoorOpenSensor, !bIgnoreLowerDoorOpen.Value);

            // 하부모듈 탈착 무시

            bool? bIgnoreLowPartOut = apIni.GetValueBool("atm-ignore_low_part_out_yn");
            if (bIgnoreLowPartOut.HasValue)
                config.SetValue(EConfigId.LowerPartOpenSensor, !bIgnoreLowPartOut.Value);

            // 히터 커버 열림 무시

            bool? bIgnoreHeaterCoverOpen = apIni.GetValueBool("atm-ignore_heater_cover_open_yn");
            if (bIgnoreHeaterCoverOpen.HasValue)
                config.SetValue(EConfigId.HeaterCoverSensor, !bIgnoreHeaterCoverOpen.Value);

            // 중부모듈 탈착 무시

            bool? bIgnoreMidPartOut = apIni.GetValueBool("atm-ignore_mid_part_out_yn");
            if (bIgnoreMidPartOut.HasValue)
                config.SetValue(EConfigId.MiddlePartOpenSensor, !bIgnoreMidPartOut.Value);

            // 포장지 없음 무시

            bool? bIgnorePackPaperEmpty = apIni.GetValueBool("atm-ignore_roll_end_yn");
            if (bIgnorePackPaperEmpty.HasValue)
                config.SetValue(EConfigId.IgnorePackingPaperEmpty, bIgnorePackPaperEmpty.Value);

            // 인쇄 리본 없음 무시

            bool? bIgnoreRibbonEmpty = apIni.GetValueBool("atm-ignore_ribbon_end_yn");
            if (bIgnoreRibbonEmpty.HasValue)
                config.SetValue(EConfigId.IgnorePrintRibbonEmpty, bIgnoreRibbonEmpty.Value);

            // 중간셔터 동작 에러 무시

            bool? bIgnoreMidShutMoveError = apIni.GetValueBool("atm-ignore_ms_err_yn");
            if (bIgnoreMidShutMoveError.HasValue)
                config.SetValue(EConfigId.IgnoreMidShutMoveError, bIgnoreMidShutMoveError.Value);

            #endregion

            #region Development -----

            bool? bCommVirtual = apIni.GetValueBool("atm-test_run_yn");
            if (bCommVirtual.HasValue)
            {
                bool? bCstCommVirtual = apIni.GetValueBool("atm-test_run_cst_yn");
                if (bCstCommVirtual.HasValue)
                    config.SetValue(EConfigId.CstCommVirtual, bCstCommVirtual.Value);

                bool? bMcuCommVirtual = apIni.GetValueBool("atm-test_run_mcu_yn");
                if (bMcuCommVirtual.HasValue)
                    config.SetValue(EConfigId.McuCommVirtual, bMcuCommVirtual.Value);

                bool? bPrtCommVirtual = apIni.GetValueBool("atm-test_run_prt_yn");
                if (bPrtCommVirtual.HasValue)
                    config.SetValue(EConfigId.PrtCommVirtual, bPrtCommVirtual.Value);

                bool? bMakePackImgFile = apIni.GetValueBool("atm-MAKE_PACK_IMG_FILE_YN");
                if (bMakePackImgFile.HasValue)
                    config.SetValue(EConfigId.MakePackImgFile, bMakePackImgFile.Value);
            }

            #endregion
        }


        bool LoadApIni(string strApIniFilePath, out CApIni apIni,
            out string strErrMsg)
        {
            strErrMsg = string.Empty;
            apIni = new CApIni();

            if (!File.Exists(strApIniFilePath))
            {
                strErrMsg = "File Not Found!";
                return false;
            }

            string[] saFileLines = File.ReadAllLines(strApIniFilePath, Encoding.UTF8);
            string strSection = string.Empty;

            foreach (string strLine in saFileLines)
            {
                if (string.IsNullOrEmpty(strLine))
                    continue;

                string strLineTrim = strLine.Trim();
                if (strLineTrim.StartsWith('['))
                {
                    int iIdx = strLineTrim.IndexOf(']');
                    if (iIdx > 1)
                    {
                        strSection = strLineTrim.Substring(1, iIdx - 1);
                    }
                }
                else
                {
                    string[] saItems = strLineTrim.Split('=');
                    if (saItems.Length > 1)
                    {
                        string strName = saItems[0].Trim();
                        string strValue = saItems[1].Trim();

                        apIni.AddItem(strSection, strName, strValue);
                    }
                }
            }
            return true;
        }


        class CApIni
        {
            public List<CApIniItem> _items = new List<CApIniItem>();
            public List<CApIniItem> Items { get { return _items; } }

            string MakeKey(string strSection, string strName)
            {
                if (string.IsNullOrEmpty(strSection) || string.IsNullOrEmpty(strName))
                    return string.Empty;

                return strSection.ToLowerInvariant() + "-" + strName.ToLowerInvariant();
            }


            CApIniItem? FindItem(string strKey)
            {
                if (string.IsNullOrEmpty(strKey))
                    return null;

                string strKeyLower = strKey.ToLowerInvariant();
                return _items.Find(a => a.Key == strKeyLower);

            }


            public void AddItem(string strSection, string strName, string strValue)
            {
                string strKey = MakeKey(strSection, strName);
                CApIniItem? item = FindItem(strKey);
                if (item == null)
                {
                    item = new CApIniItem();
                    item.Key = strKey;
                    item.Value = strValue;
                    _items.Add(item);
                }
            }

            public string? GetValue(string strKey)
            {
                var item = FindItem(strKey);
                if (item != null)
                    return item.Value;
                else
                    return null;
            }

            public bool? GetValueBool(string strKey)
            {
                string strVal = GetValue(strKey);
                if (strVal == null) return null;

                return CGenLib.StrToBool(strVal);
            }

            public int? GetValueInt(string strKey)
            {
                string strVal = GetValue(strKey);
                if (strVal == null) return null;

                return CGenLib.StrToInt(strVal);
            }
        }

        class CApIniItem
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }



    }
}
