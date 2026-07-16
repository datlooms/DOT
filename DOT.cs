//+------------------------------------------------------------------+
//|                           LWA.mq4                                |
//|         Eliminating the Unknown via Childish Artistry            |
//|                  by Ticky and the Animal                         |
//+------------------------------------------------------------------+
#property strict
#define DOTS_NUM_RULES 76
#define THR_HI 0
#define THR_LO 1
#define FEAT_ATR_1M              0
#define FEAT_Bar_Range           1
#define FEAT_D2D_ATR             2
#define FEAT_D2D_ATR_MA          3
#define FEAT_D2D_Dn_Count        4
#define FEAT_D2D_Dynamic_Sensitivity 5
#define FEAT_D2D_Persist         6
#define FEAT_D2D_Up_Count        7
#define FEAT_AT_Lookback_LT      8
#define FEAT_AT_Lookback_ST      9
#define FEAT_AT_Score_LT         10
#define FEAT_AT_Score_ST         11
#define FEAT_AT_Slope_LT         12
#define FEAT_AT_Slope_ST         13
#define FEAT_Bars_Since_Flip     14
#define FEAT_Slope_EMA_LT        15
#define FEAT_Slope_EMA_ST        16
#define FEAT_Slope_Accel_LT      17
#define FEAT_Slope_Accel_ST      18
#define FEAT_OBV_Macd            19
#define FEAT_OBV_Velocity        20
#define FEAT_OBVf_DirStepCount   21
#define FEAT_KAMA_Dist           22
#define FEAT_KAMA_Dist_ATR       23
#define FEAT_KAMA_Slope          24
#define FEAT_EMA_Oscillator      25
#define FEAT_Harmonic_LLEMA      26
#define FEAT_Sqz_Val             27
#define FEAT_RangeOsc_Val        28
#define FEAT_Volume_Avg_10       29
#define FEAT_Volume_Ratio_10     30
#define FEAT_Momentum_Value      31
#define FEAT_Efficiency_Ratio    32
#define FEAT_Dist_To_PoC_ATR     33
#define FEAT_Micro_Amihud        34
#define FEAT_Micro_AutoCorr      35
#define FEAT_Micro_BarEntropy    36
#define FEAT_Micro_BarOverlap    37
#define FEAT_Micro_CSSpread      38
#define FEAT_Micro_Entropy       39
#define FEAT_Micro_FailedBreak   40
#define FEAT_Micro_FractalDim    41
#define FEAT_Micro_GarmanKlass   42
#define FEAT_Micro_HLAsymmetry   43
#define FEAT_Micro_Hurst         44
#define FEAT_Micro_IBSP          45
#define FEAT_Micro_Lambda        46
#define FEAT_Micro_LogReturn     47
#define FEAT_Micro_MicroGap      48
#define FEAT_Micro_MomoTransfer  49
#define FEAT_Micro_OrderFlowDelta 50
#define FEAT_Micro_PriceAccel    51
#define FEAT_Micro_RangeAccel    52
#define FEAT_Micro_RangeVelocity 53
#define FEAT_Micro_Rejection     54
#define FEAT_Micro_RollProxy     55
#define FEAT_Micro_ThrustEff     56
#define FEAT_Micro_TickIntensity 57
#define FEAT_Micro_VPIN          58
#define FEAT_Micro_VolAccel      59
#define FEAT_Micro_VolOfVol      60
#define FEAT_Micro_WickImbalance 61
#define FEAT_VWAP_Dist_ATR       62
#define FEAT_VAH_Dist_ATR        63
#define FEAT_VAL_Dist_ATR        64
#define FEAT_PrevDay_High_Dist_ATR 65
#define FEAT_PrevDay_Low_Dist_ATR 66
#define FEAT_PrevDay_Close_Dist_ATR 67
#define FEAT_DailyOpen_Dist_ATR  68
#define FEAT_Round_100_Dist_ATR  69
#define FEAT_Round_500_Dist_ATR  70
#define FEAT_Round_1000_Dist_ATR 71
#define FEAT_OR_High_Dist_ATR    72
#define FEAT_OR_Low_Dist_ATR     73
#define FEAT_Session_High_Dist_ATR 74
#define FEAT_Session_Low_Dist_ATR 75
#define FEAT_WeeklyOpen_Dist_ATR 76
#define FEAT_MultiDay_Slope      77
#define FEAT_MultiDay_Position   78
#define FEAT_VWAP_Sigma_ATR      79
#define FEAT_VA_Position         80
#define FEAT_VWAP_Z              81
#define FEAT_OR_Position         82
#define FEAT_ADX_Value           83
#define FEAT_Body_Size           84
#define FEAT_Upper_Wick          85
#define FEAT_Lower_Wick          86
#define FEAT_TChan_A15           87
#define FEAT_VWAP_Sigma          88
#define FEAT_Volume              89
#define DOTS_NUM_FEATURES        90
//+------------------------------------------------------------------+
//| SECTION 1.0 - INITIALISATION                                     |
//+------------------------------------------------------------------+
string boot_log_lines[65];
int boot_log_counter=0;
void LogBootMessage(string msg) {
   string labelPrefix=ea_prefix+"BootLog_";
   if(boot_log_counter<65) {
      boot_log_lines[boot_log_counter]=msg;
      boot_log_counter++;
   } else {
      for(int i=0; i<64; i++) boot_log_lines[i]=boot_log_lines[i+1];
      boot_log_lines[64]=msg;
   }
   DotsLog("BOOT: "+msg);
   int lines_to_draw=(boot_log_counter>65)?65:boot_log_counter;
   int row_height=15; int padding=10;
   int dynamic_height=(lines_to_draw*row_height)+padding;
   int chartHeight=(int)ChartGetInteger(0,CHART_HEIGHT_IN_PIXELS);
   int panelY=80; int maxHeight=chartHeight-panelY-20;
   if(dynamic_height>maxHeight) dynamic_height=maxHeight;
   if(dynamic_height<30) dynamic_height=30;
   string bgName=ea_prefix+"BootPanelBG";
   if(ObjectFind(0,bgName)>=0) ObjectSetInteger(0,bgName,OBJPROP_YSIZE,dynamic_height);
   for(int i=0; i<65; i++) {
      string objName=labelPrefix+IntegerToString(i);
      if(ObjectFind(0,objName)>=0) ObjectSetString(0,objName,OBJPROP_TEXT,boot_log_lines[i]);
   }
   ChartRedraw();
}
void DrawBootConsole() {
   int chartWidth=(int)ChartGetInteger(0,CHART_WIDTH_IN_PIXELS);
   int chartHeight=(int)ChartGetInteger(0,CHART_HEIGHT_IN_PIXELS);
   int panelW=400; int panelY=80;
   int lines_to_draw=(boot_log_counter>65)?65:boot_log_counter;
   int initialH=(lines_to_draw*15)+10;
   if(initialH<30) initialH=30;
   int panelX=chartWidth-panelW-15;
   string bgName=ea_prefix+"BootPanelBG";
   if(ObjectFind(0,bgName)<0) {
      ObjectCreate(0,bgName,OBJ_RECTANGLE_LABEL,0,0,0);
      ObjectSetInteger(0,bgName,OBJPROP_CORNER,CORNER_LEFT_UPPER);
      ObjectSetInteger(0,bgName,OBJPROP_XDISTANCE,panelX);
      ObjectSetInteger(0,bgName,OBJPROP_YDISTANCE,panelY);
      ObjectSetInteger(0,bgName,OBJPROP_XSIZE,panelW);
      ObjectSetInteger(0,bgName,OBJPROP_YSIZE,initialH);
      ObjectSetInteger(0,bgName,OBJPROP_BGCOLOR,C'19,29,42');
      ObjectSetInteger(0,bgName,OBJPROP_BORDER_COLOR,C'95,107,119');
      ObjectSetInteger(0,bgName,OBJPROP_BORDER_TYPE,0);
      ObjectSetInteger(0,bgName,OBJPROP_BACK,false);
      ObjectSetInteger(0,bgName,OBJPROP_ZORDER,40005);
   }
   for(int i=0; i<65; i++) {
      string objName=ea_prefix+"BootLog_"+IntegerToString(i);
      if(ObjectFind(0,objName)<0) {
         ObjectCreate(0,objName,OBJ_LABEL,0,0,0);
         ObjectSetInteger(0,objName,OBJPROP_CORNER,CORNER_LEFT_UPPER);
         ObjectSetString(0,objName,OBJPROP_FONT,"Consolas");
         ObjectSetInteger(0,objName,OBJPROP_FONTSIZE,8);
         ObjectSetInteger(0,objName,OBJPROP_ANCHOR,ANCHOR_LEFT_UPPER);
         ObjectSetInteger(0,objName,OBJPROP_XDISTANCE,panelX+5);
         ObjectSetInteger(0,objName,OBJPROP_YDISTANCE,panelY+5+(i*15));
         ObjectSetInteger(0,objName,OBJPROP_COLOR,C'146,134,124');
         ObjectSetInteger(0,objName,OBJPROP_ZORDER,40006);
      }
   }
}
void RemoveBootConsole() {
   ObjectDelete(0,ea_prefix+"BootPanelBG");
   for(int i=0; i<65; i++) ObjectDelete(0,ea_prefix+"BootLog_"+IntegerToString(i));
}
string _LongToString_Legacy(long number) {
   string s=""; if(number==0) return "0";
   bool is_neg=false; if(number<0) { is_neg=true; number=-number; }
   while(number>0) { s=StringFormat("%ld",number%10)+s; number=number/10; }
   if(is_neg) s="-"+s; return s;
}
void SquashChartToMiddleThird(long chart_ID) {
   int first_visible_bar=WindowFirstVisibleBar();
   int visible_bars_count=WindowBarsPerChart();
   int start_bar=(int)MathMax((double)(first_visible_bar-visible_bars_count+1),0.0);
   int highest_idx=iHighest(NULL,0,MODE_HIGH,visible_bars_count,start_bar);
   int lowest_idx=iLowest(NULL,0,MODE_LOW,visible_bars_count,start_bar);
   if(highest_idx>=0&&lowest_idx>=0) {
      double max_price=High[highest_idx];
      double min_price=Low[lowest_idx];
      double range=max_price-min_price;
      double new_max=max_price+(range*(1.0/6.0));
      double new_min=min_price-(range*(11.0/18.0));
      ChartSetInteger(chart_ID,CHART_SCALEFIX,true);
      ChartSetDouble(chart_ID,CHART_FIXED_MAX,new_max);
      ChartSetDouble(chart_ID,CHART_FIXED_MIN,new_min);
   }
}
int OnInit() {
   g_isLoading=true;
   g_loadingStartTime=GetTickCount();
   initialPaintComplete=false;
   lastCalculatedBars=0;
   g_loadingProgress=0.0;
   boot_log_counter=0;
   for(int i=0; i<65; i++) boot_log_lines[i]="";
   long chartID=ChartID();
   string chartID_str=_LongToString_Legacy(chartID);
   string chartID_suffix=StringSubstr(chartID_str,(int)MathMax(0.0,(double)(StringLen(chartID_str)-3)));
   long temp_id=StringToInteger(chartID_suffix);
   int short_unique_id=(int)temp_id;
   MagicNumber=short_unique_id;
   ManualMagicNumber=short_unique_id+1000000;
   OBVfriendMagicNumber=short_unique_id+2000000;
   DotsMagicNumber=short_unique_id+3000000;
   EA_ID=MagicNumber;
   ea_prefix=Symbol()+"_"+IntegerToString(EA_ID)+"_";
   DrawLoadingBar(g_loadingProgress,"Boot Sequence...");
   DrawBootConsole();
   LogBootMessage("=== LOOMS SYSTEM BOOT ===");
   LogBootMessage("Core: Initializing ID "+IntegerToString(EA_ID));
   ChartRedraw();
   showOBVfCandles=true;
   isSuperTrendVisible=false;
   isOBVfriendSuperTrendVisible=true;
   isHarmonicVolVisible=ShowHarmonicVolumeCandles;
   isDotsVisualsVisible=true;
   if(chartID!=0) {
      ChartSetInteger(chartID,CHART_MODE,CHART_LINE);
      ChartSetInteger(chartID,CHART_COLOR_CHART_LINE,clrNONE);
      ChartSetInteger(chartID,CHART_COLOR_CHART_UP,clrNONE);
      ChartSetInteger(chartID,CHART_COLOR_CHART_DOWN,clrNONE);
      ChartSetInteger(chartID,CHART_COLOR_CANDLE_BULL,clrNONE);
      ChartSetInteger(chartID,CHART_COLOR_CANDLE_BEAR,clrNONE);
      ChartSetInteger(chartID,CHART_SHOW_GRID,false);
      ChartSetInteger(chartID,CHART_COLOR_BACKGROUND,C'19,29,42');
      ChartSetInteger(chartID,CHART_COLOR_FOREGROUND,C'96,95,113');
      ChartSetInteger(chartID,CHART_COLOR_GRID,C'96,95,113');
      ChartSetInteger(chartID,CHART_COLOR_VOLUME,C'146,134,124');
      ChartSetInteger(chartID,CHART_COLOR_ASK,C'89,116,124');
      ChartSetInteger(chartID,CHART_COLOR_STOP_LEVEL,C'89,116,124');
      ChartSetInteger(chartID,CHART_AUTOSCROLL,true);
      ChartSetInteger(chartID,CHART_SHIFT,true);
      ChartSetInteger(chartID,CHART_SCALE,3);
      ChartRedraw();
      LogBootMessage("Chart: Colors & Props Applied.");
   } else {
      Print("Warning: Could not get chart ID to set chart colors.");
      LogBootMessage("Chart: Color Setup Failed.");
   }
   LogBootMessage("Settings: Importing .set file...");
   LogBootMessage(" > BaseLotSize: "+DoubleToString(BaseLotSize,2));
   LogBootMessage(" > D2D Logic: "+(isLoomsActive?"ENABLED":"DISABLED"));
   LogBootMessage(" > OBVfriend Logic: "+(UseOBVfriend?"ENABLED":"DISABLED"));
   LogBootMessage(" > DOTS Engine: "+(UseDots?"LOADED":"DISABLED"));
   LogBootMessage(" > DOTS Trading: INACTIVE (activate via panel)");
   lastCommittedSignal=0;
   lastCommittedSignalTime=0;
   d2d_lastTradedSignalTime=0;
   obvfriend_lastTradedSignalTime=0;
   for(int d=0;d<DOTS_NUM_RULES;d++) {
      dots_state[d].ruleIndex=d;
      dots_state[d].ticket=0;
      dots_state[d].direction=0;
      dots_state[d].entryPrice=0.0;
      dots_state[d].initialRisk=0.0;
      dots_state[d].stepSize=0.0;
      dots_state[d].currentSL=0.0;
      dots_state[d].be_trigger=0.0;
      dots_state[d].be_lock_dist=0.0;
      dots_state[d].tiersReached=0;
      dots_state[d].beNudged=false;
      dots_state[d].condA=false;
      dots_state[d].condB=false;
      dots_state[d].condC=false;
      dots_ruleName[d]="R"+IntegerToString(d);
      dots_ruleDir[d]=0;
      dots_lastAlertTime[d]=0;
      dots_rule_wins[d]=0;
      dots_rule_losses[d]=0;
      dots_rule_pnl[d]=0.0;
   }
  isDotsTradeActive=IsTesting();
   dots_today_wins=0;
   dots_today_losses=0;
   dots_today_pnl=0.0;
   dots_today_sl=0;
   dots_today_feat=0;
   dots_today_time=0;
   dots_total_trades=0;
   dots_active_count=0;
   for(int ot=OrdersTotal()-1;ot>=0;ot--) {
      if(!OrderSelect(ot,SELECT_BY_POS,MODE_TRADES)) continue;
      if(OrderSymbol()!=Symbol()) continue;
      if(OrderMagicNumber()!=DotsMagicNumber) continue;
      string oComment=OrderComment();
      int rPos=StringFind(oComment,"DOTS_R");
      if(rPos<0) continue;
      string after=StringSubstr(oComment,rPos+6);
      int usPos=StringFind(after,"_");
      if(usPos<=0) continue;
      string idxStr=StringSubstr(after,0,usPos);
      int idx=(int)StringToInteger(idxStr);
      if(idx<0||idx>=DOTS_NUM_RULES) continue;
      if(dots_state[idx].ticket>0) continue;
      int dir=(OrderType()==OP_BUY)?1:-1;
      double entry=OrderOpenPrice();
      double sl=OrderStopLoss();
      double atr=(1<ArraySize(ATR_1M_Array))?ATR_1M_Array[1]:15.0;
      double risk=MathMin(atr*Dots_SL_Mult,Dots_SL_Cap);
      if(risk<=0.0) risk=30.0;
      double step=Dots_StepFrac*risk;
      double beTrig=Dots_BE_Trigger*step;
      double beLock=Dots_BE_LockFrac*beTrig;
      double rawSL;
      if(dir==1) rawSL=entry-risk*Point;
      else rawSL=entry+risk*Point;
      bool nudged=(MathAbs(sl-rawSL)/Point>1.0);
      int tiers=0;
      if(step>0.0) {
         double fav;
         if(dir==1) fav=(Bid-entry)/Point;
         else fav=(entry-Ask)/Point;
         tiers=(int)MathFloor(fav/step);
         if(tiers<0) tiers=0;
      }
      dots_state[idx].ruleIndex=idx;
      dots_state[idx].ticket=OrderTicket();
      dots_state[idx].direction=dir;
      dots_state[idx].entryPrice=entry;
      dots_state[idx].initialRisk=risk;
      dots_state[idx].stepSize=step;
      dots_state[idx].currentSL=sl;
      dots_state[idx].be_trigger=beTrig;
      dots_state[idx].be_lock_dist=beLock;
      dots_state[idx].tiersReached=tiers;
      dots_state[idx].beNudged=nudged;
      dots_state[idx].condA=false;
      dots_state[idx].condB=false;
      dots_state[idx].condC=false;
      dots_active_count++;
      dots_total_trades++;
      Print("DOTS| Recovery R",idx,
            " ",((dir==1)?"LONG":"SHORT"),
            " Ticket=",OrderTicket(),
            " Entry=",DoubleToString(entry,Digits),
            " SL=",DoubleToString(sl,Digits),
            " Risk~",DoubleToString(risk,1),
            " Tiers~",tiers,
            " BE=",((nudged)?"Y":"N"));
   }
   if(dots_active_count>0)
      LogBootMessage("DOTS: Recovered "+IntegerToString(dots_active_count)+" open positions.");
   Print("DOTS| Engine loaded. Trading INACTIVE — activate via panel button.");
   int current_bars=iBars(Symbol(),Period());
   LogBootMessage("Data: Syncing "+IntegerToString(current_bars)+" bars from server...");
   RefreshRates();
   current_bars=iBars(Symbol(),Period());
   LogBootMessage("Data: History Sync Final: "+IntegerToString(current_bars)+" bars.");
   if(current_bars<ClusteringLookback+trainingBars) {
      Print("Not enough bars on chart to initialise EA. Bars: ",current_bars);
      LogBootMessage("CRITICAL: Not enough bars.");
      g_isLoading=false;
      RemoveLoadingBar();
      return (INIT_FAILED);
   }
   LogBootMessage("Memory: Initializing Stateful Chain...");
   g_loadingProgress=0.1;
   DrawLoadingBar(g_loadingProgress,"Building History State...");
   LogBootMessage("Memory: Resizing State Buffers...");
   ResizeAllArrays(current_bars);
   LogBootMessage("Memory: Buffers Ready. Calc TR Cap...");
   ChartRedraw();
   CalculateTrueRangeCap();
   int startBar=current_bars-1;
   if(startBar<1) startBar=1;
   LogBootMessage("Pass 1: Locking Historical Inputs...");
   for(int i=startBar; i>=0; i--) {
      hist_VolumeValue[i]=(double)Volume[i];
      Calc_PoC_State_OnBar(i);
      Calc_ADX_OnBar(i);
      Calc_OBV_OnBar(i);
      Calc_Momentum_OnBar(i);
      CalcATR1M(i,atrPeriod);
      if(i%1000==0) {
         LogBootMessage("Pass 1: Bar "+IntegerToString(i));
         g_loadingProgress=0.1+((double)(startBar-i)/startBar)*0.2;
         DrawLoadingBar(g_loadingProgress,"Locking States...");
         ChartRedraw();
      }
   }
   g_loadingProgress=0.3;
   DrawLoadingBar(g_loadingProgress,"Initializing Normalizers...");
   LogBootMessage("Norms: Init ADX Classifier...");
   ChartRedraw();
   InitADXClassifier();
   LogBootMessage("Norms: Init Momentum Norm...");
   InitMomentumNormalizer();
   ReadKamaWarmState();
   ValidateKamaWarmState();
   LogBootMessage("Pass 2: Engine Calculation Loop...");
   for(int i=startBar; i>=1; i--) {
      Calc_D2D_ST_OnBar(i);
      Calc_AdaptiveTrend_OnBar(i);
      Calc_OBVfriend_ST_OnBar(i);
      Calc_HarmVol_LLEMA_OnBar(i);
      Calc_Sqz_Momentum_OnBar(i);
      Calc_RangeOsc_OnBar(i);
      Calc_Microstructure_OnBar(i);
      Calc_Dots_Derived_OnBar(i);
      Calc_VWAP_OnBar(i);
      Calc_RefLevels_OnBar(i);
      Calc_MultiDay_OnBar(i);
      if(LockBuffer[i]!=0 && LockTime[i]==0) { LockTime[i]=Time[i]; }
      if(i%1000==0) {
         LogBootMessage("Pass 2: Bar "+IntegerToString(i));
         g_loadingProgress=0.3+((double)(startBar-i)/startBar)*0.3;
         DrawLoadingBar(g_loadingProgress,"Calculating Strategy...");
         ChartRedraw();
      }
   }
   LogBootMessage("Engine: History Calculation Complete.");
   if(UseDots) {
      InitDotsThresholds();
      InitDotsRuleTable();
      for(int dr=0;dr<DOTS_NUM_RULES;dr++)
         dots_ruleDir[dr]=dots_rules[dr].direction;
      InitDotsSignalNames();
      SeedDotsRollingBuffers();
      if(Dots_ExportThresholds) ExportDotsThresholdSnapshots();
   }
   LogBootMessage("DOTS: Thresholds, Rules, and Buffers Initialized.");
   int newestSignalDir=0;
   datetime newestSignalTime=0;
   int newestSignalIdx=-1;
   int scanStart=(int)MathMin((double)(current_bars-1),1000.0);
   LogBootMessage("Logic: Scanning for latest signal...");
   for(int i=scanStart; i>=1; i--) {
      if(LockBuffer[i]!=0) {
         if(Time[i]>newestSignalTime) {
            newestSignalDir=LockBuffer[i];
            newestSignalTime=Time[i];
            newestSignalIdx=i;
         }
      }
   }
   if(newestSignalIdx!=-1) {
      lastCommittedSignal=newestSignalDir;
      lastCommittedSignalTime=newestSignalTime;
      lastCommittedSignalIndex=newestSignalIdx;
      lastSignalPrice=Close[newestSignalIdx];
      LogBootMessage("Logic: Last Signal Found @ "+TimeToString(newestSignalTime));
   } else {
      LogBootMessage("Logic: No recent signals found.");
   }
   lastCalculatedBars=current_bars;
   g_loadingProgress=0.8;
   DrawLoadingBar(g_loadingProgress,"Drawing Indicators...");
   LogBootMessage("Visuals: Painting Trend Lines...");
   ChartRedraw();
   DrawHistoricalIndicators_FromBuffers();
   ColorSignalBars();
   if(isSuperTrendVisible) {
      LogBootMessage("Visuals: Drawing D2D SuperTrend...");
      DrawSuperTrendLine();
   }
   if(isOBVfriendSuperTrendVisible) {
      LogBootMessage("Visuals: Drawing OBVfriend SuperTrend...");
      DrawOBVfriendSuperTrendLine();
   }
   if(isSignalDotsVisible) {
      LogBootMessage("Visuals: Placing Signal Markers...");
      for(int i=current_bars-1; i>=1; i--) {
         if(LockBuffer[i]!=0) {
            color signalColor=(LockBuffer[i]==1)?C'146,134,124':C'89,116,124';
            DrawSignalOnChart(Time[i],signalColor);
         }
      }
   }
   initialPaintComplete=true;
   d2d_lastTradedSignalTime=lastCommittedSignalTime;
   obvfriend_lastTradedSignalTime=lastCommittedSignalTime;
   Print("Initial scan complete.");
   LogBootMessage("Visuals: Calculating PoC Grid...");
   CalculateDailyPoC();
   LogBootMessage("Visuals: Drawing PoC...");
   DrawDailyPoC();
   LogBootMessage("Visuals: Rendering Sessions...");
   DrawSessionBoxes();
   LogBootMessage("Stats: Updating History Trackers...");
   lastProcessedLossTime=TimeCurrent();
   obvfriend_lastProcessedLossTime=TimeCurrent();
   statsResetTime=TimeCurrent();
   manual_statsResetTime=TimeCurrent();
   obvfriend_statsResetTime=TimeCurrent();
   UpdateStatsFromHistory();
   UpdateLiveTrackers();
   LogBootMessage("Visuals: Drawing Trade History...");
   DrawCustomTradeHistory();
   LogBootMessage("UI: Initializing Control Panel...");
   DrawControlPanel();
   if(UseDots&&isDotsVisualsVisible) DrawDotsPanel();
   RefreshRates();
   Calc_PoC_State_OnBar(0);
   Calc_Momentum_OnBar(0);
   Calc_ADX_OnBar(0);
   Calc_OBV_OnBar(0);
   CalcATR1M(0,atrPeriod);
   Calc_D2D_ST_OnBar(0);
   Calc_AdaptiveTrend_OnBar(0);
   Calc_OBVfriend_ST_OnBar(0);
   Calc_HarmVol_LLEMA_OnBar(0);
   Calc_Sqz_Momentum_OnBar(0);
   Calc_RangeOsc_OnBar(0);
   Calc_Microstructure_OnBar(0);
   Calc_Dots_Derived_OnBar(0);
   Calc_VWAP_OnBar(0);
   Calc_RefLevels_OnBar(0);
   Calc_MultiDay_OnBar(0);
   DrawLiveSignalBarSegment();
   if(isSuperTrendVisible) DrawLiveSuperTrendSegment();
   if(isOBVfriendSuperTrendVisible) DrawLiveOBVfriendSuperTrendSegment();
   if(isOBVVisualsVisible) DrawLiveOBVSegment();
   DrawLiveST_TrendDirectionIndicator();
   DrawLiveLT_TrendDirectionIndicator();
   lastHeartbeatTime=TimeCurrent();
   lastBarTime=Time[0];
   EventSetTimer(1);
   g_loadingProgress=0.95;
   g_isLoading=false;
   DrawLoadingBar(g_loadingProgress,"Aligning Visuals...");
   LogBootMessage("System: Awaiting final visual alignment...");
   ObjectSetInteger(0,ea_prefix+"BootPanelBG",OBJPROP_ZORDER,40005);
   for(int i=0; i<65; i++) ObjectSetInteger(0,ea_prefix+"BootLog_"+IntegerToString(i),OBJPROP_ZORDER,40006);
   ExportDataForAnalysis();
   if(g_warm_valid||Bars>=Warm_KAMA_Deep_Bars) WriteKamaWarmState(Time[1],state_HarmVol_KAMA[1]);
   ChartRedraw();
   return INIT_SUCCEEDED;
}
//+------------------------------------------------------------------+
//| SECTION 1.1 - EXPORTDATAFORANALYSIS                             |
//+------------------------------------------------------------------+
void RebuildStateForExport() {
   ResizeAllArrays(Bars);
   int startBar=Bars-1;
   if(startBar<1) startBar=1;
   for(int i=startBar; i>=1; i--) {
      hist_VolumeValue[i]=(double)Volume[i];
      Calc_PoC_State_OnBar(i);
      Calc_Momentum_OnBar(i);
      Calc_ADX_OnBar(i);
      Calc_OBV_OnBar(i);
      CalcATR1M(i,atrPeriod);
      Calc_D2D_ST_OnBar(i);
      Calc_AdaptiveTrend_OnBar(i);
      Calc_OBVfriend_ST_OnBar(i);
      Calc_HarmVol_LLEMA_OnBar(i);
      Calc_Sqz_Momentum_OnBar(i);
      Calc_RangeOsc_OnBar(i);
      Calc_Microstructure_OnBar(i);
      Calc_Dots_Derived_OnBar(i);
      Calc_VWAP_OnBar(i);
      Calc_RefLevels_OnBar(i);
      Calc_MultiDay_OnBar(i);
      hist_ADXValue[i]=ADXBuffer[i];
   }
}
void ExportDataForAnalysis() {
   string fileName=Symbol()+"_AUTO_EXPORT.csv";
   int fileHandle=FileOpen(fileName,FILE_WRITE|FILE_TXT);
   if(fileHandle==INVALID_HANDLE) return;
   int kamaBackupSize=ArraySize(state_HarmVol_KAMA);
   double warmKamaBackup[];
   ArrayResize(warmKamaBackup,kamaBackupSize);
   ArrayCopy(warmKamaBackup,state_HarmVol_KAMA,0,0,kamaBackupSize);
   g_warm_anchor_enabled=false;
   RebuildStateForExport();
   string hdr=
      "Time,Open,High,Low,Close,Volume,"+
      "EST_Hour,EST_Minute,EST_DayOfWeek,"+
      "Bar_Range,Body_Size,Upper_Wick,Lower_Wick,"+
      "D2D_Signal,D2D_Trend_Dir,D2D_Trend,"+
      "D2D_Upper_Band,D2D_Lower_Band,D2D_Basis,"+
      "D2D_ATR,D2D_ATR_MA,D2D_DirStep,D2D_Persist,"+
      "D2D_Up_Count,D2D_Dn_Count,D2D_Dynamic_Sensitivity,"+
      "D2D_UpTrend_Trail,D2D_DownTrend_Trail,"+
      "OBVf_Signal,OBVf_Trend_Dir,OBVf_Trend,"+
      "OBVf_Upper_Band,OBVf_Lower_Band,OBVf_Basis,"+
      "OBVf_ATR,OBVf_ATR_MA,OBVf_DirStep,OBVf_Persist,OBVf_DirStepCount,"+
      "OBVf_UpTrend_Trail,OBVf_DownTrend_Trail,"+
      "OBV_Line,OBV_Line_Prev,OBV_Accum,"+
      "OBV_Fast,OBV_Slow,OBV_Macd,OBV_Velocity,"+
      "OBV_Zero_Value,"+
      "TChan_A15,TChan_B5,"+
      "KAMA_Value,KAMA_Slope,KAMA_Dist,KAMA_Dist_ATR,KAMA_Side,"+
      "EMA_Oscillator,HarmVol_EMA8,HarmVol_EMA21,Harmonic_LLEMA,"+
      "Harmonic_Sign,Harmonic_OBVf_Concordance,Harmonic_D2D_Concordance,"+
      "ADX_Value,ADX_Rising,"+
      "ATR_1M,ATR_Assigned,"+
      "Momentum_Value,Efficiency_Ratio,"+
      "Sqz_State,Sqz_Val,"+
      "RangeOsc_State,RangeOsc_Val,"+
      "Hist_Volume,Volume_Avg_10,Volume_Ratio_10,"+
      "PoC_Price,Dist_To_PoC_ATR,PoC_Side,"+
      "ST_Flip_Event,Bars_Since_Flip,"+
      "Trend_Concordance,Trend_Conflict,"+
      "AT_Slope_ST,AT_Slope_LT,AT_Lookback_ST,AT_Lookback_LT,AT_Score_ST,AT_Score_LT,AT_Regime_ST,AT_Regime_LT,"+
      "DecayState_ST,DecayState_LT,Slope_EMA_ST,Slope_EMA_LT,Slope_Accel_ST,Slope_Accel_LT,"+
      "Micro_IBSP,Micro_Lambda,Micro_TickIntensity,Micro_GarmanKlass,"+
      "Micro_Rejection,Micro_OrderFlowDelta,Micro_BarEntropy,"+
      "Micro_LogReturn,Micro_PriceAccel,Micro_RollProxy,"+
      "Micro_BarOverlap,Micro_FailedBreak,Micro_MomoTransfer,"+
      "Micro_MicroGap,Micro_HLAsymmetry,Micro_VolAccel,"+
      "Micro_RangeVelocity,Micro_RangeAccel,Micro_ThrustEff,"+
      "Micro_AutoCorr,Micro_Entropy,Micro_VPIN,Micro_FractalDim,"+
      "Micro_VolOfVol,Micro_Amihud,Micro_WickImbalance,"+
      "Micro_CSSpread,Micro_Hurst,"+
      "Lock_Time,"+
      "VWAP_Price,VWAP_Sigma,VWAP_Dist_ATR,VWAP_Side,VWAP_Z,"+
      "VAH_Price,VAH_Dist_ATR,VAH_Side,VAL_Price,VAL_Dist_ATR,VAL_Side,VA_Position,"+
      "PrevDay_High,PrevDay_High_Dist_ATR,PrevDay_High_Side,"+
      "PrevDay_Low,PrevDay_Low_Dist_ATR,PrevDay_Low_Side,"+
      "PrevDay_Close,PrevDay_Close_Dist_ATR,PrevDay_Close_Side,"+
      "DailyOpen_Price,DailyOpen_Dist_ATR,DailyOpen_Side,"+
      "Round_100_Dist_ATR,Round_500_Dist_ATR,Round_1000_Dist_ATR,"+
      "OR_High,OR_High_Dist_ATR,OR_High_Side,OR_Low,OR_Low_Dist_ATR,OR_Low_Side,OR_Position,"+
      "Session_High,Session_High_Dist_ATR,Session_High_Side,"+
      "Session_Low,Session_Low_Dist_ATR,Session_Low_Side,"+
      "WeeklyOpen_Price,WeeklyOpen_Dist_ATR,WeeklyOpen_Side,"+
      "MultiDay_Slope,MultiDay_Position,VWAP_Sigma_ATR\r\n";
   FileWriteString(fileHandle,hdr);
   int volumeLimit=ArraySize(hist_VolumeValue);
   int d2dSignalLimit=ArraySize(LockBuffer);
   int d2dDirLimit=ArraySize(Direction);
   int superTrendLimit=ArraySize(SuperTrend);
   int d2dUpperLimit=ArraySize(U_UpperBandBuffer);
   int d2dLowerLimit=ArraySize(U_LowerBandBuffer);
   int uBasisLimit=ArraySize(U_BasisBuffer);
   int uAtrLimit=ArraySize(U_AtrBuffer);
   int uAtrMaLimit=ArraySize(U_AtrMaBuffer);
   int uDirStepLimit=ArraySize(U_DirStepBuffer);
   int uPersistLimit=ArraySize(U_PersistBuffer);
   int uUpLimit=ArraySize(U_UpCntBuffer);
   int uDnLimit=ArraySize(U_DnCntBuffer);
   int sensLimit=ArraySize(hist_Brain_Sensitivity);
   int upTrendLimit=ArraySize(UpTrendBuffer);
   int downTrendLimit=ArraySize(DownTrendBuffer);
   int tchanOcLimit=ArraySize(state_TChan_OC);
   int obvfDirLimit=ArraySize(OBVfriend_Direction);
   int obvfTrendLimit=ArraySize(OBVfriend_SuperTrend);
   int obvfUpperLimit=ArraySize(OBV_UpperBandBuffer);
   int obvfLowerLimit=ArraySize(OBV_LowerBandBuffer);
   int obvBasisLimit=ArraySize(OBV_BasisBuffer);
   int obvAtrLimit=ArraySize(OBV_AtrBuffer);
   int obvAtrMaLimit=ArraySize(OBV_AtrMaBuffer);
   int obvDirStepLimit=ArraySize(OBV_DirStepBuffer);
   int obvPersistLimit=ArraySize(OBV_PersistBuffer);
   int obvDirStepCntLimit=ArraySize(OBV_DirStepCountBuffer);
   int obvfUpTrendLimit=ArraySize(OBVfriend_UpTrendBuffer);
   int obvfDownTrendLimit=ArraySize(OBVfriend_DownTrendBuffer);
   int obvLimit=ArraySize(state_OBV_Final);
   int obvAccumLimit=ArraySize(state_OBV_Accum);
   int obvFastLimit=ArraySize(state_OBV_Fast);
   int obvSlowLimit=ArraySize(state_OBV_Slow);
   int obvMacdLimit=ArraySize(state_OBV_Macd);
   int obvVelLimit=ArraySize(state_OBV_Velocity);
   int obvZeroValLimit=ArraySize(hist_OBV_Zero_Value);
   int tchanSumLimit=ArraySize(state_TChan_Sum);
   int tchanB5Limit=ArraySize(state_TChan_B5);
   int kamaLimit=ArraySize(state_HarmVol_KAMA);
   int emaOscLimit=ArraySize(state_HarmVol_EMAOsc);
   int ema8Limit=ArraySize(state_HarmVol_EMA8);
   int ema21Limit=ArraySize(state_HarmVol_EMA21);
   int harmLlemaLimit=ArraySize(state_HarmVol_LLEMA);
   int adxLimit=ArraySize(hist_ADXValue);
   int atr1mLimit=ArraySize(ATR_1M_Array);
   int atrLimit=ArraySize(assignedATR);
   int momoLimit=ArraySize(MomentumBuffer);
   int effLimit=ArraySize(hist_Efficiency_Ratio);
   int sqzStateLimit=ArraySize(state_Sqz_State);
   int sqzValLimit=ArraySize(state_Sqz_Val);
   int rangeOscStateLimit=ArraySize(state_RangeOsc_State);
   int rangeOscValLimit=ArraySize(state_RangeOsc_Val);
   int pocLimit=ArraySize(hist_Poc_Price);
   int flipLimit=ArraySize(hist_ST_Flip_Event);
   int barsSinceLimit=ArraySize(hist_BarsSinceFlip_ST);
   int lockTimeLimit=ArraySize(LockTime);
   int atSlopeStLimit=ArraySize(hist_detectedSlope_ST);
   int atSlopeLtLimit=ArraySize(hist_LT_detectedSlope_ST);
   int atLookbackStLimit=ArraySize(hist_detectedAnchorBar_ST);
   int atLookbackLtLimit=ArraySize(hist_LT_detectedAnchorBar_ST);
   int atScoreStLimit=ArraySize(hist_trendStep_ST);
   int atScoreLtLimit=ArraySize(hist_LT_trendStep_ST);
   int atRegimeStLimit=ArraySize(hist_AnchorType_ST);
   int atRegimeLtLimit=ArraySize(hist_AnchorType_LT);
   int decayStateStLimit=ArraySize(hist_DecayState_ST);
   int decayStateLtLimit=ArraySize(hist_DecayState_LT);
   int slopeEmaStLimit=ArraySize(state_Slope_EMA_ST);
   int slopeEmaLtLimit=ArraySize(state_Slope_EMA_LT);
   int slopeAccelStLimit=ArraySize(state_Slope_Accel_ST);
   int slopeAccelLtLimit=ArraySize(state_Slope_Accel_LT);
   int ibspLimit=ArraySize(state_Micro_IBSP);
   int lambdaLimit=ArraySize(state_Micro_Lambda);
   int tickIntLimit=ArraySize(state_Micro_TickIntensity);
   int gkLimit=ArraySize(state_Micro_GarmanKlass);
   int rejectionLimit=ArraySize(state_Micro_Rejection);
   int ofdLimit=ArraySize(state_Micro_OrderFlowDelta);
   int barEntropyLimit=ArraySize(state_Micro_BarEntropy);
   int logRetLimit=ArraySize(state_Micro_LogReturn);
   int priceAccelLimit=ArraySize(state_Micro_PriceAccel);
   int rollProxyLimit=ArraySize(state_Micro_RollProxy);
   int barOverlapLimit=ArraySize(state_Micro_BarOverlap);
   int failedBreakLimit=ArraySize(state_Micro_FailedBreak);
   int momoTransferLimit=ArraySize(state_Micro_MomoTransfer);
   int microGapLimit=ArraySize(state_Micro_MicroGap);
   int hlAsymLimit=ArraySize(state_Micro_HLAsymmetry);
   int volAccelLimit=ArraySize(state_Micro_VolAccel);
   int rangeVelLimit=ArraySize(state_Micro_RangeVelocity);
   int rangeAccelLimit=ArraySize(state_Micro_RangeAccel);
   int thrustEffLimit=ArraySize(state_Micro_ThrustEff);
   int autoCorrLimit=ArraySize(state_Micro_AutoCorr);
   int entropyLimit=ArraySize(state_Micro_Entropy);
   int vpinLimit=ArraySize(state_Micro_VPIN);
   int fractalDimLimit=ArraySize(state_Micro_FractalDim);
   int volOfVolLimit=ArraySize(state_Micro_VolOfVol);
   int amihudLimit=ArraySize(state_Micro_Amihud);
   int wickImbLimit=ArraySize(state_Micro_WickImbalance);
   int csSpreadLimit=ArraySize(state_Micro_CSSpread);
   int hurstLimit=ArraySize(state_Micro_Hurst);
   int maxLimit=(int)MathMin(Bars-1,100000);
   int safeSize=ArraySize(hist_ST_Flip_Event)-1;
   int startIdx=(int)MathMin((int)MathMin(maxLimit,safeSize),Bars-2);
   for(int i=startIdx; i>=1; i--) {
      long estOffset=_GetEstOffsetForTime(Time[i]);
      datetime estBarTime=(datetime)(Time[i]+(datetime)estOffset);
      MqlDateTime estDt;
      ZeroMemory(estDt);
      TimeToStruct(estBarTime,estDt);
      double barRange=High[i]-Low[i];
      double bodySize=MathAbs(Close[i]-Open[i]);
      double upperWick=High[i]-MathMax(Open[i],Close[i]);
      double lowerWick=MathMin(Open[i],Close[i])-Low[i];
      int d2dSignal=(i<d2dSignalLimit)?(int)LockBuffer[i]:0;
      int d2dDir=(i<d2dDirLimit)?Direction[i]:0;
      double d2dTrend=(i<superTrendLimit)?SuperTrend[i]:0.0;
      if(d2dTrend==EMPTY_VALUE) d2dTrend=0.0;
      double d2dUpper=(i<d2dUpperLimit)?U_UpperBandBuffer[i]:0.0;
      double d2dLower=(i<d2dLowerLimit)?U_LowerBandBuffer[i]:0.0;
      double d2dBasis=(i<uBasisLimit)?U_BasisBuffer[i]:0.0;
      double d2dAtr=(i<uAtrLimit)?U_AtrBuffer[i]:0.0;
      double d2dAtrMa=(i<uAtrMaLimit)?U_AtrMaBuffer[i]:0.0;
      double d2dDirStep=(i<uDirStepLimit)?U_DirStepBuffer[i]:0.0;
      double d2dPersist=(i<uPersistLimit)?U_PersistBuffer[i]:0.0;
      int d2dUpCnt=(i<uUpLimit)?U_UpCntBuffer[i]:0;
      int d2dDnCnt=(i<uDnLimit)?U_DnCntBuffer[i]:0;
      double d2dSensitivity=(i<sensLimit)?hist_Brain_Sensitivity[i]:U_baseMult;
      double upTrail=(i<upTrendLimit)?UpTrendBuffer[i]:0.0;
      if(upTrail==EMPTY_VALUE) upTrail=0.0;
      double downTrail=(i<downTrendLimit)?DownTrendBuffer[i]:0.0;
      if(downTrail==EMPTY_VALUE) downTrail=0.0;
      int obvfSignal=(i<tchanOcLimit)?(int)MathRound(state_TChan_OC[i]):0;
      int obvfDir=(i<obvfDirLimit)?OBVfriend_Direction[i]:0;
      double obvfTrend=(i<obvfTrendLimit)?OBVfriend_SuperTrend[i]:0.0;
      if(obvfTrend==EMPTY_VALUE) obvfTrend=0.0;
      double obvfUpper=(i<obvfUpperLimit)?OBV_UpperBandBuffer[i]:0.0;
      double obvfLower=(i<obvfLowerLimit)?OBV_LowerBandBuffer[i]:0.0;
      double obvfBasis=(i<obvBasisLimit)?OBV_BasisBuffer[i]:0.0;
      double obvfAtr=(i<obvAtrLimit)?OBV_AtrBuffer[i]:0.0;
      double obvfAtrMa=(i<obvAtrMaLimit)?OBV_AtrMaBuffer[i]:0.0;
      double obvfDirStep=(i<obvDirStepLimit)?OBV_DirStepBuffer[i]:0.0;
      double obvfPersist=(i<obvPersistLimit)?OBV_PersistBuffer[i]:0.0;
      double obvfDirStepCnt=(i<obvDirStepCntLimit)?OBV_DirStepCountBuffer[i]:0.0;
      double obvfUpTrail=(i<obvfUpTrendLimit)?OBVfriend_UpTrendBuffer[i]:0.0;
      if(obvfUpTrail==EMPTY_VALUE) obvfUpTrail=0.0;
      double obvfDownTrail=(i<obvfDownTrendLimit)?OBVfriend_DownTrendBuffer[i]:0.0;
      if(obvfDownTrail==EMPTY_VALUE) obvfDownTrail=0.0;
      double obvLine=(i<obvLimit)?state_OBV_Final[i]:0.0;
      double obvLinePrev=((i+1)<obvLimit)?state_OBV_Final[i+1]:obvLine;
      double obvAccum=(i<obvAccumLimit)?state_OBV_Accum[i]:0.0;
      double obvFast=(i<obvFastLimit)?state_OBV_Fast[i]:0.0;
      double obvSlow=(i<obvSlowLimit)?state_OBV_Slow[i]:0.0;
      double obvMacd=(i<obvMacdLimit)?state_OBV_Macd[i]:0.0;
      double obvVelocity=(i<obvVelLimit)?state_OBV_Velocity[i]:0.0;
      double obvZeroVal=(i<obvZeroValLimit)?hist_OBV_Zero_Value[i]:0.0;
      double a15=(i<tchanSumLimit)?state_TChan_Sum[i]:0.0;
      double b5=(i<tchanB5Limit)?state_TChan_B5[i]:0.0;
      double kamaVal=(i<kamaLimit)?state_HarmVol_KAMA[i]:0.0;
      double kamaPrev=((i+1)<kamaLimit)?state_HarmVol_KAMA[i+1]:kamaVal;
      double kamaSlope=kamaVal-kamaPrev;
      double kamaDist=Close[i]-kamaVal;
      double atrAssigned=(i<atrLimit)?assignedATR[i]:0.0;
      double kamaDistATR=0.0;
      if(atrAssigned>0.0) kamaDistATR=kamaDist/atrAssigned;
      int kamaSide=0;
      if(Close[i]>kamaVal) kamaSide=1;
      else if(Close[i]<kamaVal) kamaSide=-1;
      double emaOscVal=(i<emaOscLimit)?state_HarmVol_EMAOsc[i]:0.0;
      double ema8Val=(i<ema8Limit)?state_HarmVol_EMA8[i]:0.0;
      double ema21Val=(i<ema21Limit)?state_HarmVol_EMA21[i]:0.0;
      double harmLlema=(i<harmLlemaLimit)?state_HarmVol_LLEMA[i]:0.0;
      int harmonicSign=0;
      if(harmLlema>0.0) harmonicSign=1;
      else if(harmLlema<0.0) harmonicSign=-1;
      int harmObvfConcordance=0;
      if(harmonicSign!=0&&harmonicSign==obvfSignal) harmObvfConcordance=1;
      int harmD2dConcordance=0;
      if(harmonicSign!=0&&harmonicSign==d2dDir) harmD2dConcordance=1;
      double adxVal=(i<adxLimit)?hist_ADXValue[i]:0.0;
      double adxPrev=((i+1)<adxLimit)?hist_ADXValue[i+1]:adxVal;
      int adxRising=(adxVal>adxPrev)?1:0;
      double atr1m=(i<atr1mLimit)?ATR_1M_Array[i]:0.0;
      double momoVal=(i<momoLimit)?MomentumBuffer[i]:0.0;
      double effVal=(i<effLimit)?hist_Efficiency_Ratio[i]:0.0;
      int sqzState=(i<sqzStateLimit)?state_Sqz_State[i]:0;
      double sqzVal=(i<sqzValLimit)?state_Sqz_Val[i]:0.0;
      int rangeOscState=(i<rangeOscStateLimit)?state_RangeOsc_State[i]:0;
      double rangeOscVal=(i<rangeOscValLimit)?state_RangeOsc_Val[i]:0.0;
      double histVol=(i<volumeLimit)?hist_VolumeValue[i]:0.0;
      if(histVol<=0.0&&Volume[i]>0) histVol=(double)Volume[i];
      double avgVol10=0.0;
      int avgCount10=0;
      for(int k=1; k<=10; k++) {
         int idx=i+k;
         if(idx<volumeLimit) {
            double v=hist_VolumeValue[idx];
            if(v<=0.0&&Volume[idx]>0) v=(double)Volume[idx];
            if(v>0.0) { avgVol10+=v; avgCount10++; }
         }
      }
      if(avgCount10>0) avgVol10/=(double)avgCount10;
      double volRatio10=0.0;
      if(avgVol10>0.0) volRatio10=histVol/avgVol10;
      double pocPrice=(i<pocLimit)?hist_Poc_Price[i]:0.0;
      double distToPocATR=0.0;
      if(atrAssigned>0.0&&pocPrice>0.0) distToPocATR=MathAbs(Close[i]-pocPrice)/atrAssigned;
      int pocSide=0;
      if(pocPrice>0.0) {
         if(Close[i]>pocPrice) pocSide=1;
         else if(Close[i]<pocPrice) pocSide=-1;
      }
      int stFlip=(i<flipLimit)?hist_ST_Flip_Event[i]:0;
      int barsSinceFlip=(i<barsSinceLimit)?hist_BarsSinceFlip_ST[i]:0;
      int trendConcordance=0;
      if(obvfSignal!=0&&obvfSignal==d2dDir) trendConcordance=1;
      int trendConflict=0;
      if(obvfSignal!=0&&d2dDir!=0&&obvfSignal!=d2dDir) trendConflict=1;
      double atSlopeSt=(i<atSlopeStLimit)?hist_detectedSlope_ST[i]:0.0;
      double atSlopeLt=(i<atSlopeLtLimit)?hist_LT_detectedSlope_ST[i]:0.0;
      int atLookbackSt=(i<atLookbackStLimit)?hist_detectedAnchorBar_ST[i]:0;
      int atLookbackLt=(i<atLookbackLtLimit)?hist_LT_detectedAnchorBar_ST[i]:0;
      int atScoreSt=(i<atScoreStLimit)?hist_trendStep_ST[i]:0;
      int atScoreLt=(i<atScoreLtLimit)?hist_LT_trendStep_ST[i]:0;
      int atRegimeSt=(i<atRegimeStLimit)?hist_AnchorType_ST[i]:0;
      int atRegimeLt=(i<atRegimeLtLimit)?hist_AnchorType_LT[i]:0;
      int decayStateSt=(i<decayStateStLimit)?hist_DecayState_ST[i]:0;
      int decayStateLt=(i<decayStateLtLimit)?hist_DecayState_LT[i]:0;
      double slopeEmaSt=(i<slopeEmaStLimit)?state_Slope_EMA_ST[i]:0.0;
      double slopeEmaLt=(i<slopeEmaLtLimit)?state_Slope_EMA_LT[i]:0.0;
      double slopeAccelSt=(i<slopeAccelStLimit)?state_Slope_Accel_ST[i]:0.0;
      double slopeAccelLt=(i<slopeAccelLtLimit)?state_Slope_Accel_LT[i]:0.0;
      double ibspVal=(i<ibspLimit)?state_Micro_IBSP[i]:0.0;
      double lambdaVal=(i<lambdaLimit)?state_Micro_Lambda[i]:0.0;
      double tickIntVal=(i<tickIntLimit)?state_Micro_TickIntensity[i]:0.0;
      double gkVal=(i<gkLimit)?state_Micro_GarmanKlass[i]:0.0;
      double rejectionVal=(i<rejectionLimit)?state_Micro_Rejection[i]:0.0;
      double ofdVal=(i<ofdLimit)?state_Micro_OrderFlowDelta[i]:0.0;
      double barEntropyVal=(i<barEntropyLimit)?state_Micro_BarEntropy[i]:0.0;
      double logRetVal=(i<logRetLimit)?state_Micro_LogReturn[i]:0.0;
      double priceAccelVal=(i<priceAccelLimit)?state_Micro_PriceAccel[i]:0.0;
      double rollProxyVal=(i<rollProxyLimit)?state_Micro_RollProxy[i]:0.0;
      double barOverlapVal=(i<barOverlapLimit)?state_Micro_BarOverlap[i]:0.0;
      double failedBreakVal=(i<failedBreakLimit)?state_Micro_FailedBreak[i]:0.0;
      double momoTransferVal=(i<momoTransferLimit)?state_Micro_MomoTransfer[i]:0.0;
      double microGapVal=(i<microGapLimit)?state_Micro_MicroGap[i]:0.0;
      double hlAsymVal=(i<hlAsymLimit)?state_Micro_HLAsymmetry[i]:0.0;
      double volAccelVal=(i<volAccelLimit)?state_Micro_VolAccel[i]:0.0;
      double rangeVelVal=(i<rangeVelLimit)?state_Micro_RangeVelocity[i]:0.0;
      double rangeAccelVal=(i<rangeAccelLimit)?state_Micro_RangeAccel[i]:0.0;
      double thrustEffVal=(i<thrustEffLimit)?state_Micro_ThrustEff[i]:0.0;
      double autoCorrVal=(i<autoCorrLimit)?state_Micro_AutoCorr[i]:0.0;
      double entropyVal=(i<entropyLimit)?state_Micro_Entropy[i]:0.0;
      double vpinVal=(i<vpinLimit)?state_Micro_VPIN[i]:0.0;
      double fractalDimVal=(i<fractalDimLimit)?state_Micro_FractalDim[i]:0.0;
      double volOfVolVal=(i<volOfVolLimit)?state_Micro_VolOfVol[i]:0.0;
      double amihudVal=(i<amihudLimit)?state_Micro_Amihud[i]:0.0;
      double wickImbVal=(i<wickImbLimit)?state_Micro_WickImbalance[i]:0.0;
      double csSpreadVal=(i<csSpreadLimit)?state_Micro_CSSpread[i]:0.0;
      double hurstVal=(i<hurstLimit)?state_Micro_Hurst[i]:0.0;
      datetime lockTimeVal=(i<lockTimeLimit)?LockTime[i]:(datetime)0;
      double vwapPrice=(i<ArraySize(hist_VWAP_Price))?hist_VWAP_Price[i]:0.0;
      double vwapSigma=(i<ArraySize(hist_VWAP_Sigma))?hist_VWAP_Sigma[i]:0.0;
      double vwapDistATR=0.0; if(atrAssigned>0.0&&vwapPrice>0.0) vwapDistATR=MathAbs(Close[i]-vwapPrice)/atrAssigned;
      int vwapSide=0; if(vwapPrice>0.0){ if(Close[i]>vwapPrice) vwapSide=1; else if(Close[i]<vwapPrice) vwapSide=-1; }
      double vwapZ=0.0; if(vwapSigma>0.0&&vwapPrice>0.0) vwapZ=(Close[i]-vwapPrice)/vwapSigma;
      double vahPrice=(i<ArraySize(hist_VAH_Price))?hist_VAH_Price[i]:0.0;
      double vahDistATR=0.0; if(atrAssigned>0.0&&vahPrice>0.0) vahDistATR=MathAbs(Close[i]-vahPrice)/atrAssigned;
      int vahSide=0; if(vahPrice>0.0){ if(Close[i]>vahPrice) vahSide=1; else if(Close[i]<vahPrice) vahSide=-1; }
      double valPrice=(i<ArraySize(hist_VAL_Price))?hist_VAL_Price[i]:0.0;
      double valDistATR=0.0; if(atrAssigned>0.0&&valPrice>0.0) valDistATR=MathAbs(Close[i]-valPrice)/atrAssigned;
      int valSide=0; if(valPrice>0.0){ if(Close[i]>valPrice) valSide=1; else if(Close[i]<valPrice) valSide=-1; }
      double vaPos=0.5; if(vahPrice>0.0&&valPrice>0.0&&vahPrice>valPrice) vaPos=(Close[i]-valPrice)/(vahPrice-valPrice);
      double pdHigh=(i<ArraySize(hist_PrevDay_High))?hist_PrevDay_High[i]:0.0;
      double pdHighDistATR=0.0; if(atrAssigned>0.0&&pdHigh>0.0) pdHighDistATR=MathAbs(Close[i]-pdHigh)/atrAssigned;
      int pdHighSide=0; if(pdHigh>0.0){ if(Close[i]>pdHigh) pdHighSide=1; else if(Close[i]<pdHigh) pdHighSide=-1; }
      double pdLow=(i<ArraySize(hist_PrevDay_Low))?hist_PrevDay_Low[i]:0.0;
      double pdLowDistATR=0.0; if(atrAssigned>0.0&&pdLow>0.0) pdLowDistATR=MathAbs(Close[i]-pdLow)/atrAssigned;
      int pdLowSide=0; if(pdLow>0.0){ if(Close[i]>pdLow) pdLowSide=1; else if(Close[i]<pdLow) pdLowSide=-1; }
      double pdClose=(i<ArraySize(hist_PrevDay_Close))?hist_PrevDay_Close[i]:0.0;
      double pdCloseDistATR=0.0; if(atrAssigned>0.0&&pdClose>0.0) pdCloseDistATR=MathAbs(Close[i]-pdClose)/atrAssigned;
      int pdCloseSide=0; if(pdClose>0.0){ if(Close[i]>pdClose) pdCloseSide=1; else if(Close[i]<pdClose) pdCloseSide=-1; }
      double dOpen=(i<ArraySize(hist_DailyOpen_Price))?hist_DailyOpen_Price[i]:0.0;
      double dOpenDistATR=0.0; if(atrAssigned>0.0&&dOpen>0.0) dOpenDistATR=MathAbs(Close[i]-dOpen)/atrAssigned;
      int dOpenSide=0; if(dOpen>0.0){ if(Close[i]>dOpen) dOpenSide=1; else if(Close[i]<dOpen) dOpenSide=-1; }
      double r100ATR=0.0,r500ATR=0.0,r1000ATR=0.0;
      if(atrAssigned>0.0){
         r100ATR=MathAbs(Close[i]-MathRound(Close[i]/100.0)*100.0)/atrAssigned;
         r500ATR=MathAbs(Close[i]-MathRound(Close[i]/500.0)*500.0)/atrAssigned;
         r1000ATR=MathAbs(Close[i]-MathRound(Close[i]/1000.0)*1000.0)/atrAssigned;
      }
      double orHigh=(i<ArraySize(hist_OR_High))?hist_OR_High[i]:0.0;
      double orHighDistATR=0.0; if(atrAssigned>0.0&&orHigh>0.0) orHighDistATR=MathAbs(Close[i]-orHigh)/atrAssigned;
      int orHighSide=0; if(orHigh>0.0){ if(Close[i]>orHigh) orHighSide=1; else if(Close[i]<orHigh) orHighSide=-1; }
      double orLow=(i<ArraySize(hist_OR_Low))?hist_OR_Low[i]:0.0;
      double orLowDistATR=0.0; if(atrAssigned>0.0&&orLow>0.0) orLowDistATR=MathAbs(Close[i]-orLow)/atrAssigned;
      int orLowSide=0; if(orLow>0.0){ if(Close[i]>orLow) orLowSide=1; else if(Close[i]<orLow) orLowSide=-1; }
      double orPos=0.5; if(orHigh>0.0&&orLow>0.0&&orHigh>orLow) orPos=(Close[i]-orLow)/(orHigh-orLow);
      double sHigh=(i<ArraySize(hist_Session_High))?hist_Session_High[i]:0.0;
      double sHighDistATR=0.0; if(atrAssigned>0.0&&sHigh>0.0) sHighDistATR=MathAbs(Close[i]-sHigh)/atrAssigned;
      int sHighSide=0; if(sHigh>0.0){ if(Close[i]>sHigh) sHighSide=1; else if(Close[i]<sHigh) sHighSide=-1; }
      double sLow=(i<ArraySize(hist_Session_Low))?hist_Session_Low[i]:0.0;
      double sLowDistATR=0.0; if(atrAssigned>0.0&&sLow>0.0) sLowDistATR=MathAbs(Close[i]-sLow)/atrAssigned;
      int sLowSide=0; if(sLow>0.0){ if(Close[i]>sLow) sLowSide=1; else if(Close[i]<sLow) sLowSide=-1; }
      double wOpen=(i<ArraySize(hist_WeeklyOpen_Price))?hist_WeeklyOpen_Price[i]:0.0;
      double wOpenDistATR=0.0; if(atrAssigned>0.0&&wOpen>0.0) wOpenDistATR=MathAbs(Close[i]-wOpen)/atrAssigned;
      int wOpenSide=0; if(wOpen>0.0){ if(Close[i]>wOpen) wOpenSide=1; else if(Close[i]<wOpen) wOpenSide=-1; }
      double mdSlope=(i<ArraySize(hist_MultiDay_Slope))?hist_MultiDay_Slope[i]:0.0;
      double mdPos=(i<ArraySize(hist_MultiDay_Position))?hist_MultiDay_Position[i]:0.0;
      double vwapSigmaATR=0.0; if(atrAssigned>0.0) vwapSigmaATR=vwapSigma/atrAssigned;
      string row=
         TimeToString(Time[i],TIME_DATE|TIME_MINUTES)+","+
         DoubleToString(Open[i],Digits)+","+
         DoubleToString(High[i],Digits)+","+
         DoubleToString(Low[i],Digits)+","+
         DoubleToString(Close[i],Digits)+","+
         IntegerToString((int)Volume[i])+","+
         IntegerToString((int)estDt.hour)+","+
         IntegerToString((int)estDt.min)+","+
         IntegerToString((int)estDt.day_of_week)+","+
         DoubleToString(barRange,Digits)+","+
         DoubleToString(bodySize,Digits)+","+
         DoubleToString(upperWick,Digits)+","+
         DoubleToString(lowerWick,Digits)+","+
         IntegerToString(d2dSignal)+","+
         IntegerToString(d2dDir)+","+
         DoubleToString(d2dTrend,Digits)+","+
         DoubleToString(d2dUpper,Digits)+","+
         DoubleToString(d2dLower,Digits)+","+
         DoubleToString(d2dBasis,Digits)+","+
         DoubleToString(d2dAtr,6)+","+
         DoubleToString(d2dAtrMa,6)+","+
         DoubleToString(d2dDirStep,4)+","+
         DoubleToString(d2dPersist,6)+","+
         IntegerToString(d2dUpCnt)+","+
         IntegerToString(d2dDnCnt)+","+
         DoubleToString(d2dSensitivity,6)+","+
         DoubleToString(upTrail,Digits)+","+
         DoubleToString(downTrail,Digits)+","+
         IntegerToString(obvfSignal)+","+
         IntegerToString(obvfDir)+","+
         DoubleToString(obvfTrend,Digits)+","+
         DoubleToString(obvfUpper,Digits)+","+
         DoubleToString(obvfLower,Digits)+","+
         DoubleToString(obvfBasis,Digits)+","+
         DoubleToString(obvfAtr,6)+","+
         DoubleToString(obvfAtrMa,6)+","+
         DoubleToString(obvfDirStep,4)+","+
         DoubleToString(obvfPersist,6)+","+
         DoubleToString(obvfDirStepCnt,0)+","+
         DoubleToString(obvfUpTrail,Digits)+","+
         DoubleToString(obvfDownTrail,Digits)+","+
         DoubleToString(obvLine,6)+","+
         DoubleToString(obvLinePrev,6)+","+
         DoubleToString(obvAccum,6)+","+
         DoubleToString(obvFast,6)+","+
         DoubleToString(obvSlow,6)+","+
         DoubleToString(obvMacd,6)+","+
         DoubleToString(obvVelocity,6)+","+
         DoubleToString(obvZeroVal,6)+","+
         DoubleToString(a15,6)+","+
         DoubleToString(b5,6)+","+
         DoubleToString(kamaVal,Digits)+","+
         DoubleToString(kamaSlope,6)+","+
         DoubleToString(kamaDist,6)+","+
         DoubleToString(kamaDistATR,6)+","+
         IntegerToString(kamaSide)+","+
         DoubleToString(emaOscVal,6)+","+
         DoubleToString(ema8Val,Digits)+","+
         DoubleToString(ema21Val,Digits)+","+
         DoubleToString(harmLlema,6)+","+
         IntegerToString(harmonicSign)+","+
         IntegerToString(harmObvfConcordance)+","+
         IntegerToString(harmD2dConcordance)+","+
         DoubleToString(adxVal,6)+","+
         IntegerToString(adxRising)+","+
         DoubleToString(atr1m,6)+","+
         DoubleToString(atrAssigned,Digits)+","+
         DoubleToString(momoVal,6)+","+
         DoubleToString(effVal,6)+","+
         IntegerToString(sqzState)+","+
         DoubleToString(sqzVal,6)+","+
         IntegerToString(rangeOscState)+","+
         DoubleToString(rangeOscVal,6)+","+
         DoubleToString(histVol,0)+","+
         DoubleToString(avgVol10,2)+","+
         DoubleToString(volRatio10,6)+","+
         DoubleToString(pocPrice,Digits)+","+
         DoubleToString(distToPocATR,6)+","+
         IntegerToString(pocSide)+","+
         IntegerToString(stFlip)+","+
         IntegerToString(barsSinceFlip)+","+
         IntegerToString(trendConcordance)+","+
         IntegerToString(trendConflict)+","+
         DoubleToString(atSlopeSt,6)+","+
         DoubleToString(atSlopeLt,6)+","+
         IntegerToString(atLookbackSt)+","+
         IntegerToString(atLookbackLt)+","+
         IntegerToString(atScoreSt)+","+
         IntegerToString(atScoreLt)+","+
         IntegerToString(atRegimeSt)+","+
         IntegerToString(atRegimeLt)+","+
         IntegerToString(decayStateSt)+","+
         IntegerToString(decayStateLt)+","+
         DoubleToString(slopeEmaSt,6)+","+
         DoubleToString(slopeEmaLt,6)+","+
         DoubleToString(slopeAccelSt,6)+","+
         DoubleToString(slopeAccelLt,6)+","+
         DoubleToString(ibspVal,6)+","+
         DoubleToString(lambdaVal,6)+","+
         DoubleToString(tickIntVal,6)+","+
         DoubleToString(gkVal,10)+","+
         DoubleToString(rejectionVal,6)+","+
         DoubleToString(ofdVal,6)+","+
         DoubleToString(barEntropyVal,6)+","+
         DoubleToString(logRetVal,6)+","+
         DoubleToString(priceAccelVal,6)+","+
         DoubleToString(rollProxyVal,6)+","+
         DoubleToString(barOverlapVal,6)+","+
         DoubleToString(failedBreakVal,6)+","+
         DoubleToString(momoTransferVal,6)+","+
         DoubleToString(microGapVal,6)+","+
         DoubleToString(hlAsymVal,6)+","+
         DoubleToString(volAccelVal,6)+","+
         DoubleToString(rangeVelVal,6)+","+
         DoubleToString(rangeAccelVal,6)+","+
         DoubleToString(thrustEffVal,6)+","+
         DoubleToString(autoCorrVal,6)+","+
         DoubleToString(entropyVal,6)+","+
         DoubleToString(vpinVal,6)+","+
         DoubleToString(fractalDimVal,6)+","+
         DoubleToString(volOfVolVal,6)+","+
         DoubleToString(amihudVal,6)+","+
         DoubleToString(wickImbVal,6)+","+
         DoubleToString(csSpreadVal,6)+","+
         DoubleToString(hurstVal,6)+","+
         TimeToString(lockTimeVal,TIME_DATE|TIME_MINUTES)+","+
         DoubleToString(vwapPrice,Digits)+","+
         DoubleToString(vwapSigma,6)+","+
         DoubleToString(vwapDistATR,6)+","+
         IntegerToString(vwapSide)+","+
         DoubleToString(vwapZ,6)+","+
         DoubleToString(vahPrice,Digits)+","+
         DoubleToString(vahDistATR,6)+","+
         IntegerToString(vahSide)+","+
         DoubleToString(valPrice,Digits)+","+
         DoubleToString(valDistATR,6)+","+
         IntegerToString(valSide)+","+
         DoubleToString(vaPos,6)+","+
         DoubleToString(pdHigh,Digits)+","+
         DoubleToString(pdHighDistATR,6)+","+
         IntegerToString(pdHighSide)+","+
         DoubleToString(pdLow,Digits)+","+
         DoubleToString(pdLowDistATR,6)+","+
         IntegerToString(pdLowSide)+","+
         DoubleToString(pdClose,Digits)+","+
         DoubleToString(pdCloseDistATR,6)+","+
         IntegerToString(pdCloseSide)+","+
         DoubleToString(dOpen,Digits)+","+
         DoubleToString(dOpenDistATR,6)+","+
         IntegerToString(dOpenSide)+","+
         DoubleToString(r100ATR,6)+","+
         DoubleToString(r500ATR,6)+","+
         DoubleToString(r1000ATR,6)+","+
         DoubleToString(orHigh,Digits)+","+
         DoubleToString(orHighDistATR,6)+","+
         IntegerToString(orHighSide)+","+
         DoubleToString(orLow,Digits)+","+
         DoubleToString(orLowDistATR,6)+","+
         IntegerToString(orLowSide)+","+
         DoubleToString(orPos,6)+","+
         DoubleToString(sHigh,Digits)+","+
         DoubleToString(sHighDistATR,6)+","+
         IntegerToString(sHighSide)+","+
         DoubleToString(sLow,Digits)+","+
         DoubleToString(sLowDistATR,6)+","+
         IntegerToString(sLowSide)+","+
         DoubleToString(wOpen,Digits)+","+
         DoubleToString(wOpenDistATR,6)+","+
         IntegerToString(wOpenSide)+","+
         DoubleToString(mdSlope,6)+","+
         DoubleToString(mdPos,6)+","+
         DoubleToString(vwapSigmaATR,6)+"\r\n";
      FileWriteString(fileHandle,row);
   }
   FileClose(fileHandle);
   Print("System: Blinded AI Data Auto-Exported to MQL4/Files/"+fileName);
   g_warm_anchor_enabled=true;
   if(ArraySize(state_HarmVol_KAMA)==kamaBackupSize) {
      ArrayCopy(state_HarmVol_KAMA,warmKamaBackup,0,0,kamaBackupSize);
   } else {
      g_warm_valid=false;
      g_warm_suppress_kama_signals=true;
   }
}
//+------------------------------------------------------------------+
//| SECTION 1.2 - MAIN LOOP                                          |
//+------------------------------------------------------------------+
void OnTick() {
   if(!IsConnected()||IsStopped()) return;
   if(!IsTesting()&&Bars<ClusteringLookback+trainingBars) return;
   static bool g_boot_bridge_complete=false;
   if(!g_boot_bridge_complete) {
      ForceRePaintSignals();
      g_boot_bridge_complete=true;
   }
   static bool bootConsoleRemoved=false;
   if(!bootConsoleRemoved&&TimeCurrent()>g_loadingBarHideTime&&g_loadingBarHideTime>0) {
      if(ObjectFind(0,ea_prefix+"BootPanelBG")>=0) {
         RemoveBootConsole();
         bootConsoleRemoved=true;
      }
   }
   if(TimeCurrent()-lastUIRefreshTime>=1) {
      UpdateSignalMarkerPositions();
      UpdateCustomTradeHistoryPositions();
      UpdateST_TrendDirectionIndicatorPositions();
      UpdateLT_TrendDirectionIndicatorPositions();
      if(isOBVVisualsVisible) UpdateOBVPositions();
      if(isSessionVisualsVisible) DrawSessionBoxes();
      if(isPocVisualsVisible) UpdateDailyPoCPositions();
      DrawControlPanel();
      if(UseDots&&isDotsVisualsVisible) DrawDotsPanel();
      lastUIRefreshTime=TimeCurrent();
   }
   CheckHeartbeat();
   LogStatusUpdate();
   EnforceTradingSession();
   EnforceBlockTimeSession();
   ManageForcedSessionActions();
   if(UseDots) LogEndOfDaySummary();
   bool isNewBar=(Time[0]!=lastBarTime);
   if(isNewBar) {
      lastBarTime=Time[0];
      ResizeAllArrays(Bars,true); 
      LockBuffer[0]=0;
      Direction[0]=Direction[1];
      SuperTrend[0]=SuperTrend[1];
      OBVfriend_Direction[0]=OBVfriend_Direction[1];
      OBVfriend_SuperTrend[0]=OBVfriend_SuperTrend[1];
      LockTime[0]=0;
      int barsToCalc=Bars-lastCalculatedBars;
      if(barsToCalc<2) barsToCalc=2;
      for(int calcIdx=barsToCalc; calcIdx>=1; calcIdx--) {
         hist_VolumeValue[calcIdx]=(double)Volume[calcIdx];
         CalcATR1M(calcIdx,atrPeriod);
         Calc_PoC_State_OnBar(calcIdx); 
         Calc_Momentum_OnBar(calcIdx);
         Calc_ADX_OnBar(calcIdx);
         Calc_OBV_OnBar(calcIdx); 
         Calc_D2D_ST_OnBar(calcIdx);
         Calc_AdaptiveTrend_OnBar(calcIdx);
         Calc_OBVfriend_ST_OnBar(calcIdx);
         Calc_HarmVol_LLEMA_OnBar(calcIdx);
         Calc_Sqz_Momentum_OnBar(calcIdx);
         Calc_RangeOsc_OnBar(calcIdx);
         Calc_Microstructure_OnBar(calcIdx);
         Calc_Dots_Derived_OnBar(calcIdx);
         Calc_VWAP_OnBar(calcIdx);
         Calc_RefLevels_OnBar(calcIdx);
         Calc_MultiDay_OnBar(calcIdx);
         hist_ADXValue[calcIdx]=ADXBuffer[calcIdx];
         if(hist_ST_Flip_Event[calcIdx]!=0) {
            PlayTrendFlipAlert(calcIdx);
            if(isTrendVisualsVisible) DrawT2TFlipOnChart(calcIdx,hist_ST_Flip_Event[calcIdx]);
         }
         PlayOBVAlert(calcIdx);
         if(isOBVVisualsVisible&&calcIdx<ArraySize(state_OBV_Final)) DrawOBV_Visuals_Historical(calcIdx);
      }
      lastCalculatedBars=Bars;
      if(g_warm_valid||Bars>=Warm_KAMA_Deep_Bars) WriteKamaWarmState(Time[1],state_HarmVol_KAMA[1]);
      if(g_warm_suppress_kama_signals) {
         if(g_warm_suppress_counter>0) g_warm_suppress_counter--;
         if(g_warm_valid&&g_warm_suppress_counter<=0) g_warm_suppress_kama_signals=false;
      }
      if(UseDots) {
         if(hist_ADXValue[1]>=15.0&&hist_VolumeValue[1]>50.0) {
            DotsInsertRollingValue(FEAT_ATR_1M,dots_roll_buf_ATR_1M,DotsGetFeatureValue(FEAT_ATR_1M,1));
            DotsInsertRollingValue(FEAT_Bar_Range,dots_roll_buf_Bar_Range,DotsGetFeatureValue(FEAT_Bar_Range,1));
            DotsInsertRollingValue(FEAT_D2D_ATR,dots_roll_buf_D2D_ATR,DotsGetFeatureValue(FEAT_D2D_ATR,1));
            DotsInsertRollingValue(FEAT_D2D_ATR_MA,dots_roll_buf_D2D_ATR_MA,DotsGetFeatureValue(FEAT_D2D_ATR_MA,1));
            DotsInsertRollingValue(FEAT_D2D_Dn_Count,dots_roll_buf_D2D_Dn_Count,DotsGetFeatureValue(FEAT_D2D_Dn_Count,1));
            DotsInsertRollingValue(FEAT_D2D_Dynamic_Sensitivity,dots_roll_buf_D2D_Dynamic_Sensitivity,DotsGetFeatureValue(FEAT_D2D_Dynamic_Sensitivity,1));
            DotsInsertRollingValue(FEAT_D2D_Persist,dots_roll_buf_D2D_Persist,DotsGetFeatureValue(FEAT_D2D_Persist,1));
            DotsInsertRollingValue(FEAT_D2D_Up_Count,dots_roll_buf_D2D_Up_Count,DotsGetFeatureValue(FEAT_D2D_Up_Count,1));
            DotsInsertRollingValue(FEAT_AT_Lookback_LT,dots_roll_buf_AT_Lookback_LT,DotsGetFeatureValue(FEAT_AT_Lookback_LT,1));
            DotsInsertRollingValue(FEAT_AT_Lookback_ST,dots_roll_buf_AT_Lookback_ST,DotsGetFeatureValue(FEAT_AT_Lookback_ST,1));
            DotsInsertRollingValue(FEAT_AT_Score_LT,dots_roll_buf_AT_Score_LT,DotsGetFeatureValue(FEAT_AT_Score_LT,1));
            DotsInsertRollingValue(FEAT_AT_Score_ST,dots_roll_buf_AT_Score_ST,DotsGetFeatureValue(FEAT_AT_Score_ST,1));
            DotsInsertRollingValue(FEAT_AT_Slope_LT,dots_roll_buf_AT_Slope_LT,DotsGetFeatureValue(FEAT_AT_Slope_LT,1));
            DotsInsertRollingValue(FEAT_AT_Slope_ST,dots_roll_buf_AT_Slope_ST,DotsGetFeatureValue(FEAT_AT_Slope_ST,1));
            DotsInsertRollingValue(FEAT_Bars_Since_Flip,dots_roll_buf_Bars_Since_Flip,DotsGetFeatureValue(FEAT_Bars_Since_Flip,1));
            DotsInsertRollingValue(FEAT_Slope_EMA_LT,dots_roll_buf_Slope_EMA_LT,DotsGetFeatureValue(FEAT_Slope_EMA_LT,1));
            DotsInsertRollingValue(FEAT_Slope_EMA_ST,dots_roll_buf_Slope_EMA_ST,DotsGetFeatureValue(FEAT_Slope_EMA_ST,1));
            DotsInsertRollingValue(FEAT_Slope_Accel_LT,dots_roll_buf_Slope_Accel_LT,DotsGetFeatureValue(FEAT_Slope_Accel_LT,1));
            DotsInsertRollingValue(FEAT_Slope_Accel_ST,dots_roll_buf_Slope_Accel_ST,DotsGetFeatureValue(FEAT_Slope_Accel_ST,1));
            DotsInsertRollingValue(FEAT_OBV_Macd,dots_roll_buf_OBV_Macd,DotsGetFeatureValue(FEAT_OBV_Macd,1));
            DotsInsertRollingValue(FEAT_OBV_Velocity,dots_roll_buf_OBV_Velocity,DotsGetFeatureValue(FEAT_OBV_Velocity,1));
            DotsInsertRollingValue(FEAT_OBVf_DirStepCount,dots_roll_buf_OBVf_DirStepCount,DotsGetFeatureValue(FEAT_OBVf_DirStepCount,1));
            DotsInsertRollingValue(FEAT_KAMA_Dist,dots_roll_buf_KAMA_Dist,DotsGetFeatureValue(FEAT_KAMA_Dist,1));
            DotsInsertRollingValue(FEAT_KAMA_Dist_ATR,dots_roll_buf_KAMA_Dist_ATR,DotsGetFeatureValue(FEAT_KAMA_Dist_ATR,1));
            DotsInsertRollingValue(FEAT_KAMA_Slope,dots_roll_buf_KAMA_Slope,DotsGetFeatureValue(FEAT_KAMA_Slope,1));
            DotsInsertRollingValue(FEAT_EMA_Oscillator,dots_roll_buf_EMA_Oscillator,DotsGetFeatureValue(FEAT_EMA_Oscillator,1));
            DotsInsertRollingValue(FEAT_Harmonic_LLEMA,dots_roll_buf_Harmonic_LLEMA,DotsGetFeatureValue(FEAT_Harmonic_LLEMA,1));
            DotsInsertRollingValue(FEAT_Sqz_Val,dots_roll_buf_Sqz_Val,DotsGetFeatureValue(FEAT_Sqz_Val,1));
            DotsInsertRollingValue(FEAT_RangeOsc_Val,dots_roll_buf_RangeOsc_Val,DotsGetFeatureValue(FEAT_RangeOsc_Val,1));
            DotsInsertRollingValue(FEAT_Volume_Avg_10,dots_roll_buf_Volume_Avg_10,DotsGetFeatureValue(FEAT_Volume_Avg_10,1));
            DotsInsertRollingValue(FEAT_Volume_Ratio_10,dots_roll_buf_Volume_Ratio_10,DotsGetFeatureValue(FEAT_Volume_Ratio_10,1));
            DotsInsertRollingValue(FEAT_Momentum_Value,dots_roll_buf_Momentum_Value,DotsGetFeatureValue(FEAT_Momentum_Value,1));
            DotsInsertRollingValue(FEAT_Efficiency_Ratio,dots_roll_buf_Efficiency_Ratio,DotsGetFeatureValue(FEAT_Efficiency_Ratio,1));
            DotsInsertRollingValue(FEAT_Dist_To_PoC_ATR,dots_roll_buf_Dist_To_PoC_ATR,DotsGetFeatureValue(FEAT_Dist_To_PoC_ATR,1));
            DotsInsertRollingValue(FEAT_Micro_Amihud,dots_roll_buf_Micro_Amihud,DotsGetFeatureValue(FEAT_Micro_Amihud,1));
            DotsInsertRollingValue(FEAT_Micro_AutoCorr,dots_roll_buf_Micro_AutoCorr,DotsGetFeatureValue(FEAT_Micro_AutoCorr,1));
            DotsInsertRollingValue(FEAT_Micro_BarEntropy,dots_roll_buf_Micro_BarEntropy,DotsGetFeatureValue(FEAT_Micro_BarEntropy,1));
            DotsInsertRollingValue(FEAT_Micro_BarOverlap,dots_roll_buf_Micro_BarOverlap,DotsGetFeatureValue(FEAT_Micro_BarOverlap,1));
            DotsInsertRollingValue(FEAT_Micro_CSSpread,dots_roll_buf_Micro_CSSpread,DotsGetFeatureValue(FEAT_Micro_CSSpread,1));
            DotsInsertRollingValue(FEAT_Micro_Entropy,dots_roll_buf_Micro_Entropy,DotsGetFeatureValue(FEAT_Micro_Entropy,1));
            DotsInsertRollingValue(FEAT_Micro_FailedBreak,dots_roll_buf_Micro_FailedBreak,DotsGetFeatureValue(FEAT_Micro_FailedBreak,1));
            DotsInsertRollingValue(FEAT_Micro_FractalDim,dots_roll_buf_Micro_FractalDim,DotsGetFeatureValue(FEAT_Micro_FractalDim,1));
            DotsInsertRollingValue(FEAT_Micro_GarmanKlass,dots_roll_buf_Micro_GarmanKlass,DotsGetFeatureValue(FEAT_Micro_GarmanKlass,1));
            DotsInsertRollingValue(FEAT_Micro_HLAsymmetry,dots_roll_buf_Micro_HLAsymmetry,DotsGetFeatureValue(FEAT_Micro_HLAsymmetry,1));
            DotsInsertRollingValue(FEAT_Micro_Hurst,dots_roll_buf_Micro_Hurst,DotsGetFeatureValue(FEAT_Micro_Hurst,1));
            DotsInsertRollingValue(FEAT_Micro_IBSP,dots_roll_buf_Micro_IBSP,DotsGetFeatureValue(FEAT_Micro_IBSP,1));
            DotsInsertRollingValue(FEAT_Micro_Lambda,dots_roll_buf_Micro_Lambda,DotsGetFeatureValue(FEAT_Micro_Lambda,1));
            DotsInsertRollingValue(FEAT_Micro_LogReturn,dots_roll_buf_Micro_LogReturn,DotsGetFeatureValue(FEAT_Micro_LogReturn,1));
            DotsInsertRollingValue(FEAT_Micro_MicroGap,dots_roll_buf_Micro_MicroGap,DotsGetFeatureValue(FEAT_Micro_MicroGap,1));
            DotsInsertRollingValue(FEAT_Micro_MomoTransfer,dots_roll_buf_Micro_MomoTransfer,DotsGetFeatureValue(FEAT_Micro_MomoTransfer,1));
            DotsInsertRollingValue(FEAT_Micro_OrderFlowDelta,dots_roll_buf_Micro_OrderFlowDelta,DotsGetFeatureValue(FEAT_Micro_OrderFlowDelta,1));
            DotsInsertRollingValue(FEAT_Micro_PriceAccel,dots_roll_buf_Micro_PriceAccel,DotsGetFeatureValue(FEAT_Micro_PriceAccel,1));
            DotsInsertRollingValue(FEAT_Micro_RangeAccel,dots_roll_buf_Micro_RangeAccel,DotsGetFeatureValue(FEAT_Micro_RangeAccel,1));
            DotsInsertRollingValue(FEAT_Micro_RangeVelocity,dots_roll_buf_Micro_RangeVelocity,DotsGetFeatureValue(FEAT_Micro_RangeVelocity,1));
            DotsInsertRollingValue(FEAT_Micro_Rejection,dots_roll_buf_Micro_Rejection,DotsGetFeatureValue(FEAT_Micro_Rejection,1));
            DotsInsertRollingValue(FEAT_Micro_RollProxy,dots_roll_buf_Micro_RollProxy,DotsGetFeatureValue(FEAT_Micro_RollProxy,1));
            DotsInsertRollingValue(FEAT_Micro_ThrustEff,dots_roll_buf_Micro_ThrustEff,DotsGetFeatureValue(FEAT_Micro_ThrustEff,1));
            DotsInsertRollingValue(FEAT_Micro_TickIntensity,dots_roll_buf_Micro_TickIntensity,DotsGetFeatureValue(FEAT_Micro_TickIntensity,1));
            DotsInsertRollingValue(FEAT_Micro_VPIN,dots_roll_buf_Micro_VPIN,DotsGetFeatureValue(FEAT_Micro_VPIN,1));
            DotsInsertRollingValue(FEAT_Micro_VolAccel,dots_roll_buf_Micro_VolAccel,DotsGetFeatureValue(FEAT_Micro_VolAccel,1));
            DotsInsertRollingValue(FEAT_Micro_VolOfVol,dots_roll_buf_Micro_VolOfVol,DotsGetFeatureValue(FEAT_Micro_VolOfVol,1));
            DotsInsertRollingValue(FEAT_Micro_WickImbalance,dots_roll_buf_Micro_WickImbalance,DotsGetFeatureValue(FEAT_Micro_WickImbalance,1));
            DotsInsertRollingValue(FEAT_VWAP_Dist_ATR,dots_roll_buf_VWAP_Dist_ATR,DotsGetFeatureValue(FEAT_VWAP_Dist_ATR,1));
            DotsInsertRollingValue(FEAT_VAH_Dist_ATR,dots_roll_buf_VAH_Dist_ATR,DotsGetFeatureValue(FEAT_VAH_Dist_ATR,1));
            DotsInsertRollingValue(FEAT_VAL_Dist_ATR,dots_roll_buf_VAL_Dist_ATR,DotsGetFeatureValue(FEAT_VAL_Dist_ATR,1));
            DotsInsertRollingValue(FEAT_PrevDay_High_Dist_ATR,dots_roll_buf_PrevDay_High_Dist_ATR,DotsGetFeatureValue(FEAT_PrevDay_High_Dist_ATR,1));
            DotsInsertRollingValue(FEAT_PrevDay_Low_Dist_ATR,dots_roll_buf_PrevDay_Low_Dist_ATR,DotsGetFeatureValue(FEAT_PrevDay_Low_Dist_ATR,1));
            DotsInsertRollingValue(FEAT_PrevDay_Close_Dist_ATR,dots_roll_buf_PrevDay_Close_Dist_ATR,DotsGetFeatureValue(FEAT_PrevDay_Close_Dist_ATR,1));
            DotsInsertRollingValue(FEAT_DailyOpen_Dist_ATR,dots_roll_buf_DailyOpen_Dist_ATR,DotsGetFeatureValue(FEAT_DailyOpen_Dist_ATR,1));
            DotsInsertRollingValue(FEAT_Round_100_Dist_ATR,dots_roll_buf_Round_100_Dist_ATR,DotsGetFeatureValue(FEAT_Round_100_Dist_ATR,1));
            DotsInsertRollingValue(FEAT_Round_500_Dist_ATR,dots_roll_buf_Round_500_Dist_ATR,DotsGetFeatureValue(FEAT_Round_500_Dist_ATR,1));
            DotsInsertRollingValue(FEAT_Round_1000_Dist_ATR,dots_roll_buf_Round_1000_Dist_ATR,DotsGetFeatureValue(FEAT_Round_1000_Dist_ATR,1));
            DotsInsertRollingValue(FEAT_OR_High_Dist_ATR,dots_roll_buf_OR_High_Dist_ATR,DotsGetFeatureValue(FEAT_OR_High_Dist_ATR,1));
            DotsInsertRollingValue(FEAT_OR_Low_Dist_ATR,dots_roll_buf_OR_Low_Dist_ATR,DotsGetFeatureValue(FEAT_OR_Low_Dist_ATR,1));
            DotsInsertRollingValue(FEAT_Session_High_Dist_ATR,dots_roll_buf_Session_High_Dist_ATR,DotsGetFeatureValue(FEAT_Session_High_Dist_ATR,1));
            DotsInsertRollingValue(FEAT_Session_Low_Dist_ATR,dots_roll_buf_Session_Low_Dist_ATR,DotsGetFeatureValue(FEAT_Session_Low_Dist_ATR,1));
            DotsInsertRollingValue(FEAT_WeeklyOpen_Dist_ATR,dots_roll_buf_WeeklyOpen_Dist_ATR,DotsGetFeatureValue(FEAT_WeeklyOpen_Dist_ATR,1));
            DotsInsertRollingValue(FEAT_MultiDay_Slope,dots_roll_buf_MultiDay_Slope,DotsGetFeatureValue(FEAT_MultiDay_Slope,1));
            DotsInsertRollingValue(FEAT_MultiDay_Position,dots_roll_buf_MultiDay_Position,DotsGetFeatureValue(FEAT_MultiDay_Position,1));
            DotsInsertRollingValue(FEAT_VWAP_Sigma_ATR,dots_roll_buf_VWAP_Sigma_ATR,DotsGetFeatureValue(FEAT_VWAP_Sigma_ATR,1));
            DotsInsertRollingValue(FEAT_VA_Position,dots_roll_buf_VA_Position,DotsGetFeatureValue(FEAT_VA_Position,1));
            DotsInsertRollingValue(FEAT_ADX_Value,dots_roll_buf_ADX_Value,DotsGetFeatureValue(FEAT_ADX_Value,1));
            DotsInsertRollingValue(FEAT_Body_Size,dots_roll_buf_Body_Size,DotsGetFeatureValue(FEAT_Body_Size,1));
            DotsInsertRollingValue(FEAT_Upper_Wick,dots_roll_buf_Upper_Wick,DotsGetFeatureValue(FEAT_Upper_Wick,1));
            DotsInsertRollingValue(FEAT_Lower_Wick,dots_roll_buf_Lower_Wick,DotsGetFeatureValue(FEAT_Lower_Wick,1));
            DotsInsertRollingValue(FEAT_TChan_A15,dots_roll_buf_TChan_A15,DotsGetFeatureValue(FEAT_TChan_A15,1));
            DotsInsertRollingValue(FEAT_VWAP_Sigma,dots_roll_buf_VWAP_Sigma,DotsGetFeatureValue(FEAT_VWAP_Sigma,1));
            DotsInsertRollingValue(FEAT_Volume,dots_roll_buf_Volume,DotsGetFeatureValue(FEAT_Volume,1));
         }
         if(TimeDay(TimeCurrent())!=dots_roll_lastRefreshDay)
            DotsRefreshRollingPercentiles();
      }
      if(UseDots) Eval_Dots_Signals(1);
      RefreshCachedData();
      CalculateDailyPoC(); 
      DrawDailyPoC();
      DrawCustomTradeHistory();
      DrawST_AdaptiveTrendChannel();
      DrawLT_AdaptiveTrendChannel();
      DrawST_TrendDirectionIndicator();
      DrawLT_TrendDirectionIndicator();
      DrawSessionBoxes();
      UpdateStatsFromHistory();
      DrawHarmonicVolumeCandles();
      UpdateLiveTrackers();
      if(isSuperTrendVisible) DrawSuperTrendLine();
      if(isOBVfriendSuperTrendVisible) DrawOBVfriendSuperTrendLine();
      if(LockBuffer[1]!=0&&LockBuffer[1]!=LockBuffer[2]) {
         LockTime[1]=Time[1];
         lastCommittedSignal=LockBuffer[1];
         lastCommittedSignalTime=Time[1];
         lastSignalPrice=Close[1];
         PlayCommittedSignalAlert(1);
         if(isSignalDotsVisible) {
            color signalColor=(lastCommittedSignal==1)?C'146,134,124':C'89,116,124';
            DrawSignalOnChart(Time[1],signalColor);
         }
         if(!showOBVfCandles) {
            color cColor=(lastCommittedSignal==1)?clrOrange:clrDodgerBlue;
            if(Chart_Visual_Mode==MODE_CANDLES) {
               DrawCustomCandle(1,cColor);
            } else {
               string name=ea_prefix+"line_sig_"+TimeToString(Time[1],TIME_DATE|TIME_MINUTES|TIME_SECONDS);
               if(ObjectFind(0,name)<0) {
                  ObjectCreate(0,name,OBJ_TREND,0,Time[2],Close[2],Time[1],Close[1]);
                  ObjectSetInteger(0,name,OBJPROP_COLOR,cColor);
                  ObjectSetInteger(0,name,OBJPROP_WIDTH,1);
                  ObjectSetInteger(0,name,OBJPROP_STYLE,STYLE_SOLID);
                  ObjectSetInteger(0,name,OBJPROP_RAY_RIGHT,false);
                  ObjectSetInteger(0,name,OBJPROP_SELECTABLE,false);
                  ObjectSetInteger(0,name,OBJPROP_BACK,false);
                  ObjectSetInteger(0,name,OBJPROP_ZORDER,1);
               }
            }
         }
      }
      if(UseDots) HandleDotsTradeSignal();
      if(UseDots&&isDotsVisualsVisible) DrawDotsPanel();
      if(UseOBVfriend) {
         int evalBar=1;
         if(evalBar<Bars) {
            int currentLiveDir=(int)MathRound(state_TChan_OC[evalBar]);
            if(currentLiveDir!=0&&currentLiveDir!=obvfriend_lastVisualDirection) {
               DrawLiveOBVfriendVLine(evalBar,currentLiveDir);
               obvfriend_lastVisualDirection=currentLiveDir;
            }
         }
      }
      DrawNewestSignalBarSegment();
      if(isSuperTrendVisible) DrawNewestSuperTrendSegment();
      if(isOBVfriendSuperTrendVisible) DrawNewestOBVfriendSuperTrendSegment();
      ChartRedraw();
   }
   Calc_PoC_State_OnBar(0); 
   Calc_Momentum_OnBar(0);
   Calc_ADX_OnBar(0);
   Calc_OBV_OnBar(0);       
   CalcATR1M(0,atrPeriod);
   Calc_D2D_ST_OnBar(0);    
   Calc_AdaptiveTrend_OnBar(0);
   Calc_OBVfriend_ST_OnBar(0);
   Calc_HarmVol_LLEMA_OnBar(0);
   Calc_Sqz_Momentum_OnBar(0);
   Calc_RangeOsc_OnBar(0);
   Calc_Microstructure_OnBar(0);
   Calc_Dots_Derived_OnBar(0);
   Calc_VWAP_OnBar(0);
   Calc_RefLevels_OnBar(0);
   Calc_MultiDay_OnBar(0);
   SyncLiveSuperTrendVisuals();
   DrawLiveSignalBarSegment();
   if(isSuperTrendVisible) DrawLiveSuperTrendSegment();
   if(isOBVfriendSuperTrendVisible) DrawLiveOBVfriendSuperTrendSegment();
   if(isOBVVisualsVisible) DrawLiveOBVSegment();
   DrawLiveST_TrendDirectionIndicator();
   DrawLiveLT_TrendDirectionIndicator();
   ManageLeapFrogSL();
   ManageOBVfriendLeapFrogSL();
   if(UseDots) ManageDotsPositions();
   bool tradeFound=false;
   bool obvfriend_tradeFound=false;
   for(int i=OrdersTotal()-1; i>=0; i--) {
      if(!OrderSelect(i,SELECT_BY_POS,MODE_TRADES)) continue;
      if(OrderSymbol()!=Symbol()) continue;
      int magic=OrderMagicNumber();
      if(!tradeFound&&magic==MagicNumber) {
         tradeFound=true;
         if(Move_To_BE) ManageCommissionNudge();
         if(D2D_UseDynamicSL) ManageTrailWithSuperTrend();
         if(D2D_UsePartialTP) ManagePartialTP();
      }
      else if(!obvfriend_tradeFound&&magic==OBVfriendMagicNumber) {
         obvfriend_tradeFound=true;
         if(Move_To_BE) ManageOBVfriendCommissionNudge();
         if(OBV_UseDynamicSL) ManageOBVfriendTrailWithSuperTrend();
         if(OBV_UsePartialTP) ManageOBVfriendPartialTP();
      }
      if(tradeFound&&obvfriend_tradeFound) break;
   }
   if(!tradeFound) {
      if(ptp_TradeTicket>0) ResetPartialTP();
      if(leap_TradeTicket>0) ResetLeapFrog();
      if(nudged_TradeTicket>0) nudged_TradeTicket=0;
   }
   if(!obvfriend_tradeFound) {
      if(obvfriend_ptp_TradeTicket>0) ResetOBVfriendPartialTP();
      if(obvfriend_leap_TradeTicket>0) ResetOBVfriendLeapFrog();
      if(obvfriend_nudged_TradeTicket>0) obvfriend_nudged_TradeTicket=0;
   }
   EnforceUIPanelLayering();
}
//+------------------------------------------------------------------+
//| SECTION 1.3 - TIDYING UP & HEALTH CHECKS                         |
//+------------------------------------------------------------------+
void CheckHeartbeat() {
   if(TimeCurrent()-lastHeartbeatTime>=60) {
      lastHeartbeatTime=TimeCurrent();
   }
}
void LogStatusUpdate() {
   if((TimeCurrent()-lastStatusLogTime)<3600) return;
   lastStatusLogTime=TimeCurrent();
}
//+------------------------------------------------------------------+
//| SECTION 1.4 - KAMA WARM-START PERSISTENCE                        |
//+------------------------------------------------------------------+
#define WARM_KAMA_MAGIC   0x6B616D77
#define WARM_KAMA_VERSION 1
#define HARMVOL_ER_PERIOD 50
extern int    Warm_KAMA_Suppress_Bars=5000;
extern int    Warm_KAMA_Deep_Bars=50000;
bool          g_warm_valid=false;
bool          g_warm_anchor_enabled=true;
datetime      g_warm_ts=0;
double        g_warm_kama=0.0;
bool          g_warm_suppress_kama_signals=false;
int           g_warm_suppress_counter=0;
bool          g_warm_pending=false;
datetime      g_warm_pending_ts=0;
double        g_warm_pending_kama=0.0;
string WarmStateFileName() {
   string sym=Symbol();
   string clean="";
   int n=StringLen(sym);
   for(int k=0; k<n; k++) {
      ushort ch=StringGetCharacter(sym,k);
      if(ch=='\\'||ch=='/'||ch==':'||ch=='*'||ch=='?'||ch=='"'||ch=='<'||ch=='>'||ch=='|') continue;
      clean=clean+CharToString((uchar)ch);
   }
   return "equiDOT_KAMA_"+clean+"_"+IntegerToString(Period())+".bin";
}
void WriteKamaWarmState(datetime barTime,double kamaVal) {
   int h=FileOpen(WarmStateFileName(),FILE_BIN|FILE_WRITE);
   if(h==INVALID_HANDLE) { Print("KAMA warm-start: could not open seed file for writing; seed not saved this bar."); return; }
   FileWriteInteger(h,WARM_KAMA_MAGIC,INT_VALUE);
   FileWriteInteger(h,WARM_KAMA_VERSION,INT_VALUE);
   FileWriteLong(h,(long)barTime);
   FileWriteDouble(h,kamaVal);
   FileClose(h);
   Print("KAMA warm-start: seed written (time=",TimeToString(barTime,TIME_DATE|TIME_MINUTES),", KAMA=",DoubleToString(kamaVal,Digits),").");
}
void ReadKamaWarmState() {
   g_warm_pending=false;
   int h=FileOpen(WarmStateFileName(),FILE_BIN|FILE_READ);
   if(h==INVALID_HANDLE) { Print("KAMA warm-start: no saved seed file found; cold start."); return; }
   int magic=FileReadInteger(h,INT_VALUE);
   FileReadInteger(h,INT_VALUE);
   if(magic!=WARM_KAMA_MAGIC) { Print("KAMA warm-start: saved seed file unreadable (magic mismatch); cold start."); FileClose(h); return; }
   g_warm_pending_ts=(datetime)FileReadLong(h);
   g_warm_pending_kama=FileReadDouble(h);
   FileClose(h);
   g_warm_pending=true;
   Print("KAMA warm-start: saved seed loaded (time=",TimeToString(g_warm_pending_ts,TIME_DATE|TIME_MINUTES),", KAMA=",DoubleToString(g_warm_pending_kama,Digits),"); validating against loaded chart.");
}
void ValidateKamaWarmState() {
   g_warm_valid=false;
   if(g_warm_pending && Bars>0) {
      int matchBar=-1;
      for(int k=0; k<Bars; k++) {
         if(Time[k]==g_warm_pending_ts) { matchBar=k; break; }
      }
      if(matchBar>=0 && matchBar<=Bars-1-HARMVOL_ER_PERIOD) {
         g_warm_valid=true;
         g_warm_ts=g_warm_pending_ts;
         g_warm_kama=g_warm_pending_kama;
         Print("KAMA warm-start: timestamp matched at bar ",matchBar,"; anchoring KAMA=",DoubleToString(g_warm_kama,Digits),".");
      } else if(matchBar>=0) {
         Print("KAMA warm-start: saved bar sits within the oldest ",HARMVOL_ER_PERIOD," loaded bars and cannot be anchored; load deeper history. KAMA cold-started and KAMA signals suppressed.");
      } else if(g_warm_pending_ts<Time[Bars-1]) {
         long secPerBar=(long)Period()*60;
         int gapBars=0;
         if(secPerBar>0) gapBars=(int)(((long)Time[Bars-1]-(long)g_warm_pending_ts)/secPerBar);
         Print("KAMA warm-start: saved state predates loaded history by ~",gapBars," bars; increase chart depth to span the gap to restore warm-start.");
      } else if(g_warm_pending_ts>Time[0]) {
         Print("KAMA warm-start: saved timestamp is newer than the loaded history; state ignored, KAMA cold-started and KAMA signals suppressed.");
      } else {
         Print("KAMA warm-start: saved timestamp not found in loaded history (possible session-time shift or data gap); KAMA cold-started and KAMA signals suppressed.");
      }
   }
   if(g_warm_valid) {
      g_warm_suppress_kama_signals=false;
      g_warm_suppress_counter=0;
      Print("KAMA warm-start: WARM start active - all 76 signals enabled from bar 1.");
   } else {
      g_warm_suppress_kama_signals=true;
      g_warm_suppress_counter=Warm_KAMA_Suppress_Bars;
      Print("KAMA warm-start: COLD start - 13 KAMA-dependent signals suppressed this session (63 of 76 active).");
   }
}
//+------------------------------------------------------------------+
//| SECTION 2.0 - USER SETTINGS                                      |
//+------------------------------------------------------------------+
extern string S_Trade_Settings="--- TRADE SETTINGS ---";
enum ENUM_CHART_MODE_VARIANT { 
   MODE_CANDLES=0, 
   MODE_LINE=1 
};
extern ENUM_CHART_MODE_VARIANT Chart_Visual_Mode=MODE_CANDLES;
extern string OrderExecutionType="Market";
extern double BaseLotSize=0.2;
extern double MaxLotSize=5.0;
extern int Slippage=10;
extern string S_Tier_Matrix="--- TIER MATRIX SETTINGS ---";
extern double Tier1_Vol=100;
extern double Tier1_Mult=0.0;
extern double Tier2_Vol=200;
extern double Tier2_Mult=0.0;
extern double Tier3_Vol=300;
extern double Tier3_Mult=0.0;
extern double Tier4_Vol=325;
extern double Tier4_Mult=0.0;
extern double Tier5_Vol=350;
extern double Tier5_Mult=0.0;
extern double Tier6_Vol=375;
extern double Tier6_Mult=0.0;
extern double Tier7_Vol=400;
extern double Tier7_Mult=1.0;
extern double Tier8_Vol=425;
extern double Tier8_Mult=2.0;
extern double Tier9_Vol=450;
extern double Tier9_Mult=4.0;
extern double Tier10_Vol=475;
extern double Tier10_Mult=6.0;
extern double Tier11_Vol=500;
extern double Tier11_Mult=8.0;
extern double Tier12_Vol=525;
extern double Tier12_Mult=10.0;
extern bool UseVolumeMatrixMultiplier=false;
extern string S_Safety_Settings="--- SAFETY SETTINGS ---";
enum ENUM_LOCK_MODE { 
   LOCKS_ENABLED=0, 
   LOCKS_DISABLED=1 
};
extern ENUM_LOCK_MODE Panel_Lock_Security=LOCKS_ENABLED;
extern double MaxDailyLoss=2500.0;
extern double MaxSpreadUSD=10.0;
extern bool Move_To_BE=true;
extern double Trigger_At_Risk_Ratio=1.0;
enum ENUM_BE_PROFIT_PERCENT { 
   BE_PCT_0_25=25, 
   BE_PCT_0_50=50, 
   BE_PCT_1_00=100, 
   BE_PCT_2_00=200, 
   BE_PCT_5_00=500, 
   BE_PCT_10_00=1000, 
   BE_PCT_20_00=2000,
   BE_PCT_25_00=2500 
};
extern ENUM_BE_PROFIT_PERCENT Commission_Allowance_Percent=BE_PCT_25_00;
extern string S_Adaptive_Trends="--- ADAPTIVE TRENDS ---";
extern bool UseAdaptiveTrendFilter=false;
enum ENUM_T2T_SCORE {
   SCORE_0,
   SCORE_0_PLUS,
   SCORE_1,
   SCORE_2,
   SCORE_3,
   SCORE_4,
   SCORE_5
};
extern ENUM_T2T_SCORE T2T_Standard_Flip_Score=SCORE_0;
extern ENUM_T2T_SCORE T2T_Strong_Flip_Score=SCORE_0_PLUS;
extern bool UseTrendAnchor=false;
extern double Decay_Accel_ATR_Deadzone=1.0;
extern string S_Uptrick_Settings="--- D2D 'UPTRICK' BRAIN ---";
extern int U_emaLen=13;
extern int U_atrLen=5;
extern double U_baseMult=1.5;
extern double U_sensExp=1.25;
extern int U_persLen=7;
extern double U_persGain=0.60;
extern double U_multMin=0.8;
extern double U_multMax=2.5;
extern int U_confirmN=1;
extern bool U_UseTrueRangeCapping=false;
extern double U_TrueRangeCapPercentile=0.88;
extern string S_Dynamic_Brain="--- DYNAMIC OBV MATRIX ---";
extern bool UseDynamicMatrix=true;
extern double ConcordanceMult=2.0;
extern double DiscordanceMult=0.5;
extern double DiscordanceExtreme=0.35;
extern double POC_Safety_Zone=15.0;
extern int Slope_Lookback=5;
extern double Slope_Recovery_Mult=1.2;
extern double Slope_Noise_Floor=20.0;
extern string S_D2D_Trade_Management="--- D2D TRADE MANAGEMENT ---";
extern bool D2D_UseDynamicSL=true;
extern double D2D_SL_Buffer_Mult=1.0;
extern double D2D_SL_Contraction_Mult=0.0;
extern bool D2D_Leap_Frog_SL=true;
extern double D2D_Leap_Frog_Risk_Mult=0.5;
extern double D2D_Leap_Frog_ATR_Mult=0.0;
extern int D2D_Leap_Frog_Lag=2;
extern bool D2D_SL_Follows_Tiers=true;
extern bool D2D_UsePartialTP=false;
extern double D2D_PartialTP_Step_Risk_Mult=0.5;
extern double D2D_PartialTP_Step_ATR_Mult=0.0;
extern int D2D_PTP_Lag=2;
enum ENUM_D2D_PARTIAL_TP_PERCENT { 
   D2D_PCT_5=5, 
   D2D_PCT_10=10, 
   D2D_PCT_20=20, 
   D2D_PCT_25=25, 
   D2D_PCT_50=50, 
   D2D_PCT_100=100 
};
extern ENUM_D2D_PARTIAL_TP_PERCENT D2D_PartialTP_Percent=D2D_PCT_10;
extern string S_OBVfriend_Settings="--- OBVFRIEND BRAIN SETTINGS ---";
extern int OBV_emaLen=13;
extern int OBV_atrLen=5;
extern double OBV_baseMult=1.5;
extern double OBV_sensExp=1.25;
extern int OBV_persLen=7;
extern double OBV_persGain=0.60;
extern double OBV_multMin=0.8;
extern double OBV_multMax=2.5;
extern int OBV_confirmN=1;
extern bool OBV_UseTrueRangeCapping=false;
extern double OBV_TrueRangeCapPercentile=0.88;
extern int OBV_Fast_MA=21;
extern int OBV_Slow_MA=144;
extern string S_OBVfriend_Management="--- OBVFRIEND TRADE MANAGEMENT ---";
extern bool UseOBVfriend=false;
extern bool OBV_UseDynamicSL=true;
extern double OBV_SL_Buffer_Mult=1.0;
extern double OBV_SL_Contraction_Mult=0.0;
extern bool OBV_Leap_Frog_SL=true;
extern double OBV_Leap_Frog_Risk_Mult=0.5;
extern double OBV_Leap_Frog_ATR_Mult=0.0;
extern int OBV_Leap_Frog_Lag=2;
extern bool OBV_SL_Follows_Tiers=true;
extern int OBV_PTP_Lag=2;
extern bool OBV_UsePartialTP=false;
extern double OBV_PartialTP_Step_Risk_Mult=0.5;
extern double OBV_PartialTP_Step_ATR_Mult=0.0;
enum ENUM_OBV_PARTIAL_TP_PERCENT { 
   OBV_PCT_5=5, 
   OBV_PCT_10=10, 
   OBV_PCT_20=20, 
   OBV_PCT_25=25, 
   OBV_PCT_50=50, 
   OBV_PCT_100=100 
};
extern ENUM_OBV_PARTIAL_TP_PERCENT OBV_PartialTP_Percent=OBV_PCT_10;
extern string S_Visuals="--- CHART VISUALS ---";
extern bool trendDirection=true;
extern bool ShowADXIndicator=true;
extern bool ShowD2DSuperTrend=true;
extern bool ShowOBVfriendSuperTrend=false;
extern bool ShowHarmonicVolumeCandles=true;
enum ENUM_KAMA_DEADZONE {
   KAMA_DZ_CASUAL=0,
   KAMA_DZ_BALANCED=1,
   KAMA_DZ_AGGRESSIVE=2
};
extern ENUM_KAMA_DEADZONE KAMA_DeadZone_Sensitivity=KAMA_DZ_AGGRESSIVE;
extern string S_Sqz_Settings="--- SQUEEZE MOMENTUM ---";
extern int Sqz_BB_Length=14;
extern double Sqz_BB_Mult=2.0;
extern int Sqz_KC_Length=14;
extern double Sqz_KC_Mult=1.75;
extern bool Sqz_UseTrueRange=true;
extern string S_Open_Hours="--- TRADING SESSIONS (EST) ---";
extern bool UseOpenHours=false;
extern int OpenHourEST=8;
extern int OpenMinuteEST=0;
extern int CloseHourEST=15;
extern int CloseMinuteEST=50;
extern bool ForceOpenHoursTrade=false;
extern bool UseBlockTime=false;
extern int BlockStartHourEST=10;
extern int BlockStartMinuteEST=30;
extern int BlockEndHourEST=14;
extern int BlockEndMinuteEST=0;
extern string S_Open_Days="--- TRADING DAYS ---";
extern bool TradeOnSunday=true;
extern bool TradeOnMonday=true;
extern bool TradeOnTuesday=true;
extern bool TradeOnWednesday=true;
extern bool TradeOnThursday=true;
extern bool TradeOnFriday=true;
extern string S_Manual_Trades="--- MANUAL TRADE BUTTONS ---";
extern double Manual_Buy_Lot_1=0.01;
extern double Manual_Buy_Lot_2=0.10;
extern double Manual_Sell_Lot_1=0.01;
extern double Manual_Sell_Lot_2=0.10;
extern string S_Dots_Settings="--- DOTS SYSTEM ---";
extern bool   UseDots=true;
extern double Dots_LotSize=2.0;
extern int    Dots_MaxPositions=6;
extern double Dots_Spread=3.0;
extern double Dots_SL_Mult=2.0;
extern double Dots_SL_Cap=150.0;
extern double Dots_StepFrac=0.30;
extern double Dots_BE_Trigger=1.0;
extern double Dots_BE_LockFrac=1.0;
extern bool Dots_ExportThresholds=false;
extern int    Dots_LF_Lag=2;
extern int    Dots_LF_Activation=3;
extern int    Dots_SoloVolumeGate=300;
extern int    Dots_MinConcurrent=2;
extern int    Dots_RollingBufferSize=2500;
extern int    Dots_InitBars=6900;
//+------------------------------------------------------------------+
//| SECTION 2.1 - GLOBAL TIMING MODULE                               |
//+------------------------------------------------------------------+
bool _IsUSDST(datetime gmtTime) {
   MqlDateTime dt;
   ZeroMemory(dt);
   TimeToStruct(gmtTime,dt);
   int year=dt.year;
   MqlDateTime dstStart;
   ZeroMemory(dstStart);
   dstStart.year=year;
   dstStart.mon=3;
   dstStart.day=1;
   StructToTime(dstStart);
   int firstDayOfWeek=dstStart.day_of_week;
   int firstSunday=(7-firstDayOfWeek)%7+1;
   int secondSunday=firstSunday+7;
   dstStart.day=secondSunday;
   dstStart.hour=2;
   datetime dstStartTime=StructToTime(dstStart);
   MqlDateTime dstEnd;
   ZeroMemory(dstEnd);
   dstEnd.year=year;
   dstEnd.mon=11;
   dstEnd.day=1;
   StructToTime(dstEnd);
   firstDayOfWeek=dstEnd.day_of_week;
   firstSunday=(7-firstDayOfWeek)%7+1;
   dstEnd.day=firstSunday;
   dstEnd.hour=2;
   datetime dstEndTime=StructToTime(dstEnd);
   if(gmtTime>=dstStartTime&&gmtTime<dstEndTime) {
      return true;
   }
   return false;
}
long _GetEstOffsetForTime(datetime gmtTime) {
   if(_IsUSDST(gmtTime)) {
      return (long)(-4*3600);
   }
   return (long)(-5*3600);
}
long GetUSEasternOffsetSeconds() {
   return _GetEstOffsetForTime(TimeGMT());
}
datetime GetEstTime() {
   return (datetime)(TimeGMT()+(datetime)GetUSEasternOffsetSeconds());
}
int GetEstDayOfWeek(datetime gmtTime) {
   long offset=_GetEstOffsetForTime(gmtTime);
   MqlDateTime dt;
   ZeroMemory(dt);
   TimeToStruct(gmtTime+(datetime)offset,dt);
   return dt.day_of_week;
}
//+------------------------------------------------------------------+
//| SECTION 3.0 - GLOBAL VARIABLES & MEMORY                          |
//+------------------------------------------------------------------+
double aus_session_pnl=0.0;
double asia_session_pnl=0.0;
double london_session_pnl=0.0;
double ny_session_pnl=0.0;
double atr_min_historical=0.0;
double atr_max_historical=0.0;
double momo_min_historical=0.0;
double momo_max_historical=0.0;
double adx_min_historical=0.0;
double adx_max_historical=0.0;
double latest_dynamic_factor=0.0;
double latest_d2d_dynamic_factor=0.0;
int MagicNumber;
int EA_ID;
string ea_prefix="";
double UpTrendBuffer[];
double DownTrendBuffer[];
int LockBuffer[];
datetime LockTime[];
double ATR_1M_Array[];
double assignedATR[];
double UpperBand[];
double LowerBand[];
double SuperTrend[];
int Direction[];
double tr_cap=0.0;
bool isLoomsActive=false;
bool isLocked=true;
bool isSuperTrendVisible=false;
bool isTradeHistoryVisible=true;
bool showOBVfCandles=true;
bool isHarmonicVolVisible=true;
int lastCommittedSignal=0;
datetime lastCommittedSignalTime=0;
int lastCommittedSignalIndex=-1;
datetime d2d_lastTradedSignalTime=0;
datetime obvfriend_lastTradedSignalTime=0;
datetime activationTime=0;
datetime prevSignalAlertTime=0;
datetime prevTrendFlipAlertTime=0;
datetime prevOBVAlertTime=0;
int lastOBVAlertDirection=0;
bool isPanelVisible=true;
bool g_isLoading=false;
double g_loadingProgress=0.0;
uint g_loadingStartTime=0;
datetime g_loadingBarHideTime=0;
double lastSignalPrice=0.0;
int wins=0;
int losses=0;
double profitTotal=0.0;
double lossTotal=0.0;
double totalCommissions=0.0;
double currentDailyLoss=0.0;
double combinedCurrentDailyLoss=0.0;
double dailyBestWin=0.0;
double dailyBestLoss=0.0;
int currentDay=0;
datetime statsResetTime=0;
datetime lastProcessedLossTime=0;
double lastClosedTradeProfit=0.0;
double lastClosedTradeClosePrice=0.0;
double liveLockedInProfit=0.0;
double historicalProfitSecured=0.0;
datetime lastBarTime=0;
int lastCalculatedBars=0;
datetime lastHeartbeatTime;
datetime lastStatusLogTime;
datetime lastUIRefreshTime=0;
double cachedTickValue=0.0;
double cachedTickSize=0.0;
double cachedStopLevel=0.0;
double cachedSpreadInPoints=0.0;
int drawnHistoryTickets[];
double ptp_ProfitStep=0.0;
double ptp_NextTargetPrice=0.0;
double ptp_InitialLotSize=0.0;
int ptp_TradeTicket=0;
datetime ptp_OrderOpenTime=0;
int ptp_TiersTaken=0;
int nudged_TradeTicket=0;
double leap_ProfitStep=0.0;
double leap_NextTargetPrice=0.0;
int leap_TradeTicket=0;
datetime leap_OrderOpenTime=0;
int leap_TiersReached=0;
int ManualMagicNumber;
bool isManualPanelLocked=true;
datetime manualPanelUnlockTime=0;
datetime statsPanelUnlockTime=0;
int manual_wins=0;
int manual_losses=0;
double manual_profitTotal=0.0;
double manual_lossTotal=0.0;
double manual_totalCommissions=0.0;
double manual_currentDailyLoss=0.0;
double manual_lastClosedTradeProfit=0.0;
double manual_lastClosedTradeClosePrice=0.0;
datetime manual_statsResetTime=0;
int OBVfriendMagicNumber;
int DotsMagicNumber;
int obvfriend_wins=0;
int obvfriend_losses=0;
double obvfriend_profitTotal=0.0;
double obvfriend_lossTotal=0.0;
double obvfriend_totalCommissions=0.0;
double obvfriend_currentDailyLoss=0.0;
double obvfriend_lastClosedTradeProfit=0.0;
double obvfriend_lastClosedTradeClosePrice=0.0;
datetime obvfriend_statsResetTime=0;
datetime obvfriend_lastProcessedLossTime=0;
bool isOBVfriendPanelLocked=true;
bool isOBVfriendSuperTrendVisible=false;
datetime obvfriendPanelUnlockTime=0;
double obvfriend_ptp_ProfitStep=0.0;
double obvfriend_ptp_NextTargetPrice=0.0;
double obvfriend_ptp_InitialLotSize=0.0;
int obvfriend_ptp_TradeTicket=0;
datetime obvfriend_ptp_OrderOpenTime=0;
int obvfriend_ptp_TiersTaken=0;
double obvfriend_leap_ProfitStep=0.0;
double obvfriend_leap_NextTargetPrice=0.0;
int obvfriend_leap_TradeTicket=0;
datetime obvfriend_leap_OrderOpenTime=0;
int obvfriend_leap_TiersReached=0;
int obvfriend_nudged_TradeTicket=0;
int obvfriend_lastTradedDirection=0;
int obvfriend_lastVisualDirection=0;
double OBVfriend_UpTrendBuffer[];
double OBVfriend_DownTrendBuffer[];
double OBVfriend_SuperTrend[];
int OBVfriend_Direction[];
double OBV_BasisBuffer[];
double OBV_AtrBuffer[];
double OBV_AtrMaBuffer[];
double OBV_DirStepBuffer[];
double OBV_DirStepCountBuffer[];
double OBV_PersistBuffer[];
double OBV_UpperBandBuffer[];
double OBV_LowerBandBuffer[];
double U_BasisBuffer[];
double U_AtrBuffer[];
double U_AtrMaBuffer[];
double U_DirStepBuffer[];
double U_PersistBuffer[];
double U_UpperBandBuffer[];
double U_LowerBandBuffer[];
int U_UpCntBuffer[];
int U_DnCntBuffer[];
double Master_detectedSlope_ST=0.0;
double Master_LT_detectedSlope_ST=0.0;
double detectedSlope_ST=0.0;
double detectedIntercept_ST=0.0;
double detectedStdDev_ST=0.0;
int detectedPeriod_ST=0;
int detectedAnchorBar_ST=0;
string detectedTrendStrength_ST="N/A";
double detectedAngle_ST=90.0;
double LT_detectedSlope_ST=0.0;
double LT_detectedIntercept_ST=0.0;
double LT_detectedStdDev_ST=0.0;
int LT_detectedPeriod_ST=0;
int LT_detectedAnchorBar_ST=0;
string LT_detectedTrendStrength_ST="N/A";
int live_DecayState_ST=0;
int live_DecayState_LT=0;
double live_SlopeAccel_ST=0.0;
double live_SlopeAccel_LT=0.0;
double MomentumBuffer[];
double ADXBuffer[];
double ADX_SmoothedPlusDM[];
double ADX_SmoothedMinusDM[];
double ADX_SmoothedTR[];
double latest_momentum_value=0.0;
double dailyPoCPrice=0.0;
double dailyVAHPrice=0.0;
double dailyVALPrice=0.0;
int hist_LT_trendStep_ST[];
double hist_LT_detectedSlope_ST[];
int hist_trendStep_ST[];
double hist_detectedSlope_ST[];
double Master_hist_LT_detectedSlope_ST[];
double Master_hist_detectedSlope_ST[];
int hist_detectedAnchorBar_ST[];
int hist_LT_detectedAnchorBar_ST[];
int hist_DecayState_ST[];
int hist_DecayState_LT[];
double hist_ADXValue[];
double hist_VolumeValue[];
int hist_ST_Flip_Event[];
bool initialPaintComplete=false;
bool isSessionVisualsVisible=true;
bool isTrendVisualsVisible=false;
bool isOBVVisualsVisible=true;
bool isADXIndicatorVisible=true;
bool isVolumeVisualsVisible=true;
bool isSignalDotsVisible=false;
bool isPocVisualsVisible=false;
bool isOBVfLineVisible=true;
bool isPriceTrackerVisible=true;
bool isKamaHistoVisible=true;
bool isRangeOscVisible=true;
int static_ST_AnchorBar_Live=-1;
int static_ST_AnchorType_Live=-1;
double static_ST_AnchorValue_Live=0.0;
int static_LT_AnchorBar_Live=-1;
int static_LT_AnchorType_Live=-1;
double static_LT_AnchorValue_Live=0.0;
int static_ST_barsSinceReset_Live=0;
int static_LT_barsSinceReset_Live=0;
int hist_AnchorType_ST[];
int hist_BarsSinceFlip_ST[];
int hist_AnchorType_LT[];
int hist_BarsSinceFlip_LT[];
double state_ADX[];
double state_Momentum[];
double state_D2D_Upper[];
double state_D2D_Lower[];
int state_D2D_Dir[];
double state_OBV_Accum[];
double state_OBV_Fast[];
double state_OBV_Slow[];
double state_OBV_Macd[];
double state_OBV_Final[];
double state_OBV_Velocity[];
double state_TChan_Sum[];
double state_TChan_B5[];
double state_TChan_OC[];
double state_HarmVol_LLEMA[];
double state_HarmVol_KAMA[];
double state_HarmVol_EMA8[];
double state_HarmVol_EMA21[];
double state_HarmVol_EMAOsc[];
double state_Sqz_BB_Basis[];
double state_Sqz_BB_Dev[];
double state_Sqz_KC_RangeMa[];
double state_Sqz_Detrended[];
double state_Sqz_Val[];
int state_Sqz_State[];
double state_RangeOsc_MA[];
double state_RangeOsc_ATR[];
double state_RangeOsc_Val[];
int state_RangeOsc_State[];
double state_Slope_EMA_ST[];
double state_Slope_EMA_LT[];
double state_Slope_Accel_ST[];
double state_Slope_Accel_LT[];
double state_Micro_IBSP[];
double state_Micro_Lambda[];
double state_Micro_TickIntensity[];
double state_Micro_GarmanKlass[];
double state_Micro_Rejection[];
double state_Micro_OrderFlowDelta[];
double state_Micro_BarEntropy[];
double state_Micro_LogReturn[];
double state_Micro_PriceAccel[];
double state_Micro_RollProxy[];
double state_Micro_BarOverlap[];
double state_Micro_FailedBreak[];
double state_Micro_MomoTransfer[];
double state_Micro_MicroGap[];
double state_Micro_HLAsymmetry[];
double state_Micro_VolAccel[];
double state_Micro_RangeVelocity[];
double state_Micro_RangeAccel[];
double state_Micro_ThrustEff[];
double state_Micro_AutoCorr[];
double state_Micro_Entropy[];
double state_Micro_VPIN[];
double state_Micro_FractalDim[];
double state_Micro_VolOfVol[];
double state_Micro_Amihud[];
double state_Micro_WickImbalance[];
double state_Micro_CSSpread[];
double state_Micro_Hurst[];
double hist_Brain_Sensitivity[];
double hist_Poc_Price[];
double hist_OBV_Zero_Value[];
double hist_Efficiency_Ratio[];
double hist_UpperWick[];
double hist_VolumeRatio10[];
double hist_KAMA_Slope[];
double hist_KAMA_Dist_ATR[];
int LevelDayAnchorHourEST=18;
double hist_VWAP_Price[];
double hist_VWAP_Sigma[];
double hist_VAH_Price[];
double hist_VAL_Price[];
double hist_PrevDay_High[];
double hist_PrevDay_Low[];
double hist_PrevDay_Close[];
double hist_DailyOpen_Price[];
double hist_OR_High[];
double hist_OR_Low[];
double hist_Session_High[];
double hist_Session_Low[];
double hist_WeeklyOpen_Price[];
double hist_MultiDay_Slope[];
double hist_MultiDay_Position[];
double atr_ema1[];
double atr_ema2[];
double atr_ema3[];
double atr_final_val[];
string dy_Posture="Init";
string dy_OBV_State="Wait";
string dy_Vol_State="Wait";
string dy_POC_State="Wait";
int atrPeriod=7;
int trainingBars=100;
int ClusteringLookback=1000;
int ADX_Period=14;
bool useDynamicSTFactor=true;
int ADX_Trending_Threshold=50;
int ADX_Ranging_Threshold=20;
double Factor_Static=2.5;
double Factor_Min_Fast=1.0;
double Factor_Max_Slow=6.0;
struct DotsRuleState {
   int     ruleIndex;
   int     ticket;
   int     direction;
   double  entryPrice;
   double  initialRisk;
   double  stepSize;
   double  currentSL;
   double  be_trigger;
   double  be_lock_dist;
   int     tiersReached;
   bool    beNudged;
   bool    condA;
   bool    condB;
   bool    condC;
};
DotsRuleState dots_state[DOTS_NUM_RULES];
int    dots_today_wins=0;
int    dots_today_losses=0;
double dots_today_pnl=0.0;
int    dots_total_trades=0;
int    dots_active_count=0;
int    dots_lastPercentileCalcDay=0;
bool   isDotsVisualsVisible=true;
bool   isDotsTradeActive=false;
int    dots_today_sl=0;
int    dots_today_feat=0;
int    dots_today_time=0;
datetime dots_lastAlertTime[DOTS_NUM_RULES];
string dots_ruleName[DOTS_NUM_RULES];
int dots_ruleDir[DOTS_NUM_RULES];
int    dots_hist_wins=0;
int    dots_hist_losses=0;
double dots_hist_profitTotal=0.0;
double dots_hist_lossTotal=0.0;
double dots_hist_totalCommissions=0.0;
double dots_hist_currentDailyLoss=0.0;
double dots_hist_lastClosedProfit=0.0;
double dots_hist_lastClosedPrice=0.0;
datetime dots_hist_statsResetTime=0;
int    dots_rule_wins[DOTS_NUM_RULES];
int    dots_rule_losses[DOTS_NUM_RULES];
double dots_rule_pnl[DOTS_NUM_RULES];
bool   dots_dailySummaryPrinted=false;
int    dots_dailySummaryDay=0;
double dots_worst_intraday_drawdown=0.0;
double dots_peak_daily_equity=0.0;
//+------------------------------------------------------------------+
//| SECTION 4.0 - TEMA-ATR CALCULATION                               |
//+------------------------------------------------------------------+
double CalcATR1M(int index,int period) {
   int limit=ArraySize(atr_final_val);
   if(index<0||index>=limit||index+period>=limit) return 0.0;
   double tr;
   double hl=High[index]-Low[index];
   double hc=(index+1<limit)?MathAbs(High[index]-Close[index+1]):hl;
   double lc=(index+1<limit)?MathAbs(Low[index]-Close[index+1]):hl;
   tr=MathMax(hl,MathMax(hc,lc));
   if((U_UseTrueRangeCapping||OBV_UseTrueRangeCapping)&&tr_cap>0.0) {
      tr=MathMin(tr,tr_cap);
   }
   double alpha=2.0/(period+1.0);
   if(index+1>=limit) atr_ema1[index]=tr;
   else atr_ema1[index]=tr*alpha+atr_ema1[index+1]*(1.0-alpha);
   if(index+1>=limit) atr_ema2[index]=atr_ema1[index];
   else atr_ema2[index]=atr_ema1[index]*alpha+atr_ema2[index+1]*(1.0-alpha);
   if(index+1>=limit) atr_ema3[index]=atr_ema2[index];
   else atr_ema3[index]=atr_ema2[index]*alpha+atr_ema3[index+1]*(1.0-alpha);
   double tema_atr=3.0*atr_ema1[index]-3.0*atr_ema2[index]+atr_ema3[index];
   atr_final_val[index]=tema_atr;
   ATR_1M_Array[index]=atr_final_val[index];
   assignedATR[index]=atr_final_val[index];
   return atr_final_val[index];
}
//+------------------------------------------------------------------+
//| SECTION 4.1 - TRUE RANGE CAPPING                                 |
//+------------------------------------------------------------------+
void CalculateTrueRangeCap() {
   double effectivePercentile=0.0;
   if(U_UseTrueRangeCapping&&U_TrueRangeCapPercentile>0&&U_TrueRangeCapPercentile<1.0) {
      effectivePercentile=U_TrueRangeCapPercentile;
   }
   if(OBV_UseTrueRangeCapping&&OBV_TrueRangeCapPercentile>0&&OBV_TrueRangeCapPercentile<1.0) {
      if(effectivePercentile==0.0||OBV_TrueRangeCapPercentile<effectivePercentile) effectivePercentile=OBV_TrueRangeCapPercentile;
   }
   if(effectivePercentile==0.0) {
      tr_cap=0.0;
      return;
   }
   int lookback=ClusteringLookback;
   int start_bar=trainingBars;
   int end_bar=start_bar+lookback;
   if(end_bar>=Bars) {
      Print("Warning: Not enough bars to calculate dynamic TR cap. Capping disabled.");
      if(g_isLoading) LogBootMessage("TR Cap Failed: Low Bars");
      tr_cap=0.0;
      return;
   }
   double samples[];
   ArrayResize(samples,lookback);
   int count=0;
   for(int i=start_bar; i<end_bar; i++) {
      double hl=High[i]-Low[i];
      double hc=MathAbs(High[i]-Close[i+1]);
      double lc=MathAbs(Low[i]-Close[i+1]);
      double true_range=MathMax(hl,MathMax(hc,lc));
      if(true_range>0) samples[count++]=true_range;
   }
   if(count<50) {
      tr_cap=0.0;
      return;
   }
   ArrayResize(samples,count);
   ArraySort(samples);
   int percentile_index=(int)(count*effectivePercentile);
   if(percentile_index>=count) percentile_index=count-1;
   tr_cap=samples[percentile_index];
   Print("Dynamic True Range Capping initialized. Cap: ",DoubleToString(tr_cap,Digits));
   if(g_isLoading) LogBootMessage("TR Cap Set: "+DoubleToString(tr_cap,Digits));
}
//+------------------------------------------------------------------+
//| SECTION 5.0 - ADX CLASSIFICATION                                 |
//+------------------------------------------------------------------+
void InitADXClassifier() {
   int lookback=1440;
   int requiredBars=lookback+trainingBars+ADX_Period+5;
   if(Bars<requiredBars) {
      Print("InitADXClassifier(): Not enough bars.");
      LogBootMessage("ADX Norm: Low Data");
      return;
   }
   Print("Initializing ADX Normalizer (1440 Bar Lookback)...");
   double samples[];
   int count=0;
   ArrayResize(samples,lookback);
   for(int b=trainingBars; b<trainingBars+lookback; b++) {
      if(b>=Bars) continue;
      double adx_val=ADXBuffer[b];
      if(adx_val>0) samples[count++]=adx_val;
   }
   if(count<50) {
      Print("ADX Normalizer failed: Insufficient samples.");
      LogBootMessage("ADX Norm: Fail (Samples)");
      return;
   }
   ArrayResize(samples,count);
   ArraySort(samples);
   if(count>0) {
      adx_min_historical=samples[(int)(count*0.01)];
      adx_max_historical=samples[(int)(count*0.99)];
   }
   Print("ADX Normalizer Initialized (1st-99th Percentile): Min=",DoubleToString(adx_min_historical,2),", Max=",DoubleToString(adx_max_historical,2));
   if(g_isLoading) LogBootMessage("ADX: "+DoubleToString(adx_min_historical,1)+"-"+DoubleToString(adx_max_historical,1));
}
//+------------------------------------------------------------------+
//| SECTION 5.1 - MOMENTUM CLASSIFICATION                            |
//+------------------------------------------------------------------+
void InitMomentumNormalizer() {
   int requiredBars=ClusteringLookback+trainingBars+15;
   if(Bars<requiredBars) {
      Print("InitMomentumNormalizer(): Not enough bars.");
      LogBootMessage("Momo Norm: Low Data");
      return;
   }
   Print("Initializing Momentum Normalizer...");
   double samples[];
   int count=0;
   ArrayResize(samples,ClusteringLookback);
   for(int b=trainingBars; b<trainingBars+ClusteringLookback; b++) {
      double mom_val=MomentumBuffer[b];
      if(mom_val!=0) samples[count++]=mom_val;
   }
   if(count<50) {
      Print("Momentum Normalizer failed: Insufficient samples.");
      LogBootMessage("Momo Norm: Fail (Samples)");
      return;
   }
   ArrayResize(samples,count);
   ArraySort(samples);
   if(count>0) {
      momo_min_historical=samples[0];
      momo_max_historical=samples[count-1];
   }
   Print("Momentum Normalizer Initialized: Min=",DoubleToString(momo_min_historical,6),", Max=",DoubleToString(momo_max_historical,6));
   if(g_isLoading) LogBootMessage("Momo: "+DoubleToString(momo_min_historical,2)+"-"+DoubleToString(momo_max_historical,2));
}
//+------------------------------------------------------------------+
//| SECTION 6.0 - THE CALCULATION ENGINE HELPERS (STATEFUL)          |
//+------------------------------------------------------------------+
void ResizeAllArrays(int newSize,bool forceShift=false) {
   if(g_isLoading) LogBootMessage("Memory: Chain Alloc "+IntegerToString(newSize));
   ResizeAndSmartShift(UpTrendBuffer,newSize,forceShift);
   ResizeAndSmartShift(DownTrendBuffer,newSize,forceShift);
   ResizeAndSmartShift(LockBuffer,newSize,forceShift);
   ResizeAndSmartShift(LockTime,newSize,forceShift);
   ResizeAndSmartShift(UpperBand,newSize,forceShift);
   ResizeAndSmartShift(LowerBand,newSize,forceShift);
   ResizeAndSmartShift(SuperTrend,newSize,forceShift);
   ResizeAndSmartShift(Direction,newSize,forceShift);
   ResizeAndSmartShift(assignedATR,newSize,forceShift);
   ResizeAndSmartShift(ATR_1M_Array,newSize,forceShift);
   ResizeAndSmartShift(MomentumBuffer,newSize,forceShift);
   ResizeAndSmartShift(ADXBuffer,newSize,forceShift);
   ResizeAndSmartShift(ADX_SmoothedPlusDM,newSize,forceShift);
   ResizeAndSmartShift(ADX_SmoothedMinusDM,newSize,forceShift);
   ResizeAndSmartShift(ADX_SmoothedTR,newSize,forceShift);
   ResizeAndSmartShift(OBVfriend_UpTrendBuffer,newSize,forceShift);
   ResizeAndSmartShift(OBVfriend_DownTrendBuffer,newSize,forceShift);
   ResizeAndSmartShift(OBVfriend_SuperTrend,newSize,forceShift);
   ResizeAndSmartShift(OBVfriend_Direction,newSize,forceShift);
   ResizeAndSmartShift(OBV_BasisBuffer,newSize,forceShift);
   ResizeAndSmartShift(OBV_AtrBuffer,newSize,forceShift);
   ResizeAndSmartShift(OBV_AtrMaBuffer,newSize,forceShift);
   ResizeAndSmartShift(OBV_DirStepBuffer,newSize,forceShift);
   ResizeAndSmartShift(OBV_DirStepCountBuffer,newSize,forceShift);
   ResizeAndSmartShift(OBV_PersistBuffer,newSize,forceShift);
   ResizeAndSmartShift(OBV_UpperBandBuffer,newSize,forceShift);
   ResizeAndSmartShift(OBV_LowerBandBuffer,newSize,forceShift);
   ResizeAndSmartShift(U_BasisBuffer,newSize,forceShift);
   ResizeAndSmartShift(U_AtrBuffer,newSize,forceShift);
   ResizeAndSmartShift(U_AtrMaBuffer,newSize,forceShift);
   ResizeAndSmartShift(U_DirStepBuffer,newSize,forceShift);
   ResizeAndSmartShift(U_PersistBuffer,newSize,forceShift);
   ResizeAndSmartShift(U_UpperBandBuffer,newSize,forceShift);
   ResizeAndSmartShift(U_LowerBandBuffer,newSize,forceShift);
   ResizeAndSmartShift(U_UpCntBuffer,newSize,forceShift);
   ResizeAndSmartShift(U_DnCntBuffer,newSize,forceShift);
   ResizeAndSmartShift(hist_LT_trendStep_ST,newSize,forceShift);
   ResizeAndSmartShift(hist_LT_detectedSlope_ST,newSize,forceShift);
   ResizeAndSmartShift(hist_trendStep_ST,newSize,forceShift);
   ResizeAndSmartShift(hist_detectedSlope_ST,newSize,forceShift);
   ResizeAndSmartShift(Master_hist_LT_detectedSlope_ST,newSize,forceShift);
   ResizeAndSmartShift(Master_hist_detectedSlope_ST,newSize,forceShift);
   ResizeAndSmartShift(hist_detectedAnchorBar_ST,newSize,forceShift);
   ResizeAndSmartShift(hist_LT_detectedAnchorBar_ST,newSize,forceShift);
   ResizeAndSmartShift(hist_ADXValue,newSize,forceShift);
   ResizeAndSmartShift(hist_VolumeValue,newSize,forceShift);
   ResizeAndSmartShift(hist_ST_Flip_Event,newSize,forceShift);
   ResizeAndSmartShift(hist_AnchorType_ST,newSize,forceShift);
   ResizeAndSmartShift(hist_BarsSinceFlip_ST,newSize,forceShift);
   ResizeAndSmartShift(hist_AnchorType_LT,newSize,forceShift);
   ResizeAndSmartShift(hist_BarsSinceFlip_LT,newSize,forceShift);
   ResizeAndSmartShift(hist_DecayState_ST,newSize,forceShift);
   ResizeAndSmartShift(hist_DecayState_LT,newSize,forceShift);
   ResizeAndSmartShift(state_ADX,newSize,forceShift);
   ResizeAndSmartShift(state_Momentum,newSize,forceShift);
   ResizeAndSmartShift(state_D2D_Upper,newSize,forceShift);
   ResizeAndSmartShift(state_D2D_Lower,newSize,forceShift);
   ResizeAndSmartShift(state_D2D_Dir,newSize,forceShift);
   ResizeAndSmartShift(state_OBV_Accum,newSize,forceShift);
   ResizeAndSmartShift(state_OBV_Fast,newSize,forceShift);
   ResizeAndSmartShift(state_OBV_Slow,newSize,forceShift);
   ResizeAndSmartShift(state_OBV_Macd,newSize,forceShift);
   ResizeAndSmartShift(state_OBV_Final,newSize,forceShift);
   ResizeAndSmartShift(state_OBV_Velocity,newSize,forceShift);
   ResizeAndSmartShift(state_TChan_Sum,newSize,forceShift);
   ResizeAndSmartShift(state_TChan_B5,newSize,forceShift);
   ResizeAndSmartShift(state_TChan_OC,newSize,forceShift);
   ResizeAndSmartShift(state_HarmVol_LLEMA,newSize,forceShift);
   ResizeAndSmartShift(state_HarmVol_KAMA,newSize,forceShift);
   ResizeAndSmartShift(state_HarmVol_EMA8,newSize,forceShift);
   ResizeAndSmartShift(state_HarmVol_EMA21,newSize,forceShift);
   ResizeAndSmartShift(state_HarmVol_EMAOsc,newSize,forceShift);
   ResizeAndSmartShift(state_Sqz_BB_Basis,newSize,forceShift);
   ResizeAndSmartShift(state_Sqz_BB_Dev,newSize,forceShift);
   ResizeAndSmartShift(state_Sqz_KC_RangeMa,newSize,forceShift);
   ResizeAndSmartShift(state_Sqz_Detrended,newSize,forceShift);
   ResizeAndSmartShift(state_Sqz_Val,newSize,forceShift);
   ResizeAndSmartShift(state_Sqz_State,newSize,forceShift);
   ResizeAndSmartShift(state_RangeOsc_MA,newSize,forceShift);
   ResizeAndSmartShift(state_RangeOsc_ATR,newSize,forceShift);
   ResizeAndSmartShift(state_RangeOsc_Val,newSize,forceShift);
   ResizeAndSmartShift(state_RangeOsc_State,newSize,forceShift);
   ResizeAndSmartShift(state_Slope_EMA_ST,newSize,forceShift);
   ResizeAndSmartShift(state_Slope_EMA_LT,newSize,forceShift);
   ResizeAndSmartShift(state_Slope_Accel_ST,newSize,forceShift);
   ResizeAndSmartShift(state_Slope_Accel_LT,newSize,forceShift);
   ResizeAndSmartShift(state_Micro_IBSP,newSize,forceShift);
   ResizeAndSmartShift(state_Micro_Lambda,newSize,forceShift);
   ResizeAndSmartShift(state_Micro_TickIntensity,newSize,forceShift);
   ResizeAndSmartShift(state_Micro_GarmanKlass,newSize,forceShift);
   ResizeAndSmartShift(state_Micro_Rejection,newSize,forceShift);
   ResizeAndSmartShift(state_Micro_OrderFlowDelta,newSize,forceShift);
   ResizeAndSmartShift(state_Micro_BarEntropy,newSize,forceShift);
   ResizeAndSmartShift(state_Micro_LogReturn,newSize,forceShift);
   ResizeAndSmartShift(state_Micro_PriceAccel,newSize,forceShift);
   ResizeAndSmartShift(state_Micro_RollProxy,newSize,forceShift);
   ResizeAndSmartShift(state_Micro_BarOverlap,newSize,forceShift);
   ResizeAndSmartShift(state_Micro_FailedBreak,newSize,forceShift);
   ResizeAndSmartShift(state_Micro_MomoTransfer,newSize,forceShift);
   ResizeAndSmartShift(state_Micro_MicroGap,newSize,forceShift);
   ResizeAndSmartShift(state_Micro_HLAsymmetry,newSize,forceShift);
   ResizeAndSmartShift(state_Micro_VolAccel,newSize,forceShift);
   ResizeAndSmartShift(state_Micro_RangeVelocity,newSize,forceShift);
   ResizeAndSmartShift(state_Micro_RangeAccel,newSize,forceShift);
   ResizeAndSmartShift(state_Micro_ThrustEff,newSize,forceShift);
   ResizeAndSmartShift(state_Micro_AutoCorr,newSize,forceShift);
   ResizeAndSmartShift(state_Micro_Entropy,newSize,forceShift);
   ResizeAndSmartShift(state_Micro_VPIN,newSize,forceShift);
   ResizeAndSmartShift(state_Micro_FractalDim,newSize,forceShift);
   ResizeAndSmartShift(state_Micro_VolOfVol,newSize,forceShift);
   ResizeAndSmartShift(state_Micro_Amihud,newSize,forceShift);
   ResizeAndSmartShift(state_Micro_WickImbalance,newSize,forceShift);
   ResizeAndSmartShift(state_Micro_CSSpread,newSize,forceShift);
   ResizeAndSmartShift(state_Micro_Hurst,newSize,forceShift);
   ResizeAndSmartShift(hist_Brain_Sensitivity,newSize,forceShift);
   ResizeAndSmartShift(hist_Poc_Price,newSize,forceShift);
   ResizeAndSmartShift(hist_OBV_Zero_Value,newSize,forceShift);
   ResizeAndSmartShift(hist_Efficiency_Ratio,newSize,forceShift);
   ResizeAndSmartShift(hist_UpperWick,newSize,forceShift);
   ResizeAndSmartShift(hist_VolumeRatio10,newSize,forceShift);
   ResizeAndSmartShift(hist_KAMA_Slope,newSize,forceShift);
   ResizeAndSmartShift(hist_KAMA_Dist_ATR,newSize,forceShift);
   ResizeAndSmartShift(atr_ema1,newSize,forceShift);
   ResizeAndSmartShift(atr_ema2,newSize,forceShift);
   ResizeAndSmartShift(atr_ema3,newSize,forceShift);
   ResizeAndSmartShift(atr_final_val,newSize,forceShift);
   ResizeAndSmartShift(hist_VWAP_Price,newSize,forceShift);
   ResizeAndSmartShift(hist_VWAP_Sigma,newSize,forceShift);
   ResizeAndSmartShift(hist_VAH_Price,newSize,forceShift);
   ResizeAndSmartShift(hist_VAL_Price,newSize,forceShift);
   ResizeAndSmartShift(hist_PrevDay_High,newSize,forceShift);
   ResizeAndSmartShift(hist_PrevDay_Low,newSize,forceShift);
   ResizeAndSmartShift(hist_PrevDay_Close,newSize,forceShift);
   ResizeAndSmartShift(hist_DailyOpen_Price,newSize,forceShift);
   ResizeAndSmartShift(hist_OR_High,newSize,forceShift);
   ResizeAndSmartShift(hist_OR_Low,newSize,forceShift);
   ResizeAndSmartShift(hist_Session_High,newSize,forceShift);
   ResizeAndSmartShift(hist_Session_Low,newSize,forceShift);
   ResizeAndSmartShift(hist_WeeklyOpen_Price,newSize,forceShift);
   ResizeAndSmartShift(hist_MultiDay_Slope,newSize,forceShift);
   ResizeAndSmartShift(hist_MultiDay_Position,newSize,forceShift);
   UpTrendBuffer[0]=EMPTY_VALUE;
   DownTrendBuffer[0]=EMPTY_VALUE;
   SuperTrend[0]=EMPTY_VALUE;
   OBVfriend_UpTrendBuffer[0]=EMPTY_VALUE;
   OBVfriend_DownTrendBuffer[0]=EMPTY_VALUE;
   OBVfriend_SuperTrend[0]=EMPTY_VALUE;
   hist_OBV_Zero_Value[0]=state_OBV_Final[0];
   hist_Brain_Sensitivity[0]=U_baseMult;
}
void ResizeAndSmartShift(double &arr[],int newSize,bool forceShift=false) {
   int oldSize=ArraySize(arr);
   ArrayResize(arr,newSize);
   ArraySetAsSeries(arr,true);
   if(forceShift&&oldSize>0) ArrayCopy(arr,arr,1,0,oldSize-1);
}
void ResizeAndSmartShift(int &arr[],int newSize,bool forceShift=false) {
   int oldSize=ArraySize(arr);
   ArrayResize(arr,newSize);
   ArraySetAsSeries(arr,true);
   if(forceShift&&oldSize>0) ArrayCopy(arr,arr,1,0,oldSize-1);
}
void ResizeAndSmartShift(datetime &arr[],int newSize,bool forceShift=false) {
   int oldSize=ArraySize(arr);
   ArrayResize(arr,newSize);
   ArraySetAsSeries(arr,true);
   if(forceShift&&oldSize>0) ArrayCopy(arr,arr,1,0,oldSize-1);
}
double CalculateMarketPosture(int i) {
   double base_factor=U_baseMult;
   int limit=ArraySize(state_OBV_Final);
   int velLimit=ArraySize(state_OBV_Velocity);
   if(i>=velLimit) return base_factor;
   if(!UseDynamicMatrix) return base_factor;
   if(i+Slope_Lookback>=limit) return base_factor;
   double currentDelta=state_OBV_Final[i];
   double currentVelocity=state_OBV_Velocity[i];
   if(i<ArraySize(hist_OBV_Zero_Value)) hist_OBV_Zero_Value[i]=currentDelta;
   int currentDir=(i+1<limit)?Direction[i+1]:0;
   bool isAgreeing=(currentDir==1&&currentDelta>0)||(currentDir==-1&&currentDelta<0);
   bool isDisagreeing=(currentDir==1&&currentDelta<0)||(currentDir==-1&&currentDelta>0);
   bool slopeAgreeing=(currentDir==1&&currentVelocity>Slope_Noise_Floor)||(currentDir==-1&&currentVelocity<-Slope_Noise_Floor);
   bool isSlopeNoise=(MathAbs(currentVelocity)<Slope_Noise_Floor);
   double priceForCheck=Close[i];
   double activePoC=(i<ArraySize(hist_Poc_Price))?hist_Poc_Price[i]:0.0;
   double distToPOC=MathAbs(priceForCheck-activePoC);
   bool nearFairValue=(activePoC>0)&&(distToPOC<(POC_Safety_Zone*Point*10));
   double avgVol=0;
   int volCount=0;
   for(int v=i+1; v<=i+10; v++) {
      if(v<limit) {
         if(hist_VolumeValue[v]<=0.0&&Volume[v]>0) hist_VolumeValue[v]=(double)Volume[v];
         avgVol+=hist_VolumeValue[v];
         volCount++;
      }
   }
   if(volCount>0) avgVol/=volCount; else avgVol=1.0;
   if(i>0&&hist_VolumeValue[i]<=0.0&&Volume[i]>0) hist_VolumeValue[i]=(double)Volume[i];
   double currentVol=(i==0)?(double)Volume[0]:hist_VolumeValue[i];
   bool highVolume=(currentVol>avgVol*1.5);
   bool lowVolume=(currentVol<avgVol*0.8);
   double netChange=0.0;
   double totalPath=0.0;
   int eff_period=10;
   if(i+eff_period<limit) {
      netChange=MathAbs(Close[i]-Close[i+eff_period]);
      for(int k=i; k<i+eff_period; k++) totalPath+=MathAbs(Close[k]-Close[k+1]);
   }
   double efficiency=(totalPath>0)?(netChange/totalPath):0.0;
   if(i<ArraySize(hist_Efficiency_Ratio)) hist_Efficiency_Ratio[i]=efficiency;
   bool isEfficientDrift=(efficiency>0.65&&lowVolume&&!highVolume);
   double dynamicFactor=base_factor;
   string posture="Normal";
   string obv_st="Neutral";
   string vol_st="Normal";
   string poc_st="Clear";
   if(isAgreeing) obv_st="Agree";
   if(isDisagreeing) {
      if(slopeAgreeing) obv_st="Recovering";
      else if(isSlopeNoise) obv_st="Chop/Ignored";
      else obv_st="Conflict";
   }
   if(highVolume) vol_st="High";
   if(lowVolume) vol_st="Low";
   if(nearFairValue) poc_st="In Zone";
   if(isEfficientDrift&&isAgreeing) {
      dynamicFactor=base_factor*2.8;
      posture="Drifting (144 Sim)";
   } else if(highVolume) {
      dynamicFactor=base_factor;
      posture="Thrust (NY Open)";
   } else if(nearFairValue&&lowVolume) {
      dynamicFactor*=2.0;
      posture="Defensive";
   } else if(isDisagreeing) {
      if(slopeAgreeing) {
         dynamicFactor*=Slope_Recovery_Mult;
         posture="Recovering";
      } else if(isSlopeNoise) {
         dynamicFactor*=Slope_Recovery_Mult;
         posture="Holding (Chop)";
      } else {
         if(highVolume||MathAbs(currentDelta)>30) {
            dynamicFactor*=DiscordanceExtreme;
            posture="Hyper-Sensitive";
         } else {
            dynamicFactor*=DiscordanceMult;
            posture="Tension";
         }
      }
   } else if(isAgreeing) {
      dynamicFactor*=ConcordanceMult;
      posture="Relaxed";
   }
   if(i==0) {
      dy_Posture=posture;
      dy_OBV_State=obv_st;
      dy_Vol_State=vol_st;
      dy_POC_State=poc_st;
      latest_d2d_dynamic_factor=MathMin(MathMax(dynamicFactor,U_multMin),U_multMax);
   }
   double finalSens=MathMin(MathMax(dynamicFactor,U_multMin),U_multMax);
   if(i<ArraySize(hist_Brain_Sensitivity)) hist_Brain_Sensitivity[i]=finalSens;
   return finalSens;
}
double CalculateOBVfriendPosture(int i) {
   double base_factor=OBV_baseMult;
   int limit=ArraySize(state_OBV_Final);
   int velLimit=ArraySize(state_OBV_Velocity);
   if(i>=velLimit) return base_factor;
   if(!UseDynamicMatrix) return base_factor;
   if(i+Slope_Lookback>=limit) return base_factor;
   double currentDelta=state_OBV_Final[i];
   double currentVelocity=state_OBV_Velocity[i];
   int currentDir=(i+1<limit)?OBVfriend_Direction[i+1]:0;
   bool isAgreeing=(currentDir==1&&currentDelta>0)||(currentDir==-1&&currentDelta<0);
   bool isDisagreeing=(currentDir==1&&currentDelta<0)||(currentDir==-1&&currentDelta>0);
   bool slopeAgreeing=(currentDir==1&&currentVelocity>Slope_Noise_Floor)||(currentDir==-1&&currentVelocity<-Slope_Noise_Floor);
   bool isSlopeNoise=(MathAbs(currentVelocity)<Slope_Noise_Floor);
   double priceForCheck=Close[i];
   double activePoC=(i<ArraySize(hist_Poc_Price))?hist_Poc_Price[i]:0.0;
   double distToPOC=MathAbs(priceForCheck-activePoC);
   bool nearFairValue=(activePoC>0)&&(distToPOC<(POC_Safety_Zone*Point*10));
   double avgVol=0;
   int volCount=0;
   for(int v=i+1; v<=i+10; v++) {
      if(v<limit) {
         if(hist_VolumeValue[v]<=0.0&&Volume[v]>0) hist_VolumeValue[v]=(double)Volume[v];
         avgVol+=hist_VolumeValue[v];
         volCount++;
      }
   }
   if(volCount>0) avgVol/=volCount; else avgVol=1.0;
   if(i>0&&hist_VolumeValue[i]<=0.0&&Volume[i]>0) hist_VolumeValue[i]=(double)Volume[i];
   double currentVol=(i==0)?(double)Volume[0]:hist_VolumeValue[i];
   bool highVolume=(currentVol>avgVol*1.5);
   bool lowVolume=(currentVol<avgVol*0.8);
   double netChange=0.0;
   double totalPath=0.0;
   int eff_period=10;
   if(i+eff_period<limit) {
      netChange=MathAbs(Close[i]-Close[i+eff_period]);
      for(int k=i; k<i+eff_period; k++) totalPath+=MathAbs(Close[k]-Close[k+1]);
   }
   double efficiency=(totalPath>0)?(netChange/totalPath):0.0;
   bool isEfficientDrift=(efficiency>0.65&&lowVolume&&!highVolume);
   double dynamicFactor=base_factor;
   if(isEfficientDrift&&isAgreeing) {
      dynamicFactor=base_factor*2.8;
   } else if(highVolume) {
      dynamicFactor=base_factor;
   } else if(nearFairValue&&lowVolume) {
      dynamicFactor*=2.0;
   } else if(isDisagreeing) {
      if(slopeAgreeing) {
         dynamicFactor*=Slope_Recovery_Mult;
      } else if(isSlopeNoise) {
         dynamicFactor*=Slope_Recovery_Mult;
      } else {
         if(highVolume||MathAbs(currentDelta)>30) {
            dynamicFactor*=DiscordanceExtreme;
         } else {
            dynamicFactor*=DiscordanceMult;
         }
      }
   } else if(isAgreeing) {
      dynamicFactor*=ConcordanceMult;
   }
   double finalSens=MathMin(MathMax(dynamicFactor,OBV_multMin),OBV_multMax);
   return finalSens;
}
void Calc_ADX_OnBar(int i) {
   int limit=ArraySize(ADXBuffer);
   int smLimit=ArraySize(ADX_SmoothedPlusDM);
   if(i>=limit-2) { ADXBuffer[i]=0; return; }
   double hi=High[i]; double lo=Low[i];
   double prev_hi=High[i+1]; double prev_lo=Low[i+1]; double prev_cl=Close[i+1];
   double plusDM=(hi-prev_hi>prev_lo-lo&&hi-prev_hi>0)?hi-prev_hi:0;
   double minusDM=(prev_lo-lo>hi-prev_hi&&prev_lo-lo>0)?prev_lo-lo:0;
   double tr=MathMax(MathMax(hi-lo,MathAbs(hi-prev_cl)),MathAbs(lo-prev_cl));
   if(tr==0) tr=Point;
   double alpha=1.0/(double)ADX_Period;
   double prev_sPDM=(i+1<smLimit)?ADX_SmoothedPlusDM[i+1]:plusDM;
   double prev_sMDM=(i+1<smLimit)?ADX_SmoothedMinusDM[i+1]:minusDM;
   double prev_sTR=(i+1<smLimit)?ADX_SmoothedTR[i+1]:tr;
   ADX_SmoothedPlusDM[i]=prev_sPDM*(1.0-alpha)+plusDM*alpha;
   ADX_SmoothedMinusDM[i]=prev_sMDM*(1.0-alpha)+minusDM*alpha;
   ADX_SmoothedTR[i]=prev_sTR*(1.0-alpha)+tr*alpha;
   double sTR=ADX_SmoothedTR[i];
   double diPlus=(sTR>0)?(ADX_SmoothedPlusDM[i]/sTR)*100:0;
   double diMinus=(sTR>0)?(ADX_SmoothedMinusDM[i]/sTR)*100:0;
   double sum=diPlus+diMinus;
   double dx=(sum==0)?0:(MathAbs(diPlus-diMinus)/sum)*100;
   double prev_adx=(i+1<limit)?ADXBuffer[i+1]:0.0;
   ADXBuffer[i]=prev_adx*(1.0-alpha)+dx*alpha;
   hist_ADXValue[i]=ADXBuffer[i];
}
void Calc_Momentum_OnBar(int i) {
   int momPeriod=14;
   int limit=ArraySize(MomentumBuffer);
   if(i+momPeriod>=limit) { MomentumBuffer[i]=100.0; return; }
   if(Close[i+momPeriod]!=0) MomentumBuffer[i]=(Close[i]/Close[i+momPeriod])*100.0;
   else MomentumBuffer[i]=100.0;
   latest_momentum_value=MomentumBuffer[i];
}
void Calc_D2D_ST_OnBar(int i) {
   int limit=ArraySize(U_BasisBuffer);
   if(i>=limit-2) {
      U_BasisBuffer[i]=Close[i]; U_AtrBuffer[i]=High[i]-Low[i]; U_AtrMaBuffer[i]=U_AtrBuffer[i];
      U_UpperBandBuffer[i]=Close[i]; U_LowerBandBuffer[i]=Close[i];
      Direction[i]=0; U_UpCntBuffer[i]=0; U_DnCntBuffer[i]=0;
      U_PersistBuffer[i]=0.0; U_DirStepBuffer[i]=0.0;
      return;
   }
   double dynMult=CalculateMarketPosture(i);
   double alpha_basis=2.0/(U_emaLen+1.0);
   double alpha_atr=1.0/U_atrLen;
   double alpha_persist=2.0/(U_persLen+1.0);
   double prev_Basis=U_BasisBuffer[i+1];
   double prev_Atr=U_AtrBuffer[i+1];
   double prev_AtrMa=U_AtrMaBuffer[i+1];
   double prev_Persist=U_PersistBuffer[i+1];
   U_BasisBuffer[i]=(Close[i]*alpha_basis)+(prev_Basis*(1.0-alpha_basis));
   double hl=High[i]-Low[i];
   double hc=MathAbs(High[i]-Close[i+1]);
   double lc=MathAbs(Low[i]-Close[i+1]);
   double tr=MathMax(hl,MathMax(hc,lc));
   if(U_UseTrueRangeCapping&&tr_cap>0.0) tr=MathMin(tr,tr_cap);
   U_AtrBuffer[i]=(tr*alpha_atr)+(prev_Atr*(1.0-alpha_atr));
   U_AtrMaBuffer[i]=(U_AtrBuffer[i]*alpha_basis)+(prev_AtrMa*(1.0-alpha_basis));
   double atr_ma=U_AtrMaBuffer[i];
   double expRatio=(atr_ma==0.0)?1.0:(U_AtrBuffer[i]/atr_ma);
   double expAdj=MathPow(expRatio,U_sensExp);
   double slope=U_BasisBuffer[i]-prev_Basis;
   U_DirStepBuffer[i]=(slope>=0)?1.0:-1.0;
   U_PersistBuffer[i]=(U_DirStepBuffer[i]*alpha_persist)+(prev_Persist*(1.0-alpha_persist));
   double persistAdj=1.0+U_persGain*MathAbs(U_PersistBuffer[i]);
   double finalMultRaw=dynMult*expAdj*persistAdj;
   U_UpperBandBuffer[i]=U_BasisBuffer[i]+finalMultRaw*U_AtrBuffer[i];
   U_LowerBandBuffer[i]=U_BasisBuffer[i]-finalMultRaw*U_AtrBuffer[i];
   int state_prev=Direction[i+1];
   double trailUp_prev=(state_prev==1)?UpTrendBuffer[i+1]:U_LowerBandBuffer[i+1];
   double trailDn_prev=(state_prev==-1)?DownTrendBuffer[i+1]:U_UpperBandBuffer[i+1];
   double trailUp_new, trailDn_new;
   if(state_prev==1) {
      trailUp_new=MathMax(U_LowerBandBuffer[i],trailUp_prev); trailDn_new=U_UpperBandBuffer[i];
   } else if(state_prev==-1) {
      trailDn_new=MathMin(U_UpperBandBuffer[i],trailDn_prev); trailUp_new=U_LowerBandBuffer[i];
   } else {
      trailUp_new=U_LowerBandBuffer[i]; trailDn_new=U_UpperBandBuffer[i];
   }
   int upCnt_prev=U_UpCntBuffer[i+1];
   int dnCnt_prev=U_DnCntBuffer[i+1];
   bool aboveDn=(Close[i]>trailDn_new);
   bool belowUp=(Close[i]<trailUp_new);
   U_UpCntBuffer[i]=aboveDn?(upCnt_prev+1):0;
   U_DnCntBuffer[i]=belowUp?(dnCnt_prev+1):0;
   int state_new=state_prev;
   if(state_prev==0) {
      if(U_UpCntBuffer[i]>=U_confirmN) state_new=1;
      if(U_DnCntBuffer[i]>=U_confirmN) state_new=-1;
   } else if(state_prev==1) {
      if(U_DnCntBuffer[i]>=U_confirmN) state_new=-1;
   } else if(state_prev==-1) {
      if(U_UpCntBuffer[i]>=U_confirmN) state_new=1;
   }
   Direction[i]=state_new;
   if(state_new==1) {
      UpTrendBuffer[i]=trailUp_new; DownTrendBuffer[i]=EMPTY_VALUE;
      SuperTrend[i]=trailUp_new; LockBuffer[i]=(state_prev==-1)?1:0;
   } else if(state_new==-1) {
      DownTrendBuffer[i]=trailDn_new; UpTrendBuffer[i]=EMPTY_VALUE;
      SuperTrend[i]=trailDn_new; LockBuffer[i]=(state_prev==1)?-1:0;
   } else {
      UpTrendBuffer[i]=EMPTY_VALUE; DownTrendBuffer[i]=EMPTY_VALUE;
      SuperTrend[i]=EMPTY_VALUE; LockBuffer[i]=0;
   }
}
void Calc_OBVfriend_ST_OnBar(int i) {
   int limit=ArraySize(OBV_BasisBuffer);
   if(i>=limit-2) {
      OBV_BasisBuffer[i]=Close[i]; OBV_AtrBuffer[i]=High[i]-Low[i]; OBV_AtrMaBuffer[i]=OBV_AtrBuffer[i];
      OBV_UpperBandBuffer[i]=Close[i]; OBV_LowerBandBuffer[i]=Close[i];
      OBVfriend_Direction[i]=0;
      OBV_PersistBuffer[i]=0.0; OBV_DirStepBuffer[i]=0.0; OBV_DirStepCountBuffer[i]=0.0;
      return;
   }
   double dynMult=CalculateOBVfriendPosture(i);
   double alpha_basis=2.0/(OBV_emaLen+1.0);
   double alpha_atr=1.0/OBV_atrLen;
   double alpha_persist=2.0/(OBV_persLen+1.0);
   double prev_Basis=OBV_BasisBuffer[i+1];
   double prev_Atr=OBV_AtrBuffer[i+1];
   double prev_AtrMa=OBV_AtrMaBuffer[i+1];
   double prev_Persist=OBV_PersistBuffer[i+1];
   OBV_BasisBuffer[i]=(Close[i]*alpha_basis)+(prev_Basis*(1.0-alpha_basis));
   double hl=High[i]-Low[i];
   double hc=MathAbs(High[i]-Close[i+1]);
   double lc=MathAbs(Low[i]-Close[i+1]);
   double tr=MathMax(hl,MathMax(hc,lc));
   if(OBV_UseTrueRangeCapping&&tr_cap>0.0) tr=MathMin(tr,tr_cap);
   OBV_AtrBuffer[i]=(tr*alpha_atr)+(prev_Atr*(1.0-alpha_atr));
   OBV_AtrMaBuffer[i]=(OBV_AtrBuffer[i]*alpha_basis)+(prev_AtrMa*(1.0-alpha_basis));
   double atr_ma=OBV_AtrMaBuffer[i];
   double expRatio=(atr_ma==0.0)?1.0:(OBV_AtrBuffer[i]/atr_ma);
   double expAdj=MathPow(expRatio,OBV_sensExp);
   double slope=OBV_BasisBuffer[i]-prev_Basis;
   OBV_DirStepBuffer[i]=(slope>=0)?1.0:-1.0;
   int cntLimit=ArraySize(OBV_DirStepCountBuffer);
   double prevDir=(i+1<cntLimit)?OBV_DirStepBuffer[i+1]:0.0;
   double prevCount=(i+1<cntLimit)?OBV_DirStepCountBuffer[i+1]:0.0;
   if(OBV_DirStepBuffer[i]==prevDir&&prevDir!=0.0) OBV_DirStepCountBuffer[i]=prevCount+1.0;
   else OBV_DirStepCountBuffer[i]=1.0;
   OBV_PersistBuffer[i]=(OBV_DirStepBuffer[i]*alpha_persist)+(prev_Persist*(1.0-alpha_persist));
   double persistAdj=1.0+OBV_persGain*MathAbs(OBV_PersistBuffer[i]);
   double finalMultRaw=dynMult*expAdj*persistAdj;
   OBV_UpperBandBuffer[i]=OBV_BasisBuffer[i]+finalMultRaw*OBV_AtrBuffer[i];
   OBV_LowerBandBuffer[i]=OBV_BasisBuffer[i]-finalMultRaw*OBV_AtrBuffer[i];
   int state_prev=OBVfriend_Direction[i+1];
   double trailUp_prev=(state_prev==1)?OBVfriend_UpTrendBuffer[i+1]:OBV_LowerBandBuffer[i+1];
   double trailDn_prev=(state_prev==-1)?OBVfriend_DownTrendBuffer[i+1]:OBV_UpperBandBuffer[i+1];
   double trailUp_new, trailDn_new;
   if(state_prev==1) {
      trailUp_new=MathMax(OBV_LowerBandBuffer[i],trailUp_prev); trailDn_new=OBV_UpperBandBuffer[i];
   } else if(state_prev==-1) {
      trailDn_new=MathMin(OBV_UpperBandBuffer[i],trailDn_prev); trailUp_new=OBV_LowerBandBuffer[i];
   } else {
      trailUp_new=OBV_LowerBandBuffer[i]; trailDn_new=OBV_UpperBandBuffer[i];
   }
   int state_new=state_prev;
   if(state_TChan_OC[i]==1.0) state_new=1;
   else if(state_TChan_OC[i]==-1.0) state_new=-1;
   else if(state_prev==0) state_new=1;
   OBVfriend_Direction[i]=state_new;
   if(state_new==1) {
      OBVfriend_UpTrendBuffer[i]=trailUp_new; OBVfriend_DownTrendBuffer[i]=EMPTY_VALUE;
      OBVfriend_SuperTrend[i]=trailUp_new;
   } else if(state_new==-1) {
      OBVfriend_DownTrendBuffer[i]=trailDn_new; OBVfriend_UpTrendBuffer[i]=EMPTY_VALUE;
      OBVfriend_SuperTrend[i]=trailDn_new;
   } else {
      OBVfriend_UpTrendBuffer[i]=EMPTY_VALUE; OBVfriend_DownTrendBuffer[i]=EMPTY_VALUE;
      OBVfriend_SuperTrend[i]=EMPTY_VALUE;
   }
}
double GetRollingMaxVolume(int lookback, int shift, int &outPeakBarIndex) {
   double peakVol = -1.0;
   outPeakBarIndex = -1;
   int limit = ArraySize(hist_VolumeValue);
   for(int i = shift; i < shift + lookback; i++) {
      if(i >= limit) break;
      if(hist_VolumeValue[i] > peakVol) {
         peakVol = hist_VolumeValue[i];
         outPeakBarIndex = i;
      }
   }
   return peakVol;
}
void Calc_Sqz_Momentum_OnBar(int i) {
   int bbLen=Sqz_BB_Length;
   int kcLen=Sqz_KC_Length;
   int limitBasis=ArraySize(state_Sqz_BB_Basis);
   int limitDev=ArraySize(state_Sqz_BB_Dev);
   int limitRangeMa=ArraySize(state_Sqz_KC_RangeMa);
   int limitDetrend=ArraySize(state_Sqz_Detrended);
   int limitVal=ArraySize(state_Sqz_Val);
   int limitState=ArraySize(state_Sqz_State);
   if(i<0||i>=limitBasis||i>=limitDev||i>=limitRangeMa) return;
   if(i>=limitDetrend||i>=limitVal||i>=limitState) return;
   int maxLen=bbLen;
   if(kcLen>maxLen) maxLen=kcLen;
   if(i+maxLen>=Bars) {
      state_Sqz_BB_Basis[i]=Close[i];
      state_Sqz_BB_Dev[i]=0.0;
      state_Sqz_KC_RangeMa[i]=0.0;
      state_Sqz_Detrended[i]=0.0;
      state_Sqz_Val[i]=0.0;
      state_Sqz_State[i]=0;
      return;
   }
   double sumClose=0.0;
   for(int j=i; j<i+bbLen; j++) sumClose+=Close[j];
   double basis=sumClose/(double)bbLen;
   state_Sqz_BB_Basis[i]=basis;
   double sumSqDev=0.0;
   for(int j=i; j<i+bbLen; j++) {
      double diff=Close[j]-basis;
      sumSqDev+=diff*diff;
   }
   double stdDev=MathSqrt(sumSqDev/(double)bbLen);
   state_Sqz_BB_Dev[i]=stdDev;
   double upperBB=basis+(Sqz_BB_Mult*stdDev);
   double lowerBB=basis-(Sqz_BB_Mult*stdDev);
   double sumCloseKC=0.0;
   for(int j=i; j<i+kcLen; j++) sumCloseKC+=Close[j];
   double ma=sumCloseKC/(double)kcLen;
   double sumRange=0.0;
   for(int j=i; j<i+kcLen; j++) {
      double rangeVal=0.0;
      if(Sqz_UseTrueRange) {
         double hl=High[j]-Low[j];
         double hc=(j+1<Bars)?MathAbs(High[j]-Close[j+1]):hl;
         double lc=(j+1<Bars)?MathAbs(Low[j]-Close[j+1]):hl;
         rangeVal=MathMax(hl,MathMax(hc,lc));
      } else {
         rangeVal=High[j]-Low[j];
      }
      sumRange+=rangeVal;
   }
   double rangeMa=sumRange/(double)kcLen;
   state_Sqz_KC_RangeMa[i]=rangeMa;
   double upperKC=ma+(rangeMa*Sqz_KC_Mult);
   double lowerKC=ma-(rangeMa*Sqz_KC_Mult);
   bool sqzOn=(lowerBB>lowerKC)&&(upperBB<upperKC);
   bool sqzOff=(lowerBB<lowerKC)&&(upperBB>upperKC);
   if(sqzOn) state_Sqz_State[i]=1;
   else if(sqzOff) state_Sqz_State[i]=-1;
   else state_Sqz_State[i]=0;
   double highestHigh=-DBL_MAX;
   double lowestLow=DBL_MAX;
   for(int j=i; j<i+kcLen; j++) {
      if(High[j]>highestHigh) highestHigh=High[j];
      if(Low[j]<lowestLow) lowestLow=Low[j];
   }
   double midline=((highestHigh+lowestLow)/2.0+ma)/2.0;
   state_Sqz_Detrended[i]=Close[i]-midline;
   int n=kcLen;
   if(i+n-1>=limitDetrend) {
      state_Sqz_Val[i]=state_Sqz_Detrended[i];
      return;
   }
   double sumX=((double)n*((double)n-1.0))/2.0;
   double sumX2=((double)n*((double)n-1.0)*(2.0*(double)n-1.0))/6.0;
   double sumY=0.0;
   double sumXY=0.0;
   for(int j=0; j<n; j++) {
      double x=(double)(n-1-j);
      double y=state_Sqz_Detrended[i+j];
      sumY+=y;
      sumXY+=x*y;
   }
   double denom=(double)n*sumX2-sumX*sumX;
   if(MathAbs(denom)<1e-20) {
      state_Sqz_Val[i]=state_Sqz_Detrended[i];
      return;
   }
   double slope_lr=((double)n*sumXY-sumX*sumY)/denom;
   double intercept_lr=(sumY-slope_lr*sumX)/(double)n;
   state_Sqz_Val[i]=intercept_lr+slope_lr*((double)n-1.0);
}
void Calc_RangeOsc_OnBar(int i) {
   int roLen=144;
   double roMult=2.0;
   int roAtrPeriod=200;
   int limitMA=ArraySize(state_RangeOsc_MA);
   int limitATR=ArraySize(state_RangeOsc_ATR);
   int limitVal=ArraySize(state_RangeOsc_Val);
   int limitState=ArraySize(state_RangeOsc_State);
   if(i<0||i>=limitMA||i>=limitATR||i>=limitVal||i>=limitState) return;
   if(i+roLen>=Bars) {
      state_RangeOsc_MA[i]=Close[i];
      state_RangeOsc_ATR[i]=(i+1<limitATR)?state_RangeOsc_ATR[i+1]:(High[i]-Low[i]);
      state_RangeOsc_Val[i]=0.0;
      state_RangeOsc_State[i]=0;
      return;
   }
   double sumWeightedClose=0.0;
   double sumWeights=0.0;
   for(int j=i; j<i+roLen; j++) {
      double prevClose=Close[j+1];
      if(prevClose==0.0) continue;
      double delta=MathAbs(Close[j]-prevClose);
      double w=delta/prevClose;
      sumWeightedClose+=Close[j]*w;
      sumWeights+=w;
   }
   double ma=(sumWeights!=0.0)?(sumWeightedClose/sumWeights):Close[i];
   state_RangeOsc_MA[i]=ma;
   double hl=High[i]-Low[i];
   double hc=(i+1<Bars)?MathAbs(High[i]-Close[i+1]):hl;
   double lc=(i+1<Bars)?MathAbs(Low[i]-Close[i+1]):hl;
   double tr=MathMax(hl,MathMax(hc,lc));
   double prevATR=(i+1<limitATR)?state_RangeOsc_ATR[i+1]:tr;
   state_RangeOsc_ATR[i]=prevATR+(tr-prevATR)/(double)roAtrPeriod;
   double rangeATR=state_RangeOsc_ATR[i]*roMult;
   double osc=0.0;
   if(rangeATR>0.0) osc=100.0*(Close[i]-ma)/rangeATR;
   state_RangeOsc_Val[i]=osc;
   if(Close[i]>ma+rangeATR) state_RangeOsc_State[i]=1;
   else if(Close[i]<ma-rangeATR) state_RangeOsc_State[i]=-1;
   else if(osc>0.0) state_RangeOsc_State[i]=2;
   else if(osc<0.0) state_RangeOsc_State[i]=-2;
   else state_RangeOsc_State[i]=0;
}
//+------------------------------------------------------------------+
//| SECTION 6.1 - ADAPTIVE TREND CALCULATION (STATEFUL)              |
//+------------------------------------------------------------------+
void Master_SetST_TrendMetrics_OnBar(int i,double pearsonR,double slope) {
   int score=7; string strengthStr="0";
   if(pearsonR<0.90) { score=7; strengthStr="0"; }
   else if(slope>=0.0) {
      if(pearsonR<0.92) { score=8; strengthStr="+0"; }
      else if(pearsonR<0.94) { score=9; strengthStr="+1"; }
      else if(pearsonR<0.96) { score=10; strengthStr="+2"; }
      else if(pearsonR<0.98) { score=11; strengthStr="+3"; }
      else if(pearsonR<0.99) { score=12; strengthStr="+4"; }
      else { score=13; strengthStr="+5"; }
   } else {
      if(pearsonR<0.92) { score=6; strengthStr="-0"; }
      else if(pearsonR<0.94) { score=5; strengthStr="-1"; }
      else if(pearsonR<0.96) { score=4; strengthStr="-2"; }
      else if(pearsonR<0.98) { score=3; strengthStr="-3"; }
      else if(pearsonR<0.99) { score=2; strengthStr="-4"; }
      else { score=1; strengthStr="-5"; }
   }
   hist_trendStep_ST[i]=score;
   if(i<=1) detectedTrendStrength_ST=strengthStr;
}
void Master_SetLT_TrendMetrics_OnBar(int i,double pearsonR,double slope) {
   int score=7; string strengthStr="0";
   if(pearsonR<0.90) { score=7; strengthStr="0"; }
   else if(slope>=0.0) {
      if(pearsonR<0.92) { score=8; strengthStr="+0"; }
      else if(pearsonR<0.94) { score=9; strengthStr="+1"; }
      else if(pearsonR<0.96) { score=10; strengthStr="+2"; }
      else if(pearsonR<0.98) { score=11; strengthStr="+3"; }
      else if(pearsonR<0.99) { score=12; strengthStr="+4"; }
      else { score=13; strengthStr="+5"; }
   } else {
      if(pearsonR<0.92) { score=6; strengthStr="-0"; }
      else if(pearsonR<0.94) { score=5; strengthStr="-1"; }
      else if(pearsonR<0.96) { score=4; strengthStr="-2"; }
      else if(pearsonR<0.98) { score=3; strengthStr="-3"; }
      else if(pearsonR<0.99) { score=2; strengthStr="-4"; }
      else { score=1; strengthStr="-5"; }
   }
   hist_LT_trendStep_ST[i]=score;
   if(i<=1) LT_detectedTrendStrength_ST=strengthStr;
}
bool Master_calcDev_OnBar(int startBar,int length,double &result[]) {
   ArrayResize(result,4);
   if(startBar+length>=Bars) return false;
   double sumX=0.0,sumY=0.0,sumXX=0.0,sumXY=0.0,sumYY=0.0;
   double sumW=0.0,sumWX=0.0,sumWY=0.0,sumWXX=0.0,sumWXY=0.0;
   for(int k=0; k<length; k++) {
      double val=Close[startBar+k];
      if(val<=0.0) val=(double)Point;
      double lSrc=MathLog(val);
      double x=(double)k+1.0;
      double weight=(double)(length-k)/(double)length;
      sumX+=x; sumY+=lSrc; sumXX+=x*x; sumXY+=x*lSrc; sumYY+=lSrc*lSrc;
      sumW+=weight; sumWX+=weight*x; sumWY+=weight*lSrc; sumWXX+=weight*x*x; sumWXY+=weight*x*lSrc;
   }
   double denomW=(sumW*sumWXX-sumWX*sumWX);
   if(denomW==0.0) return false;
   double slope=(sumW*sumWXY-sumWX*sumWY)/denomW;
   double intercept=(sumWY/sumW)-slope*(sumWX/sumW)+slope;
   double denomOLS=((double)length*sumXX-sumX*sumX);
   double pearson_denom=MathSqrt(denomOLS*((double)length*sumYY-sumY*sumY));
   double pearsonR=(pearson_denom!=0.0)?(((double)length*sumXY-sumX*sumY)/pearson_denom):0.0;
   double sumWRes=0.0;
   for(int k=0; k<length; k++) {
      double x=(double)k+1.0;
      double weight=(double)(length-k)/(double)length;
      double predicted=intercept+slope*(x-1.0);
      double actual=MathLog(Close[startBar+k]);
      sumWRes+=weight*MathPow(actual-predicted,2.0);
   }
   double stdDev=(sumW>0.0)?MathSqrt(sumWRes/sumW):0.0;
   result[0]=MathAbs(pearsonR); result[1]=-slope; result[2]=intercept; result[3]=stdDev;
   return true;
}
void GetBestFit_OnBar(int i,int minLen,int maxLen,double &outSlope,double &outIntercept,double &outR,double &outStdDev,int &outBestLength) {
   int step=10;
   if(step<1) step=1;
   double bestScore=-1.0,bestR=-1.0,bestSlope=0.0,bestIntercept=0.0,bestStd=0.0;
   int bestLen=minLen;
   for(int len=minLen; len<=maxLen; len+=step) {
      if(i+len>=Bars) break;
      double res[];
      if(Master_calcDev_OnBar(i,len,res)) {
         double penalizedR=res[0]-(double)len*0.0001;
         if(penalizedR>bestScore) {
            bestScore=penalizedR; bestR=res[0]; bestSlope=res[1]; bestIntercept=res[2]; bestStd=res[3]; bestLen=len;
         }
      }
   }
   outSlope=bestSlope; outIntercept=bestIntercept; outR=bestR; outStdDev=bestStd; outBestLength=bestLen;
}
void Calc_AdaptiveTrend_OnBar(int i) {
   int limit=ArraySize(hist_trendStep_ST);
   if(i>=limit-20) return;
   int prevBarsSince_ST=(i+1<limit)?hist_BarsSinceFlip_ST[i+1]:0;
   int st_min=20;
   int st_max=200;
   if(prevBarsSince_ST>0) {
      st_max=(int)MathMin((double)st_max,(double)(prevBarsSince_ST+1));
      if(st_max<st_min) st_max=st_min;
   }
   double st_Slope,st_Int,st_R,st_Std; int st_Len;
   GetBestFit_OnBar(i,st_min,st_max,st_Slope,st_Int,st_R,st_Std,st_Len);
   Master_SetST_TrendMetrics_OnBar(i,st_R,st_Slope);
   int prevBarsSince_LT=(i+1<limit)?hist_BarsSinceFlip_LT[i+1]:0;
   int lt_min=200;
   int lt_max=1200;
   if(prevBarsSince_LT>0) {
      lt_max=(int)MathMin((double)lt_max,(double)(prevBarsSince_LT+1));
      if(lt_max<lt_min) lt_max=lt_min;
   }
   double lt_Slope,lt_Int,lt_R,lt_Std; int lt_Len;
   GetBestFit_OnBar(i,lt_min,lt_max,lt_Slope,lt_Int,lt_R,lt_Std,lt_Len);
   Master_SetLT_TrendMetrics_OnBar(i,lt_R,lt_Slope);
   double slopeAlpha=2.0/6.0;
   int emaLimit_ST=ArraySize(state_Slope_EMA_ST);
   double prevSlopeEMA_ST=(i+1<emaLimit_ST)?state_Slope_EMA_ST[i+1]:st_Slope;
   state_Slope_EMA_ST[i]=st_Slope*slopeAlpha+prevSlopeEMA_ST*(1.0-slopeAlpha);
   state_Slope_Accel_ST[i]=state_Slope_EMA_ST[i]-prevSlopeEMA_ST;
   double emaVal_ST=state_Slope_EMA_ST[i];
   double accelVal_ST=state_Slope_Accel_ST[i];
   int atrLimit_AT=ArraySize(assignedATR);
   double barATR=(i<atrLimit_AT)?assignedATR[i]:0.0;
   double barClose=Close[i];
   double dzThreshold=(barClose>0.0)?((barATR/barClose)*Decay_Accel_ATR_Deadzone):0.0;
   if(MathAbs(accelVal_ST)<=dzThreshold) {
      hist_DecayState_ST[i]=0;
   } else {
      if(emaVal_ST>0.0&&accelVal_ST>0.0) hist_DecayState_ST[i]=1;
      else if(emaVal_ST>0.0&&accelVal_ST<0.0) hist_DecayState_ST[i]=2;
      else if(emaVal_ST<0.0&&accelVal_ST<0.0) hist_DecayState_ST[i]=-1;
      else if(emaVal_ST<0.0&&accelVal_ST>0.0) hist_DecayState_ST[i]=-2;
      else hist_DecayState_ST[i]=0;
   }
   int emaLimit_LT=ArraySize(state_Slope_EMA_LT);
   double prevSlopeEMA_LT=(i+1<emaLimit_LT)?state_Slope_EMA_LT[i+1]:lt_Slope;
   state_Slope_EMA_LT[i]=lt_Slope*slopeAlpha+prevSlopeEMA_LT*(1.0-slopeAlpha);
   state_Slope_Accel_LT[i]=state_Slope_EMA_LT[i]-prevSlopeEMA_LT;
   double emaVal_LT=state_Slope_EMA_LT[i];
   double accelVal_LT=state_Slope_Accel_LT[i];
   if(MathAbs(accelVal_LT)<=dzThreshold) {
      hist_DecayState_LT[i]=0;
   } else {
      if(emaVal_LT>0.0&&accelVal_LT>0.0) hist_DecayState_LT[i]=1;
      else if(emaVal_LT>0.0&&accelVal_LT<0.0) hist_DecayState_LT[i]=2;
      else if(emaVal_LT<0.0&&accelVal_LT<0.0) hist_DecayState_LT[i]=-1;
      else if(emaVal_LT<0.0&&accelVal_LT>0.0) hist_DecayState_LT[i]=-2;
      else hist_DecayState_LT[i]=0;
   }
   int prev_AnchorType_ST=(i+1<limit)?hist_AnchorType_ST[i+1]:0;
   int prev_BarsSince_ST=(i+1<limit)?hist_BarsSinceFlip_ST[i+1]:0;
   int prev_AnchorType_LT=(i+1<limit)?hist_AnchorType_LT[i+1]:0;
   int prev_BarsSince_LT=(i+1<limit)?hist_BarsSinceFlip_LT[i+1]:0;
   int curr_AnchorType_ST=(st_Slope>0.0)?0:1;
   int curr_AnchorType_LT=(lt_Slope>0.0)?0:1;
   int st_Score=hist_trendStep_ST[i];
   bool isStandardFlip_ST=(prev_BarsSince_ST>1)&&(st_Score<=(7-(int)T2T_Standard_Flip_Score)||st_Score>=(7+(int)T2T_Standard_Flip_Score));
   bool isStrongFlip_ST=(prev_BarsSince_ST==1)&&(st_Score<=(7-(int)T2T_Strong_Flip_Score)||st_Score>=(7+(int)T2T_Strong_Flip_Score));
   int new_AnchorType_ST=prev_AnchorType_ST;
   int new_BarsSince_ST=prev_BarsSince_ST+1;
   hist_ST_Flip_Event[i]=0;
   if(curr_AnchorType_ST!=prev_AnchorType_ST&&(isStandardFlip_ST||isStrongFlip_ST)) {
      new_AnchorType_ST=curr_AnchorType_ST;
      new_BarsSince_ST=0;
      hist_ST_Flip_Event[i]=(new_AnchorType_ST==0)?1:-1;
   }
   hist_AnchorType_ST[i]=new_AnchorType_ST;
   hist_BarsSinceFlip_ST[i]=new_BarsSince_ST;
   if(!UseTrendAnchor || hist_ST_Flip_Event[i]!=0) {
      hist_detectedSlope_ST[i]=st_Slope;
      hist_detectedAnchorBar_ST[i]=st_Len;
   } else {
      if(i+1<limit) {
         hist_detectedSlope_ST[i]=hist_detectedSlope_ST[i+1];
         hist_detectedAnchorBar_ST[i]=hist_detectedAnchorBar_ST[i+1];
      } else {
         hist_detectedSlope_ST[i]=st_Slope;
         hist_detectedAnchorBar_ST[i]=st_Len;
      }
   }
   int lt_Score=hist_LT_trendStep_ST[i];
   bool isStandardFlip_LT=(prev_BarsSince_LT>1)&&(lt_Score<=(7-(int)T2T_Standard_Flip_Score)||lt_Score>=(7+(int)T2T_Standard_Flip_Score));
   bool isStrongFlip_LT=(prev_BarsSince_LT==1)&&(lt_Score<=(7-(int)T2T_Strong_Flip_Score)||lt_Score>=(7+(int)T2T_Strong_Flip_Score));
   int new_AnchorType_LT=prev_AnchorType_LT;
   int new_BarsSince_LT=prev_BarsSince_LT+1;
   if(curr_AnchorType_LT!=prev_AnchorType_LT&&(isStandardFlip_LT||isStrongFlip_LT)) {
      new_AnchorType_LT=curr_AnchorType_LT;
      new_BarsSince_LT=0;
   }
   hist_AnchorType_LT[i]=new_AnchorType_LT;
   hist_BarsSinceFlip_LT[i]=new_BarsSince_LT;
   if(!UseTrendAnchor || new_BarsSince_LT==0) {
      hist_LT_detectedSlope_ST[i]=lt_Slope;
      hist_LT_detectedAnchorBar_ST[i]=lt_Len;
   } else {
      if(i+1<limit) {
         hist_LT_detectedSlope_ST[i]=hist_LT_detectedSlope_ST[i+1];
         hist_LT_detectedAnchorBar_ST[i]=hist_LT_detectedAnchorBar_ST[i+1];
      } else {
         hist_LT_detectedSlope_ST[i]=lt_Slope;
         hist_LT_detectedAnchorBar_ST[i]=lt_Len;
      }
   }
   if(i<=1) {
      Master_detectedSlope_ST=st_Slope; detectedIntercept_ST=st_Int; detectedStdDev_ST=st_Std; detectedPeriod_ST=st_Len;
      detectedSlope_ST=hist_detectedSlope_ST[i];
      detectedAnchorBar_ST=i+hist_BarsSinceFlip_ST[i]+hist_detectedAnchorBar_ST[i]-1;
      static_ST_barsSinceReset_Live=new_BarsSince_ST;
      double slope_scaler=15000.0;
      double angle_rad=MathArctan(detectedSlope_ST*slope_scaler);
      detectedAngle_ST=90.0+(angle_rad*180.0/M_PI);
      Master_LT_detectedSlope_ST=lt_Slope; LT_detectedIntercept_ST=lt_Int; LT_detectedStdDev_ST=lt_Std; LT_detectedPeriod_ST=lt_Len;
      LT_detectedSlope_ST=hist_LT_detectedSlope_ST[i];
      LT_detectedAnchorBar_ST=i+hist_BarsSinceFlip_LT[i]+hist_LT_detectedAnchorBar_ST[i]-1;
      live_DecayState_ST=hist_DecayState_ST[i];
      live_DecayState_LT=hist_DecayState_LT[i];
      live_SlopeAccel_ST=state_Slope_Accel_ST[i];
      live_SlopeAccel_LT=state_Slope_Accel_LT[i];
   }
}
//+------------------------------------------------------------------+
//| SECTION 6.2 - HISTORICAL DRAWING (STATEFUL)                      |
//+------------------------------------------------------------------+
void DrawHistoricalIndicators_FromBuffers() {
   int limit=ArraySize(hist_detectedSlope_ST);
   if(limit<=0) return;
   int chartHeight=(int)ChartGetInteger(0,CHART_HEIGHT_IN_PIXELS);
   if(chartHeight<=0) return;
   double p2p=(WindowPriceMax()-WindowPriceMin())/chartHeight;
   double ltCoord=WindowPriceMax()-(18*p2p);
   double stCoord=WindowPriceMax()-(32*p2p);
   double t2tSigCoord=WindowPriceMax()-(80*p2p);
   double priceRange=WindowPriceMax()-WindowPriceMin();
   double decayCoord=WindowPriceMax()-(priceRange*0.12);
   int decayLimit=ArraySize(hist_DecayState_ST);
   int scoreLimit=ArraySize(hist_trendStep_ST);
   if(g_isLoading) LogBootMessage("Visuals: Rendering History ("+IntegerToString(limit)+" bars)...");
   for(int i=limit-1; i>=1; i--) {
      if(g_isLoading&&i%500==0) {
          LogBootMessage("Visuals: Drawing Bar "+IntegerToString(i));
          ChartRedraw();
      }
      if(i>=Bars-1) continue; 
      if(trendDirection&&isTrendVisualsVisible) {
         if(hist_detectedSlope_ST[i]!=0) DrawST_TrendDirectionIndicator_Historical(i,hist_AnchorType_ST[i],stCoord);
         if(hist_LT_detectedSlope_ST[i]!=0) DrawLT_TrendDirectionIndicator_Historical(i,hist_AnchorType_LT[i],ltCoord);
         if(hist_ST_Flip_Event[i]!=0) {
            datetime signalTime=Time[i];
            int flipDirection=hist_ST_Flip_Event[i];
            color vlineColor=(flipDirection==1)?C'146,134,124':C'89,116,124';
            string vlineName=ea_prefix+"t2t_vline_"+TimeToString(signalTime);
            if(ObjectFind(0,vlineName)<0) {
               ObjectCreate(0,vlineName,OBJ_VLINE,0,signalTime,0);
               ObjectSetInteger(0,vlineName,OBJPROP_COLOR,vlineColor);
               ObjectSetInteger(0,vlineName,OBJPROP_STYLE,STYLE_SOLID);
               ObjectSetInteger(0,vlineName,OBJPROP_WIDTH,1);
               ObjectSetInteger(0,vlineName,OBJPROP_BACK,true);
               ObjectSetInteger(0,vlineName,OBJPROP_SELECTABLE,false);
            }
            int arrowCode=(flipDirection==1)?233:234;
            DrawArrow("t2t_sig_top_"+TimeToString(signalTime),signalTime,t2tSigCoord,vlineColor,1,arrowCode);
         }
         if(i+1<decayLimit&&i<scoreLimit) {
            if(hist_DecayState_ST[i]==2&&hist_DecayState_ST[i+1]==1&&hist_trendStep_ST[i]!=7)
               DrawArrow("decay_sig_st_"+TimeToString(Time[i]),Time[i],decayCoord,C'89,116,124',1,234);
            else if(hist_DecayState_ST[i]==-2&&hist_DecayState_ST[i+1]==-1&&hist_trendStep_ST[i]!=7)
               DrawArrow("decay_sig_st_"+TimeToString(Time[i]),Time[i],decayCoord,C'146,134,124',1,233);
         }
      }
      if(isOBVVisualsVisible) DrawOBV_Visuals_Historical(i);
   }
}
void DrawST_TrendDirectionIndicator_Historical(int barIndex,int anchorType,double yCoord) {
   if(!trendDirection||!isTrendVisualsVisible) return;
   if(barIndex<1||barIndex>=Bars-1) return;
   color lineColor;
   if(anchorType==0) lineColor=C'146,134,124';
   else if(anchorType==1) lineColor=C'89,116,124';
   else return;
   int lineWidth=5;
   datetime barTime=Time[barIndex];
   string timeStr=TimeToString(barTime);
   string topName=ea_prefix+"td_top_"+timeStr;
   if(ObjectFind(0,topName)<0) {
      ObjectCreate(0,topName,OBJ_TREND,0,Time[barIndex+1],yCoord,Time[barIndex],yCoord);
      ObjectSetInteger(0,topName,OBJPROP_COLOR,lineColor);
      ObjectSetInteger(0,topName,OBJPROP_WIDTH,lineWidth);
      ObjectSetInteger(0,topName,OBJPROP_STYLE,STYLE_SOLID);
      ObjectSetInteger(0,topName,OBJPROP_SELECTABLE,false);
      ObjectSetInteger(0,topName,OBJPROP_BACK,true);
      ObjectSetInteger(0,topName,OBJPROP_RAY_RIGHT,false);
   }
}
void DrawLT_TrendDirectionIndicator_Historical(int barIndex,int anchorType,double yCoord) {
   if(!trendDirection||!isTrendVisualsVisible) return;
   if(barIndex<1||barIndex>=Bars-1) return;
   color lineColor;
   if(anchorType==0) lineColor=C'146,134,124';
   else if(anchorType==1) lineColor=C'89,116,124';
   else return;
   int lineWidth=5;
   datetime barTime=Time[barIndex];
   string timeStr=TimeToString(barTime);
   string topName=ea_prefix+"lttd_top_"+timeStr;
   if(ObjectFind(0,topName)<0) {
      ObjectCreate(0,topName,OBJ_TREND,0,Time[barIndex+1],yCoord,Time[barIndex],yCoord);
      ObjectSetInteger(0,topName,OBJPROP_COLOR,lineColor);
      ObjectSetInteger(0,topName,OBJPROP_WIDTH,lineWidth);
      ObjectSetInteger(0,topName,OBJPROP_STYLE,STYLE_SOLID);
      ObjectSetInteger(0,topName,OBJPROP_SELECTABLE,false);
      ObjectSetInteger(0,topName,OBJPROP_BACK,true);
      ObjectSetInteger(0,topName,OBJPROP_RAY_RIGHT,false);
   }
}
//+------------------------------------------------------------------+
//| SECTION 6.3 - OBV CALCULATION                                    |
//+------------------------------------------------------------------+
double Calc_StDev_Buffer(double &buffer[],int index,int length) {
   int limit=ArraySize(buffer);
   if(index+length>=limit) return 1.0;
   double sum=0.0;
   double sumSq=0.0;
   for(int k=0; k<length; k++) {
      double val=buffer[index+k];
      sum+=val;
      sumSq+=val*val;
   }
   double mean=sum/length;
   double variance=(sumSq-(sum*mean))/(length>1?length-1:1);
   return (variance>0)?MathSqrt(variance):1.0;
}
double Calc_Pine_Slope_Forecast(double &src[],int index,int len) {
   int limit=ArraySize(src);
   if(index+len>=limit) return src[index];
   double sumX=0.0; double sumY=0.0; double sumXSqr=0.0; double sumXY=0.0;
   for(int i=1; i<=len; i++) {
      double val=src[index+(len-i)];
      double per=(double)(i+1.0);
      sumX+=per; sumY+=val; sumXSqr+=per*per; sumXY+=val*per;
   }
   double denom=(len*sumXSqr-sumX*sumX);
   if(denom==0) return src[index];
   double slope=(len*sumXY-sumX*sumY)/denom;
   double average=sumY/len;
   double intercept=average-slope*(sumX/len)+slope;
   return intercept+slope*len;
}
void Calc_OBV_OnBar(int i) {
   int limit=ArraySize(state_OBV_Final);
   if(i>=limit-145) return;
   double change=Close[i]-Close[i+1];
   double tr=MathMax(High[i]-Low[i],MathAbs(Close[i]-Close[i+1]));
   if(tr==0) tr=(double)Point;
   double fraction=change/tr;
   if(fraction>1.0) fraction=1.0;
   else if(fraction<-1.0) fraction=-1.0;
   double prev_obv=(i+1<limit)?state_OBV_Accum[i+1]:0.0;
   double currentVol=(i==0)?(double)Volume[0]:hist_VolumeValue[i];
   if(i>0 && currentVol<=0.0 && Volume[i]>0) {
      currentVol=(double)Volume[i];
      hist_VolumeValue[i]=currentVol;
   }
   state_OBV_Accum[i]=prev_obv+(fraction*currentVol);
   double src0=state_OBV_Accum[i];
   double src1=(i+1<limit)?state_OBV_Accum[i+1]:src0;
   double src2=(i+2<limit)?state_OBV_Accum[i+2]:src1;
   double llema_src=(0.25*src0)+(0.5*src1)+(0.25*src2);
   double alpha_ma=2.0/((double)OBV_Fast_MA+1.0);
   double prev_ma=(i+1<limit && state_OBV_Fast[i+1]!=0.0)?state_OBV_Fast[i+1]:llema_src;
   state_OBV_Fast[i]=(llema_src*alpha_ma)+(prev_ma*(1.0-alpha_ma));
   double alpha_slow=2.0/((double)OBV_Slow_MA+1.0);
   double prev_slow=(i+1<limit && state_OBV_Slow[i+1]!=0.0)?state_OBV_Slow[i+1]:state_OBV_Accum[i];
   state_OBV_Slow[i]=(state_OBV_Accum[i]*alpha_slow)+(prev_slow*(1.0-alpha_slow));
   state_OBV_Macd[i]=state_OBV_Fast[i]-state_OBV_Slow[i];
   double tt1=Calc_Pine_Slope_Forecast(state_OBV_Macd,i,3);
   double prev_b5=tt1;
   if(i+1<limit && state_TChan_B5[i+1]!=0.0) {
      prev_b5=state_TChan_B5[i+1];
   }
   double diff=MathAbs(tt1-prev_b5);
   double alpha_a15=2.0/(50.0+1.0);
   double prev_a15=(i+1<limit && state_TChan_Sum[i+1]!=0.0)?state_TChan_Sum[i+1]:diff;
   double a15=(diff*alpha_a15)+(prev_a15*(1.0-alpha_a15));
   double macd_stdev=Calc_StDev_Buffer(state_OBV_Macd,i,28);
   a15=MathMax(a15,macd_stdev*0.30);
   state_TChan_Sum[i]=a15;
   double b5=prev_b5;
   if(tt1>prev_b5+a15) b5=tt1;
   else if(tt1<prev_b5-a15) b5=tt1;
   state_TChan_B5[i]=b5;
   double prev_oc=(i+1<limit)?state_TChan_OC[i+1]:0.0;
   double oc=prev_oc;
   if(b5>prev_b5) oc=1.0;
   else if(b5<prev_b5) oc=-1.0;
   state_TChan_OC[i]=oc;
   state_OBV_Final[i]=b5;
   double prev_final=(i+1<limit)?state_OBV_Final[i+1]:b5;
   state_OBV_Velocity[i]=b5-prev_final;
}
//+------------------------------------------------------------------+
//| SECTION 6.4 - HARMONIC VOLUME LLEMA CALCULATION                  |
//+------------------------------------------------------------------+
void Calc_HarmVol_LLEMA_OnBar(int i) {
   int kamaDeadZoneDivisor=75;
   if(KAMA_DeadZone_Sensitivity==KAMA_DZ_CASUAL) kamaDeadZoneDivisor=200;
   else if(KAMA_DeadZone_Sensitivity==KAMA_DZ_AGGRESSIVE) kamaDeadZoneDivisor=30;
   int erPeriod=HARMVOL_ER_PERIOD;
   int limitKAMA=ArraySize(state_HarmVol_KAMA);
   int limitLLEMA=ArraySize(state_HarmVol_LLEMA);
   int limitEMA8=ArraySize(state_HarmVol_EMA8);
   int limitEMA21=ArraySize(state_HarmVol_EMA21);
   int limitEMAOsc=ArraySize(state_HarmVol_EMAOsc);
   if(i<0||i>=limitKAMA||i>=limitLLEMA) return;
   if(i>=limitEMA8||i>=limitEMA21||i>=limitEMAOsc) return;
   double alphaFast=2.0/(8.0+1.0);
   double alphaSlow=2.0/(21.0+1.0);
   if(i+1>=limitEMA8||i+1>=limitEMA21) {
      state_HarmVol_EMA8[i]=Close[i];
      state_HarmVol_EMA21[i]=Close[i];
   } else {
      state_HarmVol_EMA8[i]=(Close[i]*alphaFast)+(state_HarmVol_EMA8[i+1]*(1.0-alphaFast));
      state_HarmVol_EMA21[i]=(Close[i]*alphaSlow)+(state_HarmVol_EMA21[i+1]*(1.0-alphaSlow));
   }
   state_HarmVol_EMAOsc[i]=state_HarmVol_EMA8[i]-state_HarmVol_EMA21[i];
   if(i+erPeriod>=Bars) return;
   double direction=MathAbs(Close[i]-Close[i+erPeriod]);
   double volatility=0.0;
   for(int k=i; k<i+erPeriod; k++) {
      volatility+=MathAbs(Close[k]-Close[k+1]);
   }
   double er=0.0;
   if(volatility>0.0) er=direction/volatility;
   double fastSC=2.0/(2.0+1.0);
   double slowSC=2.0/(30.0+1.0);
   double sc=(er*(fastSC-slowSC)+slowSC);
   sc=sc*sc;
   bool isSeedBar=(i+1>=limitKAMA||i+1+erPeriod>=Bars);
   if(g_warm_valid&&g_warm_anchor_enabled&&Time[i]==g_warm_ts) {
      state_HarmVol_KAMA[i]=g_warm_kama;
      state_HarmVol_LLEMA[i]=0.0;
   } else if(isSeedBar) {
      int basisLimit=ArraySize(U_BasisBuffer);
      state_HarmVol_KAMA[i]=(i<basisLimit&&U_BasisBuffer[i]>0.0)?U_BasisBuffer[i]:Close[i];
      state_HarmVol_LLEMA[i]=0.0;
   } else {
      state_HarmVol_KAMA[i]=state_HarmVol_KAMA[i+1]+sc*(Close[i]-state_HarmVol_KAMA[i+1]);
      double rawSlope=state_HarmVol_KAMA[i]-state_HarmVol_KAMA[i+1];
      int atrLimit=ArraySize(assignedATR);
      double atr=(i<atrLimit)?assignedATR[i]:0.0;
      double deadZone=(atr>0.0&&kamaDeadZoneDivisor>0)?(atr/(double)kamaDeadZoneDivisor):0.0;
      if(MathAbs(rawSlope)<=deadZone) state_HarmVol_LLEMA[i]=0.0;
      else state_HarmVol_LLEMA[i]=rawSlope;
   }
}
//+------------------------------------------------------------------+
//| SECTION 6.5 - MICROSTRUCTURE CALCULATION (STATEFUL)              |
//+------------------------------------------------------------------+
void Calc_Microstructure_OnBar(int i) {
   int microLimit=ArraySize(state_Micro_IBSP);
   if(i<0||i>=microLimit) return;
   int atrLimit=ArraySize(assignedATR);
   double lnK_f23[3]={0.0};
   double lnL_f23[3]={0.0};
   double rets_f28[7]={0.0};
   double range_f1=High[i]-Low[i];
   state_Micro_IBSP[i]=(range_f1>0.0)?(Close[i]-Low[i])/range_f1:0.5;
   double vol_f2=(double)Volume[i];
   state_Micro_Lambda[i]=(vol_f2>0.0)?MathAbs(Close[i]-Open[i])/vol_f2:0.0;
   double range_f3=High[i]-Low[i];
   state_Micro_TickIntensity[i]=(range_f3>0.0)?(double)Volume[i]/range_f3:0.0;
   double hl_f4=(High[i]>Low[i])?MathLog(High[i]/Low[i]):0.0;
   double co_f4=(Close[i]>0.0&&Open[i]>0.0)?MathLog(Close[i]/Open[i]):0.0;
   state_Micro_GarmanKlass[i]=0.5*hl_f4*hl_f4-(2.0*MathLog(2.0)-1.0)*co_f4*co_f4;
   double lw_f5=MathMin(Open[i],Close[i])-Low[i];
   double uw_f5=High[i]-MathMax(Open[i],Close[i]);
   double range_f5=High[i]-Low[i];
   state_Micro_Rejection[i]=(range_f5>0.0)?(lw_f5-uw_f5)/range_f5:0.0;
   state_Micro_OrderFlowDelta[i]=(double)Volume[i]*(2.0*state_Micro_IBSP[i]-1.0);
   double range_f7=High[i]-Low[i];
   double eps_f7=1e-10;
   double lwPct_f7,bodyPct_f7,uwPct_f7;
   if(range_f7>0.0) {
      lwPct_f7=(MathMin(Open[i],Close[i])-Low[i])/range_f7;
      bodyPct_f7=MathAbs(Close[i]-Open[i])/range_f7;
      uwPct_f7=(High[i]-MathMax(Open[i],Close[i]))/range_f7;
   } else {
      lwPct_f7=1.0/3.0;
      bodyPct_f7=1.0/3.0;
      uwPct_f7=1.0/3.0;
   }
   double p1_f7=lwPct_f7+eps_f7;
   double p2_f7=bodyPct_f7+eps_f7;
   double p3_f7=uwPct_f7+eps_f7;
   double sum_f7=p1_f7+p2_f7+p3_f7;
   p1_f7/=sum_f7; p2_f7/=sum_f7; p3_f7/=sum_f7;
   state_Micro_BarEntropy[i]=-(p1_f7*MathLog(p1_f7)+p2_f7*MathLog(p2_f7)+p3_f7*MathLog(p3_f7))/MathLog(3.0);
   if(i+1>=Bars) {
      state_Micro_LogReturn[i]=0.0;
   } else {
      state_Micro_LogReturn[i]=(Close[i+1]>0.0&&Close[i]>0.0)?MathLog(Close[i]/Close[i+1]):0.0;
   }
   if(i+1>=microLimit) {
      state_Micro_PriceAccel[i]=0.0;
   } else {
      state_Micro_PriceAccel[i]=state_Micro_LogReturn[i]-state_Micro_LogReturn[i+1];
   }
   if(i+2>=Bars) {
      state_Micro_RollProxy[i]=0.0;
   } else {
      double delta0_f10=Close[i]-Close[i+1];
      double delta1_f10=Close[i+1]-Close[i+2];
      state_Micro_RollProxy[i]=delta0_f10*delta1_f10;
   }
   if(i+1>=Bars) {
      state_Micro_BarOverlap[i]=0.0;
   } else {
      double overlap_f11=MathMax(0.0,MathMin(High[i],High[i+1])-MathMax(Low[i],Low[i+1]));
      double range_f11=High[i]-Low[i];
      state_Micro_BarOverlap[i]=(range_f11>0.0)?overlap_f11/range_f11:1.0;
   }
   if(i+1>=Bars) {
      state_Micro_FailedBreak[i]=0.0;
   } else {
      double score_f12=0.0;
      if(High[i]>High[i+1])
         score_f12+=(Close[i]-High[i+1]);
      if(Low[i]<Low[i+1])
         score_f12-=(Low[i+1]-Close[i]);
      double atr_f12=(i<atrLimit)?assignedATR[i]:1.0;
      state_Micro_FailedBreak[i]=(atr_f12>0.0)?score_f12/atr_f12:0.0;
   }
   if(i+1>=Bars) {
      state_Micro_MomoTransfer[i]=0.0;
   } else {
      double dir0_f13=Close[i]-Open[i];
      double dir1_f13=Close[i+1]-Open[i+1];
      double mag1_f13=MathAbs(dir1_f13);
      double vol1_f13=(double)Volume[i+1];
      double ratio_mag_f13=(mag1_f13>0.0)?MathAbs(dir0_f13)/mag1_f13:0.0;
      double ratio_vol_f13=(vol1_f13>0.0)?(double)Volume[i]/vol1_f13:0.0;
      double sign_agree_f13=(dir0_f13*dir1_f13>0.0)?1.0:-1.0;
      state_Micro_MomoTransfer[i]=sign_agree_f13*ratio_mag_f13*ratio_vol_f13;
   }
   if(i+1>=Bars) {
      state_Micro_MicroGap[i]=0.0;
   } else {
      state_Micro_MicroGap[i]=Open[i]-Close[i+1];
   }
   if(i+1>=Bars) {
      state_Micro_HLAsymmetry[i]=0.0;
   } else {
      state_Micro_HLAsymmetry[i]=(High[i]-High[i+1])+(Low[i]-Low[i+1]);
   }
   if(i+2>=Bars) {
      state_Micro_VolAccel[i]=0.0;
   } else {
      state_Micro_VolAccel[i]=(double)Volume[i]-2.0*(double)Volume[i+1]+(double)Volume[i+2];
   }
   if(i+1>=Bars) {
      state_Micro_RangeVelocity[i]=0.0;
   } else {
      state_Micro_RangeVelocity[i]=(High[i]-Low[i])-(High[i+1]-Low[i+1]);
   }
   if(i+2>=Bars) {
      state_Micro_RangeAccel[i]=0.0;
   } else {
      double rv0_f18=(High[i]-Low[i])-(High[i+1]-Low[i+1]);
      double rv1_f18=(High[i+1]-Low[i+1])-(High[i+2]-Low[i+2]);
      state_Micro_RangeAccel[i]=rv0_f18-rv1_f18;
   }
   if(i+2>=Bars) {
      state_Micro_ThrustEff[i]=0.0;
   } else {
      double net_f19=Close[i]-Open[i+2];
      double path_f19=(High[i]-Low[i])+(High[i+1]-Low[i+1])+(High[i+2]-Low[i+2]);
      state_Micro_ThrustEff[i]=(path_f19>0.0)?net_f19/path_f19:0.0;
   }
   int N_f20=10;
   if(i+N_f20+1>=Bars) {
      state_Micro_AutoCorr[i]=0.0;
   } else {
      int len_f20=N_f20-1;
      double sumX_f20=0.0,sumY_f20=0.0,sumXY_f20=0.0,sumX2_f20=0.0,sumY2_f20=0.0;
      for(int k=0; k<len_f20; k++) {
         double x_f20=Close[i+k]-Close[i+k+1];
         double y_f20=Close[i+k+1]-Close[i+k+2];
         sumX_f20+=x_f20;
         sumY_f20+=y_f20;
         sumXY_f20+=x_f20*y_f20;
         sumX2_f20+=x_f20*x_f20;
         sumY2_f20+=y_f20*y_f20;
      }
      double n_f20=(double)len_f20;
      double denom_f20=MathSqrt((n_f20*sumX2_f20-sumX_f20*sumX_f20)*(n_f20*sumY2_f20-sumY_f20*sumY_f20));
      if(denom_f20>0.0)
         state_Micro_AutoCorr[i]=(n_f20*sumXY_f20-sumX_f20*sumY_f20)/denom_f20;
      else
         state_Micro_AutoCorr[i]=0.0;
   }
   int N_f21=10;
   if(i+N_f21>=Bars) {
      state_Micro_Entropy[i]=0.0;
   } else {
      double flatThresh_f21=(i<atrLimit)?assignedATR[i]*0.25:0.0;
      int upCount_f21=0;
      int dnCount_f21=0;
      int flatCount_f21=0;
      for(int k=0; k<N_f21; k++) {
         double diff_f21=Close[i+k]-Close[i+k+1];
         if(diff_f21>flatThresh_f21) upCount_f21++;
         else if(diff_f21<-flatThresh_f21) dnCount_f21++;
         else flatCount_f21++;
      }
      double pUp_f21=(double)upCount_f21/(double)N_f21;
      double pDn_f21=(double)dnCount_f21/(double)N_f21;
      double pFl_f21=(double)flatCount_f21/(double)N_f21;
      double entropy_f21=0.0;
      if(pUp_f21>0.0) entropy_f21-=pUp_f21*MathLog(pUp_f21);
      if(pDn_f21>0.0) entropy_f21-=pDn_f21*MathLog(pDn_f21);
      if(pFl_f21>0.0) entropy_f21-=pFl_f21*MathLog(pFl_f21);
      state_Micro_Entropy[i]=entropy_f21/MathLog(3.0);
   }
   int N_f22=10;
   if(i+N_f22>=microLimit||i+N_f22>=Bars) {
      state_Micro_VPIN[i]=0.0;
   } else {
      double sumBuy_f22=0.0,sumSell_f22=0.0,sumVol_f22=0.0;
      for(int k=0; k<N_f22; k++) {
         double v_f22=(double)Volume[i+k];
         double ibsp_f22=state_Micro_IBSP[i+k];
         sumBuy_f22+=v_f22*ibsp_f22;
         sumSell_f22+=v_f22*(1.0-ibsp_f22);
         sumVol_f22+=v_f22;
      }
      state_Micro_VPIN[i]=(sumVol_f22>0.0)?MathAbs(sumBuy_f22-sumSell_f22)/sumVol_f22:0.0;
   }
   int N_f23=10;
   int kmax_f23=3;
   if(i+N_f23>=Bars) {
      state_Micro_FractalDim[i]=1.5;
   } else {
      bool valid_f23=true;
      for(int kk=1; kk<=kmax_f23; kk++) {
         double sumLm_f23=0.0;
         int countLm_f23=0;
         for(int m=0; m<kk; m++) {
            int nk_f23=(int)MathFloor((double)(N_f23-1-m)/(double)kk);
            if(nk_f23<1) continue;
            double length_f23=0.0;
            for(int j=1; j<=nk_f23; j++) {
               int idx1_f23=m+j*kk;
               int idx0_f23=m+(j-1)*kk;
               length_f23+=MathAbs(Close[i+N_f23-1-idx1_f23]-Close[i+N_f23-1-idx0_f23]);
            }
            double norm_f23=(double)(N_f23-1)/((double)nk_f23*(double)kk*(double)kk);
            sumLm_f23+=length_f23*norm_f23;
            countLm_f23++;
         }
         if(countLm_f23>0) {
            double Lk_f23=sumLm_f23/(double)countLm_f23;
            if(Lk_f23>0.0) {
               lnK_f23[kk-1]=MathLog(1.0/(double)kk);
               lnL_f23[kk-1]=MathLog(Lk_f23);
            } else {
               valid_f23=false;
            }
         } else {
            valid_f23=false;
         }
      }
      if(valid_f23) {
         double sx_f23=0.0,sy_f23=0.0,sxy_f23=0.0,sx2_f23=0.0;
         for(int q=0; q<kmax_f23; q++) {
            sx_f23+=lnK_f23[q];
            sy_f23+=lnL_f23[q];
            sxy_f23+=lnK_f23[q]*lnL_f23[q];
            sx2_f23+=lnK_f23[q]*lnK_f23[q];
         }
         double n_f23=(double)kmax_f23;
         double denom_f23=n_f23*sx2_f23-sx_f23*sx_f23;
         double fd_f23=1.5;
         if(MathAbs(denom_f23)>1e-20)
            fd_f23=(n_f23*sxy_f23-sx_f23*sy_f23)/denom_f23;
         if(fd_f23<1.0) fd_f23=1.0;
         if(fd_f23>2.0) fd_f23=2.0;
         state_Micro_FractalDim[i]=fd_f23;
      } else {
         state_Micro_FractalDim[i]=1.5;
      }
   }
   int N_f24=10;
   if(i+N_f24>=Bars) {
      state_Micro_VolOfVol[i]=0.0;
   } else {
      double sum_f24=0.0;
      for(int k=0; k<N_f24; k++) {
         sum_f24+=(High[i+k]-Low[i+k]);
      }
      double mean_f24=sum_f24/(double)N_f24;
      double sumSq_f24=0.0;
      for(int k=0; k<N_f24; k++) {
         double diff_f24=(High[i+k]-Low[i+k])-mean_f24;
         sumSq_f24+=diff_f24*diff_f24;
      }
      double stddev_f24=MathSqrt(sumSq_f24/(double)N_f24);
      state_Micro_VolOfVol[i]=(mean_f24>0.0)?stddev_f24/mean_f24:0.0;
   }
   int N_f25=3;
   if(i+N_f25>=Bars) {
      state_Micro_Amihud[i]=0.0;
   } else {
      double sum_f25=0.0;
      for(int k=0; k<N_f25; k++) {
         double ret_f25=MathAbs(Close[i+k]-Close[i+k+1]);
         double vol_f25=(double)Volume[i+k];
         sum_f25+=(vol_f25>0.0)?ret_f25/vol_f25:0.0;
      }
      state_Micro_Amihud[i]=sum_f25/(double)N_f25;
   }
   int N_f26=10;
   if(i+N_f26>=Bars) {
      state_Micro_WickImbalance[i]=0.0;
   } else {
      double sumLW_f26=0.0,sumUW_f26=0.0;
      for(int k=0; k<N_f26; k++) {
         sumLW_f26+=MathMin(Open[i+k],Close[i+k])-Low[i+k];
         sumUW_f26+=High[i+k]-MathMax(Open[i+k],Close[i+k]);
      }
      state_Micro_WickImbalance[i]=sumLW_f26-sumUW_f26;
   }
   int N_f27=10;
   if(i+N_f27>=Bars) {
      state_Micro_CSSpread[i]=0.0;
   } else {
      double sum_f27=0.0;
      for(int k=0; k<N_f27; k++) {
         if(i+k+1>=Bars) break;
         double h2_f27=MathMax(High[i+k],High[i+k+1]);
         double l2_f27=MathMin(Low[i+k],Low[i+k+1]);
         double r1_f27=High[i+k]-Low[i+k];
         double r2_f27=High[i+k+1]-Low[i+k+1];
         double denom_f27=r1_f27+r2_f27;
         sum_f27+=(denom_f27>0.0)?(h2_f27-l2_f27)/denom_f27:1.0;
      }
      state_Micro_CSSpread[i]=sum_f27/(double)N_f27;
   }
   int N_f28=8;
   if(i+N_f28>=Bars) {
      state_Micro_Hurst[i]=0.5;
   } else {
      int numRet_f28=N_f28-1;
      double sumRet_f28=0.0;
      for(int j=0; j<numRet_f28; j++) {
         rets_f28[j]=Close[i+N_f28-2-j]-Close[i+N_f28-1-j];
         sumRet_f28+=rets_f28[j];
      }
      double meanRet_f28=sumRet_f28/(double)numRet_f28;
      double cumDev_f28=0.0;
      double maxCum_f28=-DBL_MAX;
      double minCum_f28=DBL_MAX;
      for(int j=0; j<numRet_f28; j++) {
         cumDev_f28+=(rets_f28[j]-meanRet_f28);
         if(cumDev_f28>maxCum_f28) maxCum_f28=cumDev_f28;
         if(cumDev_f28<minCum_f28) minCum_f28=cumDev_f28;
      }
      double R_f28=maxCum_f28-minCum_f28;
      double sumSq_f28=0.0;
      for(int j=0; j<numRet_f28; j++) {
         double diff_f28=rets_f28[j]-meanRet_f28;
         sumSq_f28+=diff_f28*diff_f28;
      }
      double S_f28=MathSqrt(sumSq_f28/(double)numRet_f28);
      double hurst_f28=0.5;
      if(S_f28>0.0)
         hurst_f28=MathLog(R_f28/S_f28)/MathLog((double)N_f28);
      if(hurst_f28<0.0) hurst_f28=0.0;
      if(hurst_f28>1.0) hurst_f28=1.0;
      state_Micro_Hurst[i]=hurst_f28;
   }
}
//+------------------------------------------------------------------+
//| SECTION 6.6 - DOTS DERIVED CALCULATION                           |
//+------------------------------------------------------------------+
void Calc_Dots_Derived_OnBar(int i) {
   int limitWick=ArraySize(hist_UpperWick);
   int limitVR=ArraySize(hist_VolumeRatio10);
   int limitKS=ArraySize(hist_KAMA_Slope);
   int limitKD=ArraySize(hist_KAMA_Dist_ATR);
   if(i<0||i>=limitWick||i>=limitVR||i>=limitKS||i>=limitKD) return;
   hist_UpperWick[i]=High[i]-MathMax(Open[i],Close[i]);
   int limitVol=ArraySize(hist_VolumeValue);
   double avgVol=0.0;
   int avgCount=0;
   int maxJ=i+10;
   if(maxJ>=limitVol) maxJ=limitVol-1;
   for(int j=i+1; j<=maxJ; j++) {
      double v=hist_VolumeValue[j];
      if(v>0.0) { avgVol+=v; avgCount++; }
   }
   if(avgCount>0) avgVol/=(double)avgCount;
   if(avgVol>0.0&&i<limitVol) hist_VolumeRatio10[i]=hist_VolumeValue[i]/avgVol;
   else hist_VolumeRatio10[i]=0.0;
   int limitKAMA=ArraySize(state_HarmVol_KAMA);
   if(i+1<limitKAMA) hist_KAMA_Slope[i]=state_HarmVol_KAMA[i]-state_HarmVol_KAMA[i+1];
   else hist_KAMA_Slope[i]=0.0;
   int limitATR=ArraySize(assignedATR);
   double kamaVal=(i<limitKAMA)?state_HarmVol_KAMA[i]:Close[i];
   double atr=(i<limitATR)?assignedATR[i]:0.0;
   double kamaDist=Close[i]-kamaVal;
   if(atr>0.0) hist_KAMA_Dist_ATR[i]=kamaDist/atr;
   else hist_KAMA_Dist_ATR[i]=0.0;
}
//+------------------------------------------------------------------+
//| SECTION 6.7 - DOTS FEATURE CONSTANTS                             |
//+------------------------------------------------------------------+
double dots_threshold[DOTS_NUM_FEATURES][2];
double dots_roll_buf_AT_Slope_LT[];
double dots_roll_buf_Slope_EMA_LT[];
double dots_roll_buf_OBV_Macd[];
double dots_roll_buf_Harmonic_LLEMA[];
double dots_roll_buf_RangeOsc_Val[];
double dots_roll_buf_ATR_1M[];
double dots_roll_buf_Bar_Range[];
double dots_roll_buf_D2D_ATR[];
double dots_roll_buf_D2D_ATR_MA[];
double dots_roll_buf_D2D_Dn_Count[];
double dots_roll_buf_D2D_Dynamic_Sensitivity[];
double dots_roll_buf_D2D_Persist[];
double dots_roll_buf_D2D_Up_Count[];
double dots_roll_buf_AT_Lookback_LT[];
double dots_roll_buf_AT_Lookback_ST[];
double dots_roll_buf_AT_Score_LT[];
double dots_roll_buf_AT_Score_ST[];
double dots_roll_buf_AT_Slope_ST[];
double dots_roll_buf_Bars_Since_Flip[];
double dots_roll_buf_Slope_EMA_ST[];
double dots_roll_buf_Slope_Accel_LT[];
double dots_roll_buf_Slope_Accel_ST[];
double dots_roll_buf_OBV_Velocity[];
double dots_roll_buf_OBVf_DirStepCount[];
double dots_roll_buf_KAMA_Dist[];
double dots_roll_buf_KAMA_Dist_ATR[];
double dots_roll_buf_KAMA_Slope[];
double dots_roll_buf_EMA_Oscillator[];
double dots_roll_buf_Sqz_Val[];
double dots_roll_buf_Volume_Avg_10[];
double dots_roll_buf_Volume_Ratio_10[];
double dots_roll_buf_Momentum_Value[];
double dots_roll_buf_Efficiency_Ratio[];
double dots_roll_buf_Dist_To_PoC_ATR[];
double dots_roll_buf_Micro_Amihud[];
double dots_roll_buf_Micro_AutoCorr[];
double dots_roll_buf_Micro_BarEntropy[];
double dots_roll_buf_Micro_BarOverlap[];
double dots_roll_buf_Micro_CSSpread[];
double dots_roll_buf_Micro_Entropy[];
double dots_roll_buf_Micro_FailedBreak[];
double dots_roll_buf_Micro_FractalDim[];
double dots_roll_buf_Micro_GarmanKlass[];
double dots_roll_buf_Micro_HLAsymmetry[];
double dots_roll_buf_Micro_Hurst[];
double dots_roll_buf_Micro_IBSP[];
double dots_roll_buf_Micro_Lambda[];
double dots_roll_buf_Micro_LogReturn[];
double dots_roll_buf_Micro_MicroGap[];
double dots_roll_buf_Micro_MomoTransfer[];
double dots_roll_buf_Micro_OrderFlowDelta[];
double dots_roll_buf_Micro_PriceAccel[];
double dots_roll_buf_Micro_RangeAccel[];
double dots_roll_buf_Micro_RangeVelocity[];
double dots_roll_buf_Micro_Rejection[];
double dots_roll_buf_Micro_RollProxy[];
double dots_roll_buf_Micro_ThrustEff[];
double dots_roll_buf_Micro_TickIntensity[];
double dots_roll_buf_Micro_VPIN[];
double dots_roll_buf_Micro_VolAccel[];
double dots_roll_buf_Micro_VolOfVol[];
double dots_roll_buf_Micro_WickImbalance[];
double dots_roll_buf_VWAP_Dist_ATR[];
double dots_roll_buf_VAH_Dist_ATR[];
double dots_roll_buf_VAL_Dist_ATR[];
double dots_roll_buf_PrevDay_High_Dist_ATR[];
double dots_roll_buf_PrevDay_Low_Dist_ATR[];
double dots_roll_buf_PrevDay_Close_Dist_ATR[];
double dots_roll_buf_DailyOpen_Dist_ATR[];
double dots_roll_buf_Round_100_Dist_ATR[];
double dots_roll_buf_Round_500_Dist_ATR[];
double dots_roll_buf_Round_1000_Dist_ATR[];
double dots_roll_buf_OR_High_Dist_ATR[];
double dots_roll_buf_OR_Low_Dist_ATR[];
double dots_roll_buf_Session_High_Dist_ATR[];
double dots_roll_buf_Session_Low_Dist_ATR[];
double dots_roll_buf_WeeklyOpen_Dist_ATR[];
double dots_roll_buf_MultiDay_Slope[];
double dots_roll_buf_MultiDay_Position[];
double dots_roll_buf_VWAP_Sigma_ATR[];
double dots_roll_buf_VA_Position[];
double dots_roll_buf_ADX_Value[];
double dots_roll_buf_Body_Size[];
double dots_roll_buf_Upper_Wick[];
double dots_roll_buf_Lower_Wick[];
double dots_roll_buf_TChan_A15[];
double dots_roll_buf_VWAP_Sigma[];
double dots_roll_buf_Volume[];
int    dots_roll_ptr[DOTS_NUM_FEATURES];
int    dots_roll_cnt[DOTS_NUM_FEATURES];
int    dots_roll_lastRefreshDay=0;
void InitDotsThresholds() {
   int i,j;
   for(i=0;i<DOTS_NUM_FEATURES;i++)
      for(j=0;j<2;j++) {
         dots_threshold[i][j]=0.0;
      }
   dots_threshold[FEAT_VWAP_Z][THR_HI]=2.0;
   dots_threshold[FEAT_VWAP_Z][THR_LO]=-2.0;
   dots_threshold[FEAT_OR_Position][THR_HI]=0.80;
   dots_threshold[FEAT_OR_Position][THR_LO]=0.20;
   ArrayResize(dots_roll_buf_AT_Slope_LT,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Slope_EMA_LT,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_OBV_Macd,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Harmonic_LLEMA,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_RangeOsc_Val,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_ATR_1M,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Bar_Range,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_D2D_ATR,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_D2D_ATR_MA,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_D2D_Dn_Count,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_D2D_Dynamic_Sensitivity,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_D2D_Persist,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_D2D_Up_Count,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_AT_Lookback_LT,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_AT_Lookback_ST,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_AT_Score_LT,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_AT_Score_ST,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_AT_Slope_ST,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Bars_Since_Flip,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Slope_EMA_ST,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Slope_Accel_LT,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Slope_Accel_ST,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_OBV_Velocity,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_OBVf_DirStepCount,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_KAMA_Dist,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_KAMA_Dist_ATR,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_KAMA_Slope,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_EMA_Oscillator,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Sqz_Val,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Volume_Avg_10,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Volume_Ratio_10,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Momentum_Value,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Efficiency_Ratio,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Dist_To_PoC_ATR,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Micro_Amihud,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Micro_AutoCorr,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Micro_BarEntropy,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Micro_BarOverlap,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Micro_CSSpread,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Micro_Entropy,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Micro_FailedBreak,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Micro_FractalDim,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Micro_GarmanKlass,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Micro_HLAsymmetry,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Micro_Hurst,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Micro_IBSP,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Micro_Lambda,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Micro_LogReturn,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Micro_MicroGap,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Micro_MomoTransfer,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Micro_OrderFlowDelta,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Micro_PriceAccel,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Micro_RangeAccel,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Micro_RangeVelocity,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Micro_Rejection,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Micro_RollProxy,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Micro_ThrustEff,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Micro_TickIntensity,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Micro_VPIN,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Micro_VolAccel,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Micro_VolOfVol,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Micro_WickImbalance,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_VWAP_Dist_ATR,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_VAH_Dist_ATR,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_VAL_Dist_ATR,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_PrevDay_High_Dist_ATR,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_PrevDay_Low_Dist_ATR,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_PrevDay_Close_Dist_ATR,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_DailyOpen_Dist_ATR,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Round_100_Dist_ATR,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Round_500_Dist_ATR,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Round_1000_Dist_ATR,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_OR_High_Dist_ATR,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_OR_Low_Dist_ATR,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Session_High_Dist_ATR,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Session_Low_Dist_ATR,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_WeeklyOpen_Dist_ATR,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_MultiDay_Slope,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_MultiDay_Position,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_VWAP_Sigma_ATR,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_VA_Position,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_ADX_Value,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Body_Size,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Upper_Wick,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Lower_Wick,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_TChan_A15,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_VWAP_Sigma,Dots_RollingBufferSize);
   ArrayResize(dots_roll_buf_Volume,Dots_RollingBufferSize);
   ArrayInitialize(dots_roll_buf_AT_Slope_LT,0.0);
   ArrayInitialize(dots_roll_buf_Slope_EMA_LT,0.0);
   ArrayInitialize(dots_roll_buf_OBV_Macd,0.0);
   ArrayInitialize(dots_roll_buf_Harmonic_LLEMA,0.0);
   ArrayInitialize(dots_roll_buf_RangeOsc_Val,0.0);
   ArrayInitialize(dots_roll_buf_ATR_1M,0.0);
   ArrayInitialize(dots_roll_buf_Bar_Range,0.0);
   ArrayInitialize(dots_roll_buf_D2D_ATR,0.0);
   ArrayInitialize(dots_roll_buf_D2D_ATR_MA,0.0);
   ArrayInitialize(dots_roll_buf_D2D_Dn_Count,0.0);
   ArrayInitialize(dots_roll_buf_D2D_Dynamic_Sensitivity,0.0);
   ArrayInitialize(dots_roll_buf_D2D_Persist,0.0);
   ArrayInitialize(dots_roll_buf_D2D_Up_Count,0.0);
   ArrayInitialize(dots_roll_buf_AT_Lookback_LT,0.0);
   ArrayInitialize(dots_roll_buf_AT_Lookback_ST,0.0);
   ArrayInitialize(dots_roll_buf_AT_Score_LT,0.0);
   ArrayInitialize(dots_roll_buf_AT_Score_ST,0.0);
   ArrayInitialize(dots_roll_buf_AT_Slope_ST,0.0);
   ArrayInitialize(dots_roll_buf_Bars_Since_Flip,0.0);
   ArrayInitialize(dots_roll_buf_Slope_EMA_ST,0.0);
   ArrayInitialize(dots_roll_buf_Slope_Accel_LT,0.0);
   ArrayInitialize(dots_roll_buf_Slope_Accel_ST,0.0);
   ArrayInitialize(dots_roll_buf_OBV_Velocity,0.0);
   ArrayInitialize(dots_roll_buf_OBVf_DirStepCount,0.0);
   ArrayInitialize(dots_roll_buf_KAMA_Dist,0.0);
   ArrayInitialize(dots_roll_buf_KAMA_Dist_ATR,0.0);
   ArrayInitialize(dots_roll_buf_KAMA_Slope,0.0);
   ArrayInitialize(dots_roll_buf_EMA_Oscillator,0.0);
   ArrayInitialize(dots_roll_buf_Sqz_Val,0.0);
   ArrayInitialize(dots_roll_buf_Volume_Avg_10,0.0);
   ArrayInitialize(dots_roll_buf_Volume_Ratio_10,0.0);
   ArrayInitialize(dots_roll_buf_Momentum_Value,0.0);
   ArrayInitialize(dots_roll_buf_Efficiency_Ratio,0.0);
   ArrayInitialize(dots_roll_buf_Dist_To_PoC_ATR,0.0);
   ArrayInitialize(dots_roll_buf_Micro_Amihud,0.0);
   ArrayInitialize(dots_roll_buf_Micro_AutoCorr,0.0);
   ArrayInitialize(dots_roll_buf_Micro_BarEntropy,0.0);
   ArrayInitialize(dots_roll_buf_Micro_BarOverlap,0.0);
   ArrayInitialize(dots_roll_buf_Micro_CSSpread,0.0);
   ArrayInitialize(dots_roll_buf_Micro_Entropy,0.0);
   ArrayInitialize(dots_roll_buf_Micro_FailedBreak,0.0);
   ArrayInitialize(dots_roll_buf_Micro_FractalDim,0.0);
   ArrayInitialize(dots_roll_buf_Micro_GarmanKlass,0.0);
   ArrayInitialize(dots_roll_buf_Micro_HLAsymmetry,0.0);
   ArrayInitialize(dots_roll_buf_Micro_Hurst,0.0);
   ArrayInitialize(dots_roll_buf_Micro_IBSP,0.0);
   ArrayInitialize(dots_roll_buf_Micro_Lambda,0.0);
   ArrayInitialize(dots_roll_buf_Micro_LogReturn,0.0);
   ArrayInitialize(dots_roll_buf_Micro_MicroGap,0.0);
   ArrayInitialize(dots_roll_buf_Micro_MomoTransfer,0.0);
   ArrayInitialize(dots_roll_buf_Micro_OrderFlowDelta,0.0);
   ArrayInitialize(dots_roll_buf_Micro_PriceAccel,0.0);
   ArrayInitialize(dots_roll_buf_Micro_RangeAccel,0.0);
   ArrayInitialize(dots_roll_buf_Micro_RangeVelocity,0.0);
   ArrayInitialize(dots_roll_buf_Micro_Rejection,0.0);
   ArrayInitialize(dots_roll_buf_Micro_RollProxy,0.0);
   ArrayInitialize(dots_roll_buf_Micro_ThrustEff,0.0);
   ArrayInitialize(dots_roll_buf_Micro_TickIntensity,0.0);
   ArrayInitialize(dots_roll_buf_Micro_VPIN,0.0);
   ArrayInitialize(dots_roll_buf_Micro_VolAccel,0.0);
   ArrayInitialize(dots_roll_buf_Micro_VolOfVol,0.0);
   ArrayInitialize(dots_roll_buf_Micro_WickImbalance,0.0);
   ArrayInitialize(dots_roll_buf_VWAP_Dist_ATR,0.0);
   ArrayInitialize(dots_roll_buf_VAH_Dist_ATR,0.0);
   ArrayInitialize(dots_roll_buf_VAL_Dist_ATR,0.0);
   ArrayInitialize(dots_roll_buf_PrevDay_High_Dist_ATR,0.0);
   ArrayInitialize(dots_roll_buf_PrevDay_Low_Dist_ATR,0.0);
   ArrayInitialize(dots_roll_buf_PrevDay_Close_Dist_ATR,0.0);
   ArrayInitialize(dots_roll_buf_DailyOpen_Dist_ATR,0.0);
   ArrayInitialize(dots_roll_buf_Round_100_Dist_ATR,0.0);
   ArrayInitialize(dots_roll_buf_Round_500_Dist_ATR,0.0);
   ArrayInitialize(dots_roll_buf_Round_1000_Dist_ATR,0.0);
   ArrayInitialize(dots_roll_buf_OR_High_Dist_ATR,0.0);
   ArrayInitialize(dots_roll_buf_OR_Low_Dist_ATR,0.0);
   ArrayInitialize(dots_roll_buf_Session_High_Dist_ATR,0.0);
   ArrayInitialize(dots_roll_buf_Session_Low_Dist_ATR,0.0);
   ArrayInitialize(dots_roll_buf_WeeklyOpen_Dist_ATR,0.0);
   ArrayInitialize(dots_roll_buf_MultiDay_Slope,0.0);
   ArrayInitialize(dots_roll_buf_MultiDay_Position,0.0);
   ArrayInitialize(dots_roll_buf_VWAP_Sigma_ATR,0.0);
   ArrayInitialize(dots_roll_buf_VA_Position,0.0);
   ArrayInitialize(dots_roll_buf_ADX_Value,0.0);
   ArrayInitialize(dots_roll_buf_Body_Size,0.0);
   ArrayInitialize(dots_roll_buf_Upper_Wick,0.0);
   ArrayInitialize(dots_roll_buf_Lower_Wick,0.0);
   ArrayInitialize(dots_roll_buf_TChan_A15,0.0);
   ArrayInitialize(dots_roll_buf_VWAP_Sigma,0.0);
   ArrayInitialize(dots_roll_buf_Volume,0.0);
   ArrayInitialize(dots_roll_ptr,0);
   ArrayInitialize(dots_roll_cnt,0);
   dots_roll_lastRefreshDay=0;
}
double DotsPercentileFromBuffer(double &buf[],int count,double pct) {
   if(count<2) return 0.0;
   double tmp[];
   ArrayResize(tmp,count);
   ArrayCopy(tmp,buf,0,0,count);
   ArraySort(tmp);
   int idx=(int)MathFloor(count*pct);
   if(idx>=count) idx=count-1;
   if(idx<0) idx=0;
   return tmp[idx];
}
void DotsRefreshRollingPercentiles() {
   dots_threshold[FEAT_ATR_1M][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_ATR_1M,dots_roll_cnt[FEAT_ATR_1M],0.80);
   dots_threshold[FEAT_ATR_1M][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_ATR_1M,dots_roll_cnt[FEAT_ATR_1M],0.20);
   dots_threshold[FEAT_Bar_Range][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Bar_Range,dots_roll_cnt[FEAT_Bar_Range],0.80);
   dots_threshold[FEAT_Bar_Range][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Bar_Range,dots_roll_cnt[FEAT_Bar_Range],0.20);
   dots_threshold[FEAT_D2D_ATR][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_D2D_ATR,dots_roll_cnt[FEAT_D2D_ATR],0.80);
   dots_threshold[FEAT_D2D_ATR][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_D2D_ATR,dots_roll_cnt[FEAT_D2D_ATR],0.20);
   dots_threshold[FEAT_D2D_ATR_MA][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_D2D_ATR_MA,dots_roll_cnt[FEAT_D2D_ATR_MA],0.80);
   dots_threshold[FEAT_D2D_ATR_MA][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_D2D_ATR_MA,dots_roll_cnt[FEAT_D2D_ATR_MA],0.20);
   dots_threshold[FEAT_D2D_Dn_Count][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_D2D_Dn_Count,dots_roll_cnt[FEAT_D2D_Dn_Count],0.80);
   dots_threshold[FEAT_D2D_Dn_Count][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_D2D_Dn_Count,dots_roll_cnt[FEAT_D2D_Dn_Count],0.20);
   dots_threshold[FEAT_D2D_Dynamic_Sensitivity][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_D2D_Dynamic_Sensitivity,dots_roll_cnt[FEAT_D2D_Dynamic_Sensitivity],0.80);
   dots_threshold[FEAT_D2D_Dynamic_Sensitivity][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_D2D_Dynamic_Sensitivity,dots_roll_cnt[FEAT_D2D_Dynamic_Sensitivity],0.20);
   dots_threshold[FEAT_D2D_Persist][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_D2D_Persist,dots_roll_cnt[FEAT_D2D_Persist],0.80);
   dots_threshold[FEAT_D2D_Persist][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_D2D_Persist,dots_roll_cnt[FEAT_D2D_Persist],0.20);
   dots_threshold[FEAT_D2D_Up_Count][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_D2D_Up_Count,dots_roll_cnt[FEAT_D2D_Up_Count],0.80);
   dots_threshold[FEAT_D2D_Up_Count][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_D2D_Up_Count,dots_roll_cnt[FEAT_D2D_Up_Count],0.20);
   dots_threshold[FEAT_AT_Lookback_LT][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_AT_Lookback_LT,dots_roll_cnt[FEAT_AT_Lookback_LT],0.80);
   dots_threshold[FEAT_AT_Lookback_LT][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_AT_Lookback_LT,dots_roll_cnt[FEAT_AT_Lookback_LT],0.20);
   dots_threshold[FEAT_AT_Lookback_ST][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_AT_Lookback_ST,dots_roll_cnt[FEAT_AT_Lookback_ST],0.80);
   dots_threshold[FEAT_AT_Lookback_ST][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_AT_Lookback_ST,dots_roll_cnt[FEAT_AT_Lookback_ST],0.20);
   dots_threshold[FEAT_AT_Score_LT][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_AT_Score_LT,dots_roll_cnt[FEAT_AT_Score_LT],0.80);
   dots_threshold[FEAT_AT_Score_LT][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_AT_Score_LT,dots_roll_cnt[FEAT_AT_Score_LT],0.20);
   dots_threshold[FEAT_AT_Score_ST][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_AT_Score_ST,dots_roll_cnt[FEAT_AT_Score_ST],0.80);
   dots_threshold[FEAT_AT_Score_ST][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_AT_Score_ST,dots_roll_cnt[FEAT_AT_Score_ST],0.20);
   dots_threshold[FEAT_AT_Slope_LT][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_AT_Slope_LT,dots_roll_cnt[FEAT_AT_Slope_LT],0.80);
   dots_threshold[FEAT_AT_Slope_LT][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_AT_Slope_LT,dots_roll_cnt[FEAT_AT_Slope_LT],0.20);
   dots_threshold[FEAT_AT_Slope_ST][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_AT_Slope_ST,dots_roll_cnt[FEAT_AT_Slope_ST],0.80);
   dots_threshold[FEAT_AT_Slope_ST][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_AT_Slope_ST,dots_roll_cnt[FEAT_AT_Slope_ST],0.20);
   dots_threshold[FEAT_Bars_Since_Flip][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Bars_Since_Flip,dots_roll_cnt[FEAT_Bars_Since_Flip],0.80);
   dots_threshold[FEAT_Bars_Since_Flip][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Bars_Since_Flip,dots_roll_cnt[FEAT_Bars_Since_Flip],0.20);
   dots_threshold[FEAT_Slope_EMA_LT][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Slope_EMA_LT,dots_roll_cnt[FEAT_Slope_EMA_LT],0.80);
   dots_threshold[FEAT_Slope_EMA_LT][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Slope_EMA_LT,dots_roll_cnt[FEAT_Slope_EMA_LT],0.20);
   dots_threshold[FEAT_Slope_EMA_ST][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Slope_EMA_ST,dots_roll_cnt[FEAT_Slope_EMA_ST],0.80);
   dots_threshold[FEAT_Slope_EMA_ST][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Slope_EMA_ST,dots_roll_cnt[FEAT_Slope_EMA_ST],0.20);
   dots_threshold[FEAT_Slope_Accel_LT][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Slope_Accel_LT,dots_roll_cnt[FEAT_Slope_Accel_LT],0.80);
   dots_threshold[FEAT_Slope_Accel_LT][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Slope_Accel_LT,dots_roll_cnt[FEAT_Slope_Accel_LT],0.20);
   dots_threshold[FEAT_Slope_Accel_ST][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Slope_Accel_ST,dots_roll_cnt[FEAT_Slope_Accel_ST],0.80);
   dots_threshold[FEAT_Slope_Accel_ST][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Slope_Accel_ST,dots_roll_cnt[FEAT_Slope_Accel_ST],0.20);
   dots_threshold[FEAT_OBV_Macd][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_OBV_Macd,dots_roll_cnt[FEAT_OBV_Macd],0.80);
   dots_threshold[FEAT_OBV_Macd][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_OBV_Macd,dots_roll_cnt[FEAT_OBV_Macd],0.20);
   dots_threshold[FEAT_OBV_Velocity][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_OBV_Velocity,dots_roll_cnt[FEAT_OBV_Velocity],0.80);
   dots_threshold[FEAT_OBV_Velocity][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_OBV_Velocity,dots_roll_cnt[FEAT_OBV_Velocity],0.20);
   dots_threshold[FEAT_OBVf_DirStepCount][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_OBVf_DirStepCount,dots_roll_cnt[FEAT_OBVf_DirStepCount],0.80);
   dots_threshold[FEAT_OBVf_DirStepCount][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_OBVf_DirStepCount,dots_roll_cnt[FEAT_OBVf_DirStepCount],0.20);
   dots_threshold[FEAT_KAMA_Dist][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_KAMA_Dist,dots_roll_cnt[FEAT_KAMA_Dist],0.80);
   dots_threshold[FEAT_KAMA_Dist][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_KAMA_Dist,dots_roll_cnt[FEAT_KAMA_Dist],0.20);
   dots_threshold[FEAT_KAMA_Dist_ATR][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_KAMA_Dist_ATR,dots_roll_cnt[FEAT_KAMA_Dist_ATR],0.80);
   dots_threshold[FEAT_KAMA_Dist_ATR][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_KAMA_Dist_ATR,dots_roll_cnt[FEAT_KAMA_Dist_ATR],0.20);
   dots_threshold[FEAT_KAMA_Slope][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_KAMA_Slope,dots_roll_cnt[FEAT_KAMA_Slope],0.80);
   dots_threshold[FEAT_KAMA_Slope][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_KAMA_Slope,dots_roll_cnt[FEAT_KAMA_Slope],0.20);
   dots_threshold[FEAT_EMA_Oscillator][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_EMA_Oscillator,dots_roll_cnt[FEAT_EMA_Oscillator],0.80);
   dots_threshold[FEAT_EMA_Oscillator][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_EMA_Oscillator,dots_roll_cnt[FEAT_EMA_Oscillator],0.20);
   dots_threshold[FEAT_Harmonic_LLEMA][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Harmonic_LLEMA,dots_roll_cnt[FEAT_Harmonic_LLEMA],0.80);
   dots_threshold[FEAT_Harmonic_LLEMA][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Harmonic_LLEMA,dots_roll_cnt[FEAT_Harmonic_LLEMA],0.20);
   dots_threshold[FEAT_Sqz_Val][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Sqz_Val,dots_roll_cnt[FEAT_Sqz_Val],0.80);
   dots_threshold[FEAT_Sqz_Val][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Sqz_Val,dots_roll_cnt[FEAT_Sqz_Val],0.20);
   dots_threshold[FEAT_RangeOsc_Val][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_RangeOsc_Val,dots_roll_cnt[FEAT_RangeOsc_Val],0.80);
   dots_threshold[FEAT_RangeOsc_Val][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_RangeOsc_Val,dots_roll_cnt[FEAT_RangeOsc_Val],0.20);
   dots_threshold[FEAT_Volume_Avg_10][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Volume_Avg_10,dots_roll_cnt[FEAT_Volume_Avg_10],0.80);
   dots_threshold[FEAT_Volume_Avg_10][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Volume_Avg_10,dots_roll_cnt[FEAT_Volume_Avg_10],0.20);
   dots_threshold[FEAT_Volume_Ratio_10][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Volume_Ratio_10,dots_roll_cnt[FEAT_Volume_Ratio_10],0.80);
   dots_threshold[FEAT_Volume_Ratio_10][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Volume_Ratio_10,dots_roll_cnt[FEAT_Volume_Ratio_10],0.20);
   dots_threshold[FEAT_Momentum_Value][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Momentum_Value,dots_roll_cnt[FEAT_Momentum_Value],0.80);
   dots_threshold[FEAT_Momentum_Value][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Momentum_Value,dots_roll_cnt[FEAT_Momentum_Value],0.20);
   dots_threshold[FEAT_Efficiency_Ratio][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Efficiency_Ratio,dots_roll_cnt[FEAT_Efficiency_Ratio],0.80);
   dots_threshold[FEAT_Efficiency_Ratio][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Efficiency_Ratio,dots_roll_cnt[FEAT_Efficiency_Ratio],0.20);
   dots_threshold[FEAT_Dist_To_PoC_ATR][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Dist_To_PoC_ATR,dots_roll_cnt[FEAT_Dist_To_PoC_ATR],0.80);
   dots_threshold[FEAT_Dist_To_PoC_ATR][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Dist_To_PoC_ATR,dots_roll_cnt[FEAT_Dist_To_PoC_ATR],0.20);
   dots_threshold[FEAT_Micro_Amihud][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Micro_Amihud,dots_roll_cnt[FEAT_Micro_Amihud],0.80);
   dots_threshold[FEAT_Micro_Amihud][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Micro_Amihud,dots_roll_cnt[FEAT_Micro_Amihud],0.20);
   dots_threshold[FEAT_Micro_AutoCorr][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Micro_AutoCorr,dots_roll_cnt[FEAT_Micro_AutoCorr],0.80);
   dots_threshold[FEAT_Micro_AutoCorr][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Micro_AutoCorr,dots_roll_cnt[FEAT_Micro_AutoCorr],0.20);
   dots_threshold[FEAT_Micro_BarEntropy][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Micro_BarEntropy,dots_roll_cnt[FEAT_Micro_BarEntropy],0.80);
   dots_threshold[FEAT_Micro_BarEntropy][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Micro_BarEntropy,dots_roll_cnt[FEAT_Micro_BarEntropy],0.20);
   dots_threshold[FEAT_Micro_BarOverlap][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Micro_BarOverlap,dots_roll_cnt[FEAT_Micro_BarOverlap],0.80);
   dots_threshold[FEAT_Micro_BarOverlap][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Micro_BarOverlap,dots_roll_cnt[FEAT_Micro_BarOverlap],0.20);
   dots_threshold[FEAT_Micro_CSSpread][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Micro_CSSpread,dots_roll_cnt[FEAT_Micro_CSSpread],0.80);
   dots_threshold[FEAT_Micro_CSSpread][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Micro_CSSpread,dots_roll_cnt[FEAT_Micro_CSSpread],0.20);
   dots_threshold[FEAT_Micro_Entropy][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Micro_Entropy,dots_roll_cnt[FEAT_Micro_Entropy],0.80);
   dots_threshold[FEAT_Micro_Entropy][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Micro_Entropy,dots_roll_cnt[FEAT_Micro_Entropy],0.20);
   dots_threshold[FEAT_Micro_FailedBreak][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Micro_FailedBreak,dots_roll_cnt[FEAT_Micro_FailedBreak],0.80);
   dots_threshold[FEAT_Micro_FailedBreak][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Micro_FailedBreak,dots_roll_cnt[FEAT_Micro_FailedBreak],0.20);
   dots_threshold[FEAT_Micro_FractalDim][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Micro_FractalDim,dots_roll_cnt[FEAT_Micro_FractalDim],0.80);
   dots_threshold[FEAT_Micro_FractalDim][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Micro_FractalDim,dots_roll_cnt[FEAT_Micro_FractalDim],0.20);
   dots_threshold[FEAT_Micro_GarmanKlass][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Micro_GarmanKlass,dots_roll_cnt[FEAT_Micro_GarmanKlass],0.80);
   dots_threshold[FEAT_Micro_GarmanKlass][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Micro_GarmanKlass,dots_roll_cnt[FEAT_Micro_GarmanKlass],0.20);
   dots_threshold[FEAT_Micro_HLAsymmetry][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Micro_HLAsymmetry,dots_roll_cnt[FEAT_Micro_HLAsymmetry],0.80);
   dots_threshold[FEAT_Micro_HLAsymmetry][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Micro_HLAsymmetry,dots_roll_cnt[FEAT_Micro_HLAsymmetry],0.20);
   dots_threshold[FEAT_Micro_Hurst][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Micro_Hurst,dots_roll_cnt[FEAT_Micro_Hurst],0.80);
   dots_threshold[FEAT_Micro_Hurst][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Micro_Hurst,dots_roll_cnt[FEAT_Micro_Hurst],0.20);
   dots_threshold[FEAT_Micro_IBSP][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Micro_IBSP,dots_roll_cnt[FEAT_Micro_IBSP],0.80);
   dots_threshold[FEAT_Micro_IBSP][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Micro_IBSP,dots_roll_cnt[FEAT_Micro_IBSP],0.20);
   dots_threshold[FEAT_Micro_Lambda][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Micro_Lambda,dots_roll_cnt[FEAT_Micro_Lambda],0.80);
   dots_threshold[FEAT_Micro_Lambda][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Micro_Lambda,dots_roll_cnt[FEAT_Micro_Lambda],0.20);
   dots_threshold[FEAT_Micro_LogReturn][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Micro_LogReturn,dots_roll_cnt[FEAT_Micro_LogReturn],0.80);
   dots_threshold[FEAT_Micro_LogReturn][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Micro_LogReturn,dots_roll_cnt[FEAT_Micro_LogReturn],0.20);
   dots_threshold[FEAT_Micro_MicroGap][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Micro_MicroGap,dots_roll_cnt[FEAT_Micro_MicroGap],0.80);
   dots_threshold[FEAT_Micro_MicroGap][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Micro_MicroGap,dots_roll_cnt[FEAT_Micro_MicroGap],0.20);
   dots_threshold[FEAT_Micro_MomoTransfer][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Micro_MomoTransfer,dots_roll_cnt[FEAT_Micro_MomoTransfer],0.80);
   dots_threshold[FEAT_Micro_MomoTransfer][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Micro_MomoTransfer,dots_roll_cnt[FEAT_Micro_MomoTransfer],0.20);
   dots_threshold[FEAT_Micro_OrderFlowDelta][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Micro_OrderFlowDelta,dots_roll_cnt[FEAT_Micro_OrderFlowDelta],0.80);
   dots_threshold[FEAT_Micro_OrderFlowDelta][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Micro_OrderFlowDelta,dots_roll_cnt[FEAT_Micro_OrderFlowDelta],0.20);
   dots_threshold[FEAT_Micro_PriceAccel][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Micro_PriceAccel,dots_roll_cnt[FEAT_Micro_PriceAccel],0.80);
   dots_threshold[FEAT_Micro_PriceAccel][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Micro_PriceAccel,dots_roll_cnt[FEAT_Micro_PriceAccel],0.20);
   dots_threshold[FEAT_Micro_RangeAccel][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Micro_RangeAccel,dots_roll_cnt[FEAT_Micro_RangeAccel],0.80);
   dots_threshold[FEAT_Micro_RangeAccel][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Micro_RangeAccel,dots_roll_cnt[FEAT_Micro_RangeAccel],0.20);
   dots_threshold[FEAT_Micro_RangeVelocity][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Micro_RangeVelocity,dots_roll_cnt[FEAT_Micro_RangeVelocity],0.80);
   dots_threshold[FEAT_Micro_RangeVelocity][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Micro_RangeVelocity,dots_roll_cnt[FEAT_Micro_RangeVelocity],0.20);
   dots_threshold[FEAT_Micro_Rejection][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Micro_Rejection,dots_roll_cnt[FEAT_Micro_Rejection],0.80);
   dots_threshold[FEAT_Micro_Rejection][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Micro_Rejection,dots_roll_cnt[FEAT_Micro_Rejection],0.20);
   dots_threshold[FEAT_Micro_RollProxy][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Micro_RollProxy,dots_roll_cnt[FEAT_Micro_RollProxy],0.80);
   dots_threshold[FEAT_Micro_RollProxy][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Micro_RollProxy,dots_roll_cnt[FEAT_Micro_RollProxy],0.20);
   dots_threshold[FEAT_Micro_ThrustEff][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Micro_ThrustEff,dots_roll_cnt[FEAT_Micro_ThrustEff],0.80);
   dots_threshold[FEAT_Micro_ThrustEff][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Micro_ThrustEff,dots_roll_cnt[FEAT_Micro_ThrustEff],0.20);
   dots_threshold[FEAT_Micro_TickIntensity][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Micro_TickIntensity,dots_roll_cnt[FEAT_Micro_TickIntensity],0.80);
   dots_threshold[FEAT_Micro_TickIntensity][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Micro_TickIntensity,dots_roll_cnt[FEAT_Micro_TickIntensity],0.20);
   dots_threshold[FEAT_Micro_VPIN][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Micro_VPIN,dots_roll_cnt[FEAT_Micro_VPIN],0.80);
   dots_threshold[FEAT_Micro_VPIN][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Micro_VPIN,dots_roll_cnt[FEAT_Micro_VPIN],0.20);
   dots_threshold[FEAT_Micro_VolAccel][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Micro_VolAccel,dots_roll_cnt[FEAT_Micro_VolAccel],0.80);
   dots_threshold[FEAT_Micro_VolAccel][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Micro_VolAccel,dots_roll_cnt[FEAT_Micro_VolAccel],0.20);
   dots_threshold[FEAT_Micro_VolOfVol][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Micro_VolOfVol,dots_roll_cnt[FEAT_Micro_VolOfVol],0.80);
   dots_threshold[FEAT_Micro_VolOfVol][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Micro_VolOfVol,dots_roll_cnt[FEAT_Micro_VolOfVol],0.20);
   dots_threshold[FEAT_Micro_WickImbalance][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Micro_WickImbalance,dots_roll_cnt[FEAT_Micro_WickImbalance],0.80);
   dots_threshold[FEAT_Micro_WickImbalance][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Micro_WickImbalance,dots_roll_cnt[FEAT_Micro_WickImbalance],0.20);
   dots_threshold[FEAT_VWAP_Dist_ATR][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_VWAP_Dist_ATR,dots_roll_cnt[FEAT_VWAP_Dist_ATR],0.80);
   dots_threshold[FEAT_VWAP_Dist_ATR][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_VWAP_Dist_ATR,dots_roll_cnt[FEAT_VWAP_Dist_ATR],0.20);
   dots_threshold[FEAT_VAH_Dist_ATR][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_VAH_Dist_ATR,dots_roll_cnt[FEAT_VAH_Dist_ATR],0.80);
   dots_threshold[FEAT_VAH_Dist_ATR][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_VAH_Dist_ATR,dots_roll_cnt[FEAT_VAH_Dist_ATR],0.20);
   dots_threshold[FEAT_VAL_Dist_ATR][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_VAL_Dist_ATR,dots_roll_cnt[FEAT_VAL_Dist_ATR],0.80);
   dots_threshold[FEAT_VAL_Dist_ATR][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_VAL_Dist_ATR,dots_roll_cnt[FEAT_VAL_Dist_ATR],0.20);
   dots_threshold[FEAT_PrevDay_High_Dist_ATR][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_PrevDay_High_Dist_ATR,dots_roll_cnt[FEAT_PrevDay_High_Dist_ATR],0.80);
   dots_threshold[FEAT_PrevDay_High_Dist_ATR][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_PrevDay_High_Dist_ATR,dots_roll_cnt[FEAT_PrevDay_High_Dist_ATR],0.20);
   dots_threshold[FEAT_PrevDay_Low_Dist_ATR][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_PrevDay_Low_Dist_ATR,dots_roll_cnt[FEAT_PrevDay_Low_Dist_ATR],0.80);
   dots_threshold[FEAT_PrevDay_Low_Dist_ATR][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_PrevDay_Low_Dist_ATR,dots_roll_cnt[FEAT_PrevDay_Low_Dist_ATR],0.20);
   dots_threshold[FEAT_PrevDay_Close_Dist_ATR][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_PrevDay_Close_Dist_ATR,dots_roll_cnt[FEAT_PrevDay_Close_Dist_ATR],0.80);
   dots_threshold[FEAT_PrevDay_Close_Dist_ATR][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_PrevDay_Close_Dist_ATR,dots_roll_cnt[FEAT_PrevDay_Close_Dist_ATR],0.20);
   dots_threshold[FEAT_DailyOpen_Dist_ATR][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_DailyOpen_Dist_ATR,dots_roll_cnt[FEAT_DailyOpen_Dist_ATR],0.80);
   dots_threshold[FEAT_DailyOpen_Dist_ATR][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_DailyOpen_Dist_ATR,dots_roll_cnt[FEAT_DailyOpen_Dist_ATR],0.20);
   dots_threshold[FEAT_Round_100_Dist_ATR][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Round_100_Dist_ATR,dots_roll_cnt[FEAT_Round_100_Dist_ATR],0.80);
   dots_threshold[FEAT_Round_100_Dist_ATR][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Round_100_Dist_ATR,dots_roll_cnt[FEAT_Round_100_Dist_ATR],0.20);
   dots_threshold[FEAT_Round_500_Dist_ATR][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Round_500_Dist_ATR,dots_roll_cnt[FEAT_Round_500_Dist_ATR],0.80);
   dots_threshold[FEAT_Round_500_Dist_ATR][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Round_500_Dist_ATR,dots_roll_cnt[FEAT_Round_500_Dist_ATR],0.20);
   dots_threshold[FEAT_Round_1000_Dist_ATR][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Round_1000_Dist_ATR,dots_roll_cnt[FEAT_Round_1000_Dist_ATR],0.80);
   dots_threshold[FEAT_Round_1000_Dist_ATR][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Round_1000_Dist_ATR,dots_roll_cnt[FEAT_Round_1000_Dist_ATR],0.20);
   dots_threshold[FEAT_OR_High_Dist_ATR][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_OR_High_Dist_ATR,dots_roll_cnt[FEAT_OR_High_Dist_ATR],0.80);
   dots_threshold[FEAT_OR_High_Dist_ATR][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_OR_High_Dist_ATR,dots_roll_cnt[FEAT_OR_High_Dist_ATR],0.20);
   dots_threshold[FEAT_OR_Low_Dist_ATR][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_OR_Low_Dist_ATR,dots_roll_cnt[FEAT_OR_Low_Dist_ATR],0.80);
   dots_threshold[FEAT_OR_Low_Dist_ATR][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_OR_Low_Dist_ATR,dots_roll_cnt[FEAT_OR_Low_Dist_ATR],0.20);
   dots_threshold[FEAT_Session_High_Dist_ATR][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Session_High_Dist_ATR,dots_roll_cnt[FEAT_Session_High_Dist_ATR],0.80);
   dots_threshold[FEAT_Session_High_Dist_ATR][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Session_High_Dist_ATR,dots_roll_cnt[FEAT_Session_High_Dist_ATR],0.20);
   dots_threshold[FEAT_Session_Low_Dist_ATR][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Session_Low_Dist_ATR,dots_roll_cnt[FEAT_Session_Low_Dist_ATR],0.80);
   dots_threshold[FEAT_Session_Low_Dist_ATR][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Session_Low_Dist_ATR,dots_roll_cnt[FEAT_Session_Low_Dist_ATR],0.20);
   dots_threshold[FEAT_WeeklyOpen_Dist_ATR][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_WeeklyOpen_Dist_ATR,dots_roll_cnt[FEAT_WeeklyOpen_Dist_ATR],0.80);
   dots_threshold[FEAT_WeeklyOpen_Dist_ATR][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_WeeklyOpen_Dist_ATR,dots_roll_cnt[FEAT_WeeklyOpen_Dist_ATR],0.20);
   dots_threshold[FEAT_MultiDay_Slope][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_MultiDay_Slope,dots_roll_cnt[FEAT_MultiDay_Slope],0.80);
   dots_threshold[FEAT_MultiDay_Slope][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_MultiDay_Slope,dots_roll_cnt[FEAT_MultiDay_Slope],0.20);
   dots_threshold[FEAT_MultiDay_Position][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_MultiDay_Position,dots_roll_cnt[FEAT_MultiDay_Position],0.80);
   dots_threshold[FEAT_MultiDay_Position][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_MultiDay_Position,dots_roll_cnt[FEAT_MultiDay_Position],0.20);
   dots_threshold[FEAT_VWAP_Sigma_ATR][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_VWAP_Sigma_ATR,dots_roll_cnt[FEAT_VWAP_Sigma_ATR],0.80);
   dots_threshold[FEAT_VWAP_Sigma_ATR][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_VWAP_Sigma_ATR,dots_roll_cnt[FEAT_VWAP_Sigma_ATR],0.20);
   dots_threshold[FEAT_VA_Position][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_VA_Position,dots_roll_cnt[FEAT_VA_Position],0.80);
   dots_threshold[FEAT_VA_Position][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_VA_Position,dots_roll_cnt[FEAT_VA_Position],0.20);
   dots_threshold[FEAT_ADX_Value][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_ADX_Value,dots_roll_cnt[FEAT_ADX_Value],0.80);
   dots_threshold[FEAT_ADX_Value][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_ADX_Value,dots_roll_cnt[FEAT_ADX_Value],0.20);
   dots_threshold[FEAT_Body_Size][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Body_Size,dots_roll_cnt[FEAT_Body_Size],0.80);
   dots_threshold[FEAT_Body_Size][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Body_Size,dots_roll_cnt[FEAT_Body_Size],0.20);
   dots_threshold[FEAT_Upper_Wick][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Upper_Wick,dots_roll_cnt[FEAT_Upper_Wick],0.80);
   dots_threshold[FEAT_Upper_Wick][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Upper_Wick,dots_roll_cnt[FEAT_Upper_Wick],0.20);
   dots_threshold[FEAT_Lower_Wick][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Lower_Wick,dots_roll_cnt[FEAT_Lower_Wick],0.80);
   dots_threshold[FEAT_Lower_Wick][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Lower_Wick,dots_roll_cnt[FEAT_Lower_Wick],0.20);
   dots_threshold[FEAT_TChan_A15][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_TChan_A15,dots_roll_cnt[FEAT_TChan_A15],0.80);
   dots_threshold[FEAT_TChan_A15][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_TChan_A15,dots_roll_cnt[FEAT_TChan_A15],0.20);
   dots_threshold[FEAT_VWAP_Sigma][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_VWAP_Sigma,dots_roll_cnt[FEAT_VWAP_Sigma],0.80);
   dots_threshold[FEAT_VWAP_Sigma][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_VWAP_Sigma,dots_roll_cnt[FEAT_VWAP_Sigma],0.20);
   dots_threshold[FEAT_Volume][THR_HI]=DotsPercentileFromBuffer(dots_roll_buf_Volume,dots_roll_cnt[FEAT_Volume],0.80);
   dots_threshold[FEAT_Volume][THR_LO]=DotsPercentileFromBuffer(dots_roll_buf_Volume,dots_roll_cnt[FEAT_Volume],0.20);
   dots_roll_lastRefreshDay=TimeDay(TimeCurrent());
}
void DotsInsertRollingValue(int bufIdx,double &buf[],double val) {
   int sz=Dots_RollingBufferSize;
   buf[dots_roll_ptr[bufIdx]]=val;
   dots_roll_ptr[bufIdx]=(dots_roll_ptr[bufIdx]+1)%sz;
   if(dots_roll_cnt[bufIdx]<sz) dots_roll_cnt[bufIdx]++;
}
void SeedDotsRollingBuffers() {
   int maxBars=Bars;
   int limADX=ArraySize(hist_ADXValue);
   int limVol=ArraySize(hist_VolumeValue);
   int eligible=0;
   int seedDay=-1;
   int scanLimit=(int)MathMin((double)(maxBars-1),(double)Dots_InitBars);
   for(int i=scanLimit; i>=1; i--) {
      if(i<limADX&&i<limVol&&hist_ADXValue[i]>=15.0&&hist_VolumeValue[i]>50.0) {
         DotsInsertRollingValue(FEAT_ATR_1M,dots_roll_buf_ATR_1M,DotsGetFeatureValue(FEAT_ATR_1M,i));
         DotsInsertRollingValue(FEAT_Bar_Range,dots_roll_buf_Bar_Range,DotsGetFeatureValue(FEAT_Bar_Range,i));
         DotsInsertRollingValue(FEAT_D2D_ATR,dots_roll_buf_D2D_ATR,DotsGetFeatureValue(FEAT_D2D_ATR,i));
         DotsInsertRollingValue(FEAT_D2D_ATR_MA,dots_roll_buf_D2D_ATR_MA,DotsGetFeatureValue(FEAT_D2D_ATR_MA,i));
         DotsInsertRollingValue(FEAT_D2D_Dn_Count,dots_roll_buf_D2D_Dn_Count,DotsGetFeatureValue(FEAT_D2D_Dn_Count,i));
         DotsInsertRollingValue(FEAT_D2D_Dynamic_Sensitivity,dots_roll_buf_D2D_Dynamic_Sensitivity,DotsGetFeatureValue(FEAT_D2D_Dynamic_Sensitivity,i));
         DotsInsertRollingValue(FEAT_D2D_Persist,dots_roll_buf_D2D_Persist,DotsGetFeatureValue(FEAT_D2D_Persist,i));
         DotsInsertRollingValue(FEAT_D2D_Up_Count,dots_roll_buf_D2D_Up_Count,DotsGetFeatureValue(FEAT_D2D_Up_Count,i));
         DotsInsertRollingValue(FEAT_AT_Lookback_LT,dots_roll_buf_AT_Lookback_LT,DotsGetFeatureValue(FEAT_AT_Lookback_LT,i));
         DotsInsertRollingValue(FEAT_AT_Lookback_ST,dots_roll_buf_AT_Lookback_ST,DotsGetFeatureValue(FEAT_AT_Lookback_ST,i));
         DotsInsertRollingValue(FEAT_AT_Score_LT,dots_roll_buf_AT_Score_LT,DotsGetFeatureValue(FEAT_AT_Score_LT,i));
         DotsInsertRollingValue(FEAT_AT_Score_ST,dots_roll_buf_AT_Score_ST,DotsGetFeatureValue(FEAT_AT_Score_ST,i));
         DotsInsertRollingValue(FEAT_AT_Slope_LT,dots_roll_buf_AT_Slope_LT,DotsGetFeatureValue(FEAT_AT_Slope_LT,i));
         DotsInsertRollingValue(FEAT_AT_Slope_ST,dots_roll_buf_AT_Slope_ST,DotsGetFeatureValue(FEAT_AT_Slope_ST,i));
         DotsInsertRollingValue(FEAT_Bars_Since_Flip,dots_roll_buf_Bars_Since_Flip,DotsGetFeatureValue(FEAT_Bars_Since_Flip,i));
         DotsInsertRollingValue(FEAT_Slope_EMA_LT,dots_roll_buf_Slope_EMA_LT,DotsGetFeatureValue(FEAT_Slope_EMA_LT,i));
         DotsInsertRollingValue(FEAT_Slope_EMA_ST,dots_roll_buf_Slope_EMA_ST,DotsGetFeatureValue(FEAT_Slope_EMA_ST,i));
         DotsInsertRollingValue(FEAT_Slope_Accel_LT,dots_roll_buf_Slope_Accel_LT,DotsGetFeatureValue(FEAT_Slope_Accel_LT,i));
         DotsInsertRollingValue(FEAT_Slope_Accel_ST,dots_roll_buf_Slope_Accel_ST,DotsGetFeatureValue(FEAT_Slope_Accel_ST,i));
         DotsInsertRollingValue(FEAT_OBV_Macd,dots_roll_buf_OBV_Macd,DotsGetFeatureValue(FEAT_OBV_Macd,i));
         DotsInsertRollingValue(FEAT_OBV_Velocity,dots_roll_buf_OBV_Velocity,DotsGetFeatureValue(FEAT_OBV_Velocity,i));
         DotsInsertRollingValue(FEAT_OBVf_DirStepCount,dots_roll_buf_OBVf_DirStepCount,DotsGetFeatureValue(FEAT_OBVf_DirStepCount,i));
         DotsInsertRollingValue(FEAT_KAMA_Dist,dots_roll_buf_KAMA_Dist,DotsGetFeatureValue(FEAT_KAMA_Dist,i));
         DotsInsertRollingValue(FEAT_KAMA_Dist_ATR,dots_roll_buf_KAMA_Dist_ATR,DotsGetFeatureValue(FEAT_KAMA_Dist_ATR,i));
         DotsInsertRollingValue(FEAT_KAMA_Slope,dots_roll_buf_KAMA_Slope,DotsGetFeatureValue(FEAT_KAMA_Slope,i));
         DotsInsertRollingValue(FEAT_EMA_Oscillator,dots_roll_buf_EMA_Oscillator,DotsGetFeatureValue(FEAT_EMA_Oscillator,i));
         DotsInsertRollingValue(FEAT_Harmonic_LLEMA,dots_roll_buf_Harmonic_LLEMA,DotsGetFeatureValue(FEAT_Harmonic_LLEMA,i));
         DotsInsertRollingValue(FEAT_Sqz_Val,dots_roll_buf_Sqz_Val,DotsGetFeatureValue(FEAT_Sqz_Val,i));
         DotsInsertRollingValue(FEAT_RangeOsc_Val,dots_roll_buf_RangeOsc_Val,DotsGetFeatureValue(FEAT_RangeOsc_Val,i));
         DotsInsertRollingValue(FEAT_Volume_Avg_10,dots_roll_buf_Volume_Avg_10,DotsGetFeatureValue(FEAT_Volume_Avg_10,i));
         DotsInsertRollingValue(FEAT_Volume_Ratio_10,dots_roll_buf_Volume_Ratio_10,DotsGetFeatureValue(FEAT_Volume_Ratio_10,i));
         DotsInsertRollingValue(FEAT_Momentum_Value,dots_roll_buf_Momentum_Value,DotsGetFeatureValue(FEAT_Momentum_Value,i));
         DotsInsertRollingValue(FEAT_Efficiency_Ratio,dots_roll_buf_Efficiency_Ratio,DotsGetFeatureValue(FEAT_Efficiency_Ratio,i));
         DotsInsertRollingValue(FEAT_Dist_To_PoC_ATR,dots_roll_buf_Dist_To_PoC_ATR,DotsGetFeatureValue(FEAT_Dist_To_PoC_ATR,i));
         DotsInsertRollingValue(FEAT_Micro_Amihud,dots_roll_buf_Micro_Amihud,DotsGetFeatureValue(FEAT_Micro_Amihud,i));
         DotsInsertRollingValue(FEAT_Micro_AutoCorr,dots_roll_buf_Micro_AutoCorr,DotsGetFeatureValue(FEAT_Micro_AutoCorr,i));
         DotsInsertRollingValue(FEAT_Micro_BarEntropy,dots_roll_buf_Micro_BarEntropy,DotsGetFeatureValue(FEAT_Micro_BarEntropy,i));
         DotsInsertRollingValue(FEAT_Micro_BarOverlap,dots_roll_buf_Micro_BarOverlap,DotsGetFeatureValue(FEAT_Micro_BarOverlap,i));
         DotsInsertRollingValue(FEAT_Micro_CSSpread,dots_roll_buf_Micro_CSSpread,DotsGetFeatureValue(FEAT_Micro_CSSpread,i));
         DotsInsertRollingValue(FEAT_Micro_Entropy,dots_roll_buf_Micro_Entropy,DotsGetFeatureValue(FEAT_Micro_Entropy,i));
         DotsInsertRollingValue(FEAT_Micro_FailedBreak,dots_roll_buf_Micro_FailedBreak,DotsGetFeatureValue(FEAT_Micro_FailedBreak,i));
         DotsInsertRollingValue(FEAT_Micro_FractalDim,dots_roll_buf_Micro_FractalDim,DotsGetFeatureValue(FEAT_Micro_FractalDim,i));
         DotsInsertRollingValue(FEAT_Micro_GarmanKlass,dots_roll_buf_Micro_GarmanKlass,DotsGetFeatureValue(FEAT_Micro_GarmanKlass,i));
         DotsInsertRollingValue(FEAT_Micro_HLAsymmetry,dots_roll_buf_Micro_HLAsymmetry,DotsGetFeatureValue(FEAT_Micro_HLAsymmetry,i));
         DotsInsertRollingValue(FEAT_Micro_Hurst,dots_roll_buf_Micro_Hurst,DotsGetFeatureValue(FEAT_Micro_Hurst,i));
         DotsInsertRollingValue(FEAT_Micro_IBSP,dots_roll_buf_Micro_IBSP,DotsGetFeatureValue(FEAT_Micro_IBSP,i));
         DotsInsertRollingValue(FEAT_Micro_Lambda,dots_roll_buf_Micro_Lambda,DotsGetFeatureValue(FEAT_Micro_Lambda,i));
         DotsInsertRollingValue(FEAT_Micro_LogReturn,dots_roll_buf_Micro_LogReturn,DotsGetFeatureValue(FEAT_Micro_LogReturn,i));
         DotsInsertRollingValue(FEAT_Micro_MicroGap,dots_roll_buf_Micro_MicroGap,DotsGetFeatureValue(FEAT_Micro_MicroGap,i));
         DotsInsertRollingValue(FEAT_Micro_MomoTransfer,dots_roll_buf_Micro_MomoTransfer,DotsGetFeatureValue(FEAT_Micro_MomoTransfer,i));
         DotsInsertRollingValue(FEAT_Micro_OrderFlowDelta,dots_roll_buf_Micro_OrderFlowDelta,DotsGetFeatureValue(FEAT_Micro_OrderFlowDelta,i));
         DotsInsertRollingValue(FEAT_Micro_PriceAccel,dots_roll_buf_Micro_PriceAccel,DotsGetFeatureValue(FEAT_Micro_PriceAccel,i));
         DotsInsertRollingValue(FEAT_Micro_RangeAccel,dots_roll_buf_Micro_RangeAccel,DotsGetFeatureValue(FEAT_Micro_RangeAccel,i));
         DotsInsertRollingValue(FEAT_Micro_RangeVelocity,dots_roll_buf_Micro_RangeVelocity,DotsGetFeatureValue(FEAT_Micro_RangeVelocity,i));
         DotsInsertRollingValue(FEAT_Micro_Rejection,dots_roll_buf_Micro_Rejection,DotsGetFeatureValue(FEAT_Micro_Rejection,i));
         DotsInsertRollingValue(FEAT_Micro_RollProxy,dots_roll_buf_Micro_RollProxy,DotsGetFeatureValue(FEAT_Micro_RollProxy,i));
         DotsInsertRollingValue(FEAT_Micro_ThrustEff,dots_roll_buf_Micro_ThrustEff,DotsGetFeatureValue(FEAT_Micro_ThrustEff,i));
         DotsInsertRollingValue(FEAT_Micro_TickIntensity,dots_roll_buf_Micro_TickIntensity,DotsGetFeatureValue(FEAT_Micro_TickIntensity,i));
         DotsInsertRollingValue(FEAT_Micro_VPIN,dots_roll_buf_Micro_VPIN,DotsGetFeatureValue(FEAT_Micro_VPIN,i));
         DotsInsertRollingValue(FEAT_Micro_VolAccel,dots_roll_buf_Micro_VolAccel,DotsGetFeatureValue(FEAT_Micro_VolAccel,i));
         DotsInsertRollingValue(FEAT_Micro_VolOfVol,dots_roll_buf_Micro_VolOfVol,DotsGetFeatureValue(FEAT_Micro_VolOfVol,i));
         DotsInsertRollingValue(FEAT_Micro_WickImbalance,dots_roll_buf_Micro_WickImbalance,DotsGetFeatureValue(FEAT_Micro_WickImbalance,i));
         DotsInsertRollingValue(FEAT_VWAP_Dist_ATR,dots_roll_buf_VWAP_Dist_ATR,DotsGetFeatureValue(FEAT_VWAP_Dist_ATR,i));
         DotsInsertRollingValue(FEAT_VAH_Dist_ATR,dots_roll_buf_VAH_Dist_ATR,DotsGetFeatureValue(FEAT_VAH_Dist_ATR,i));
         DotsInsertRollingValue(FEAT_VAL_Dist_ATR,dots_roll_buf_VAL_Dist_ATR,DotsGetFeatureValue(FEAT_VAL_Dist_ATR,i));
         DotsInsertRollingValue(FEAT_PrevDay_High_Dist_ATR,dots_roll_buf_PrevDay_High_Dist_ATR,DotsGetFeatureValue(FEAT_PrevDay_High_Dist_ATR,i));
         DotsInsertRollingValue(FEAT_PrevDay_Low_Dist_ATR,dots_roll_buf_PrevDay_Low_Dist_ATR,DotsGetFeatureValue(FEAT_PrevDay_Low_Dist_ATR,i));
         DotsInsertRollingValue(FEAT_PrevDay_Close_Dist_ATR,dots_roll_buf_PrevDay_Close_Dist_ATR,DotsGetFeatureValue(FEAT_PrevDay_Close_Dist_ATR,i));
         DotsInsertRollingValue(FEAT_DailyOpen_Dist_ATR,dots_roll_buf_DailyOpen_Dist_ATR,DotsGetFeatureValue(FEAT_DailyOpen_Dist_ATR,i));
         DotsInsertRollingValue(FEAT_Round_100_Dist_ATR,dots_roll_buf_Round_100_Dist_ATR,DotsGetFeatureValue(FEAT_Round_100_Dist_ATR,i));
         DotsInsertRollingValue(FEAT_Round_500_Dist_ATR,dots_roll_buf_Round_500_Dist_ATR,DotsGetFeatureValue(FEAT_Round_500_Dist_ATR,i));
         DotsInsertRollingValue(FEAT_Round_1000_Dist_ATR,dots_roll_buf_Round_1000_Dist_ATR,DotsGetFeatureValue(FEAT_Round_1000_Dist_ATR,i));
         DotsInsertRollingValue(FEAT_OR_High_Dist_ATR,dots_roll_buf_OR_High_Dist_ATR,DotsGetFeatureValue(FEAT_OR_High_Dist_ATR,i));
         DotsInsertRollingValue(FEAT_OR_Low_Dist_ATR,dots_roll_buf_OR_Low_Dist_ATR,DotsGetFeatureValue(FEAT_OR_Low_Dist_ATR,i));
         DotsInsertRollingValue(FEAT_Session_High_Dist_ATR,dots_roll_buf_Session_High_Dist_ATR,DotsGetFeatureValue(FEAT_Session_High_Dist_ATR,i));
         DotsInsertRollingValue(FEAT_Session_Low_Dist_ATR,dots_roll_buf_Session_Low_Dist_ATR,DotsGetFeatureValue(FEAT_Session_Low_Dist_ATR,i));
         DotsInsertRollingValue(FEAT_WeeklyOpen_Dist_ATR,dots_roll_buf_WeeklyOpen_Dist_ATR,DotsGetFeatureValue(FEAT_WeeklyOpen_Dist_ATR,i));
         DotsInsertRollingValue(FEAT_MultiDay_Slope,dots_roll_buf_MultiDay_Slope,DotsGetFeatureValue(FEAT_MultiDay_Slope,i));
         DotsInsertRollingValue(FEAT_MultiDay_Position,dots_roll_buf_MultiDay_Position,DotsGetFeatureValue(FEAT_MultiDay_Position,i));
         DotsInsertRollingValue(FEAT_VWAP_Sigma_ATR,dots_roll_buf_VWAP_Sigma_ATR,DotsGetFeatureValue(FEAT_VWAP_Sigma_ATR,i));
         DotsInsertRollingValue(FEAT_VA_Position,dots_roll_buf_VA_Position,DotsGetFeatureValue(FEAT_VA_Position,i));
         DotsInsertRollingValue(FEAT_ADX_Value,dots_roll_buf_ADX_Value,DotsGetFeatureValue(FEAT_ADX_Value,i));
         DotsInsertRollingValue(FEAT_Body_Size,dots_roll_buf_Body_Size,DotsGetFeatureValue(FEAT_Body_Size,i));
         DotsInsertRollingValue(FEAT_Upper_Wick,dots_roll_buf_Upper_Wick,DotsGetFeatureValue(FEAT_Upper_Wick,i));
         DotsInsertRollingValue(FEAT_Lower_Wick,dots_roll_buf_Lower_Wick,DotsGetFeatureValue(FEAT_Lower_Wick,i));
         DotsInsertRollingValue(FEAT_TChan_A15,dots_roll_buf_TChan_A15,DotsGetFeatureValue(FEAT_TChan_A15,i));
         DotsInsertRollingValue(FEAT_VWAP_Sigma,dots_roll_buf_VWAP_Sigma,DotsGetFeatureValue(FEAT_VWAP_Sigma,i));
         DotsInsertRollingValue(FEAT_Volume,dots_roll_buf_Volume,DotsGetFeatureValue(FEAT_Volume,i));
         eligible++;
      }
      int barDay=TimeDay(Time[i]);
      if(barDay!=seedDay) { DotsRefreshRollingPercentiles(); seedDay=barDay; }
   }
   if(eligible<100) {
      Print("DOTS| Rolling buffer seed: insufficient data (",eligible," eligible bars of ",scanLimit," scanned)");
      return;
   }
   Print("DOTS| Rolling buffers seeded: ",eligible," eligible bars");
   Print("DOTS|   AT_Slope_LT  p80=",DoubleToString(dots_threshold[FEAT_AT_Slope_LT][THR_HI],10));
   Print("DOTS|   Slope_EMA_LT p80=",DoubleToString(dots_threshold[FEAT_Slope_EMA_LT][THR_HI],10));
   Print("DOTS|   OBV_Macd     p80=",DoubleToString(dots_threshold[FEAT_OBV_Macd][THR_HI],6)," p20=",DoubleToString(dots_threshold[FEAT_OBV_Macd][THR_LO],6));
   Print("DOTS|   Harm_LLEMA   p80=",DoubleToString(dots_threshold[FEAT_Harmonic_LLEMA][THR_HI],6));
   Print("DOTS|   RangeOsc_Val p20=",DoubleToString(dots_threshold[FEAT_RangeOsc_Val][THR_LO],6));
}
void ExportDotsThresholdSnapshots() {
   int fh=FileOpen("equiDOT_thresholds_"+Symbol()+"_"+IntegerToString(Period())+".csv",FILE_WRITE|FILE_TXT);
   if(fh==INVALID_HANDLE) { Print("DOTS| Threshold export: cannot open file"); return; }
   string header="Time,"+
      "ATR_1M_HI,ATR_1M_LO,Bar_Range_HI,Bar_Range_LO,D2D_ATR_HI,D2D_ATR_LO,D2D_ATR_MA_HI,D2D_ATR_MA_LO,"+
      "D2D_Dn_Count_HI,D2D_Dn_Count_LO,D2D_Dynamic_Sensitivity_HI,D2D_Dynamic_Sensitivity_LO,D2D_Persist_HI,D2D_Persist_LO,D2D_Up_Count_HI,D2D_Up_Count_LO,"+
      "AT_Lookback_LT_HI,AT_Lookback_LT_LO,AT_Lookback_ST_HI,AT_Lookback_ST_LO,AT_Score_LT_HI,AT_Score_LT_LO,AT_Score_ST_HI,AT_Score_ST_LO,"+
      "AT_Slope_LT_HI,AT_Slope_LT_LO,AT_Slope_ST_HI,AT_Slope_ST_LO,Bars_Since_Flip_HI,Bars_Since_Flip_LO,Slope_EMA_LT_HI,Slope_EMA_LT_LO,"+
      "Slope_EMA_ST_HI,Slope_EMA_ST_LO,Slope_Accel_LT_HI,Slope_Accel_LT_LO,Slope_Accel_ST_HI,Slope_Accel_ST_LO,OBV_Macd_HI,OBV_Macd_LO,"+
      "OBV_Velocity_HI,OBV_Velocity_LO,OBVf_DirStepCount_HI,OBVf_DirStepCount_LO,KAMA_Dist_HI,KAMA_Dist_LO,KAMA_Dist_ATR_HI,KAMA_Dist_ATR_LO,"+
      "KAMA_Slope_HI,KAMA_Slope_LO,EMA_Oscillator_HI,EMA_Oscillator_LO,Harmonic_LLEMA_HI,Harmonic_LLEMA_LO,Sqz_Val_HI,Sqz_Val_LO,"+
      "RangeOsc_Val_HI,RangeOsc_Val_LO,Volume_Avg_10_HI,Volume_Avg_10_LO,Volume_Ratio_10_HI,Volume_Ratio_10_LO,Momentum_Value_HI,Momentum_Value_LO,"+
      "Efficiency_Ratio_HI,Efficiency_Ratio_LO,Dist_To_PoC_ATR_HI,Dist_To_PoC_ATR_LO,Micro_Amihud_HI,Micro_Amihud_LO,Micro_AutoCorr_HI,Micro_AutoCorr_LO,"+
      "Micro_BarEntropy_HI,Micro_BarEntropy_LO,Micro_BarOverlap_HI,Micro_BarOverlap_LO,Micro_CSSpread_HI,Micro_CSSpread_LO,Micro_Entropy_HI,Micro_Entropy_LO,"+
      "Micro_FailedBreak_HI,Micro_FailedBreak_LO,Micro_FractalDim_HI,Micro_FractalDim_LO,Micro_GarmanKlass_HI,Micro_GarmanKlass_LO,Micro_HLAsymmetry_HI,Micro_HLAsymmetry_LO,"+
      "Micro_Hurst_HI,Micro_Hurst_LO,Micro_IBSP_HI,Micro_IBSP_LO,Micro_Lambda_HI,Micro_Lambda_LO,Micro_LogReturn_HI,Micro_LogReturn_LO,"+
      "Micro_MicroGap_HI,Micro_MicroGap_LO,Micro_MomoTransfer_HI,Micro_MomoTransfer_LO,Micro_OrderFlowDelta_HI,Micro_OrderFlowDelta_LO,Micro_PriceAccel_HI,Micro_PriceAccel_LO,"+
      "Micro_RangeAccel_HI,Micro_RangeAccel_LO,Micro_RangeVelocity_HI,Micro_RangeVelocity_LO,Micro_Rejection_HI,Micro_Rejection_LO,Micro_RollProxy_HI,Micro_RollProxy_LO,"+
      "Micro_ThrustEff_HI,Micro_ThrustEff_LO,Micro_TickIntensity_HI,Micro_TickIntensity_LO,Micro_VPIN_HI,Micro_VPIN_LO,Micro_VolAccel_HI,Micro_VolAccel_LO,"+
      "Micro_VolOfVol_HI,Micro_VolOfVol_LO,Micro_WickImbalance_HI,Micro_WickImbalance_LO,VWAP_Dist_ATR_HI,VWAP_Dist_ATR_LO,VAH_Dist_ATR_HI,VAH_Dist_ATR_LO,"+
      "VAL_Dist_ATR_HI,VAL_Dist_ATR_LO,PrevDay_High_Dist_ATR_HI,PrevDay_High_Dist_ATR_LO,PrevDay_Low_Dist_ATR_HI,PrevDay_Low_Dist_ATR_LO,PrevDay_Close_Dist_ATR_HI,PrevDay_Close_Dist_ATR_LO,"+
      "DailyOpen_Dist_ATR_HI,DailyOpen_Dist_ATR_LO,Round_100_Dist_ATR_HI,Round_100_Dist_ATR_LO,Round_500_Dist_ATR_HI,Round_500_Dist_ATR_LO,Round_1000_Dist_ATR_HI,Round_1000_Dist_ATR_LO,"+
      "OR_High_Dist_ATR_HI,OR_High_Dist_ATR_LO,OR_Low_Dist_ATR_HI,OR_Low_Dist_ATR_LO,Session_High_Dist_ATR_HI,Session_High_Dist_ATR_LO,Session_Low_Dist_ATR_HI,Session_Low_Dist_ATR_LO,"+
      "WeeklyOpen_Dist_ATR_HI,WeeklyOpen_Dist_ATR_LO,MultiDay_Slope_HI,MultiDay_Slope_LO,MultiDay_Position_HI,MultiDay_Position_LO,VWAP_Sigma_ATR_HI,VWAP_Sigma_ATR_LO,"+
      "VA_Position_HI,VA_Position_LO,"+
      "ADX_Value_HI,ADX_Value_LO,Body_Size_HI,Body_Size_LO,Upper_Wick_HI,Upper_Wick_LO,Lower_Wick_HI,Lower_Wick_LO,TChan_A15_HI,TChan_A15_LO,VWAP_Sigma_HI,VWAP_Sigma_LO,Volume_HI,Volume_LO";
   FileWriteString(fh,header+"\r\n");
   ArrayInitialize(dots_roll_ptr,0);
   ArrayInitialize(dots_roll_cnt,0);
   int limADX=ArraySize(hist_ADXValue);
   int limVol=ArraySize(hist_VolumeValue);
   int seedDay=-1;
   int scanLimit=(int)MathMin((double)(Bars-1),(double)Dots_InitBars);
   for(int i=scanLimit; i>=1; i--) {
      if(i<limADX&&i<limVol&&hist_ADXValue[i]>=15.0&&hist_VolumeValue[i]>50.0) {
         DotsInsertRollingValue(FEAT_ATR_1M,dots_roll_buf_ATR_1M,DotsGetFeatureValue(FEAT_ATR_1M,i));
         DotsInsertRollingValue(FEAT_Bar_Range,dots_roll_buf_Bar_Range,DotsGetFeatureValue(FEAT_Bar_Range,i));
         DotsInsertRollingValue(FEAT_D2D_ATR,dots_roll_buf_D2D_ATR,DotsGetFeatureValue(FEAT_D2D_ATR,i));
         DotsInsertRollingValue(FEAT_D2D_ATR_MA,dots_roll_buf_D2D_ATR_MA,DotsGetFeatureValue(FEAT_D2D_ATR_MA,i));
         DotsInsertRollingValue(FEAT_D2D_Dn_Count,dots_roll_buf_D2D_Dn_Count,DotsGetFeatureValue(FEAT_D2D_Dn_Count,i));
         DotsInsertRollingValue(FEAT_D2D_Dynamic_Sensitivity,dots_roll_buf_D2D_Dynamic_Sensitivity,DotsGetFeatureValue(FEAT_D2D_Dynamic_Sensitivity,i));
         DotsInsertRollingValue(FEAT_D2D_Persist,dots_roll_buf_D2D_Persist,DotsGetFeatureValue(FEAT_D2D_Persist,i));
         DotsInsertRollingValue(FEAT_D2D_Up_Count,dots_roll_buf_D2D_Up_Count,DotsGetFeatureValue(FEAT_D2D_Up_Count,i));
         DotsInsertRollingValue(FEAT_AT_Lookback_LT,dots_roll_buf_AT_Lookback_LT,DotsGetFeatureValue(FEAT_AT_Lookback_LT,i));
         DotsInsertRollingValue(FEAT_AT_Lookback_ST,dots_roll_buf_AT_Lookback_ST,DotsGetFeatureValue(FEAT_AT_Lookback_ST,i));
         DotsInsertRollingValue(FEAT_AT_Score_LT,dots_roll_buf_AT_Score_LT,DotsGetFeatureValue(FEAT_AT_Score_LT,i));
         DotsInsertRollingValue(FEAT_AT_Score_ST,dots_roll_buf_AT_Score_ST,DotsGetFeatureValue(FEAT_AT_Score_ST,i));
         DotsInsertRollingValue(FEAT_AT_Slope_LT,dots_roll_buf_AT_Slope_LT,DotsGetFeatureValue(FEAT_AT_Slope_LT,i));
         DotsInsertRollingValue(FEAT_AT_Slope_ST,dots_roll_buf_AT_Slope_ST,DotsGetFeatureValue(FEAT_AT_Slope_ST,i));
         DotsInsertRollingValue(FEAT_Bars_Since_Flip,dots_roll_buf_Bars_Since_Flip,DotsGetFeatureValue(FEAT_Bars_Since_Flip,i));
         DotsInsertRollingValue(FEAT_Slope_EMA_LT,dots_roll_buf_Slope_EMA_LT,DotsGetFeatureValue(FEAT_Slope_EMA_LT,i));
         DotsInsertRollingValue(FEAT_Slope_EMA_ST,dots_roll_buf_Slope_EMA_ST,DotsGetFeatureValue(FEAT_Slope_EMA_ST,i));
         DotsInsertRollingValue(FEAT_Slope_Accel_LT,dots_roll_buf_Slope_Accel_LT,DotsGetFeatureValue(FEAT_Slope_Accel_LT,i));
         DotsInsertRollingValue(FEAT_Slope_Accel_ST,dots_roll_buf_Slope_Accel_ST,DotsGetFeatureValue(FEAT_Slope_Accel_ST,i));
         DotsInsertRollingValue(FEAT_OBV_Macd,dots_roll_buf_OBV_Macd,DotsGetFeatureValue(FEAT_OBV_Macd,i));
         DotsInsertRollingValue(FEAT_OBV_Velocity,dots_roll_buf_OBV_Velocity,DotsGetFeatureValue(FEAT_OBV_Velocity,i));
         DotsInsertRollingValue(FEAT_OBVf_DirStepCount,dots_roll_buf_OBVf_DirStepCount,DotsGetFeatureValue(FEAT_OBVf_DirStepCount,i));
         DotsInsertRollingValue(FEAT_KAMA_Dist,dots_roll_buf_KAMA_Dist,DotsGetFeatureValue(FEAT_KAMA_Dist,i));
         DotsInsertRollingValue(FEAT_KAMA_Dist_ATR,dots_roll_buf_KAMA_Dist_ATR,DotsGetFeatureValue(FEAT_KAMA_Dist_ATR,i));
         DotsInsertRollingValue(FEAT_KAMA_Slope,dots_roll_buf_KAMA_Slope,DotsGetFeatureValue(FEAT_KAMA_Slope,i));
         DotsInsertRollingValue(FEAT_EMA_Oscillator,dots_roll_buf_EMA_Oscillator,DotsGetFeatureValue(FEAT_EMA_Oscillator,i));
         DotsInsertRollingValue(FEAT_Harmonic_LLEMA,dots_roll_buf_Harmonic_LLEMA,DotsGetFeatureValue(FEAT_Harmonic_LLEMA,i));
         DotsInsertRollingValue(FEAT_Sqz_Val,dots_roll_buf_Sqz_Val,DotsGetFeatureValue(FEAT_Sqz_Val,i));
         DotsInsertRollingValue(FEAT_RangeOsc_Val,dots_roll_buf_RangeOsc_Val,DotsGetFeatureValue(FEAT_RangeOsc_Val,i));
         DotsInsertRollingValue(FEAT_Volume_Avg_10,dots_roll_buf_Volume_Avg_10,DotsGetFeatureValue(FEAT_Volume_Avg_10,i));
         DotsInsertRollingValue(FEAT_Volume_Ratio_10,dots_roll_buf_Volume_Ratio_10,DotsGetFeatureValue(FEAT_Volume_Ratio_10,i));
         DotsInsertRollingValue(FEAT_Momentum_Value,dots_roll_buf_Momentum_Value,DotsGetFeatureValue(FEAT_Momentum_Value,i));
         DotsInsertRollingValue(FEAT_Efficiency_Ratio,dots_roll_buf_Efficiency_Ratio,DotsGetFeatureValue(FEAT_Efficiency_Ratio,i));
         DotsInsertRollingValue(FEAT_Dist_To_PoC_ATR,dots_roll_buf_Dist_To_PoC_ATR,DotsGetFeatureValue(FEAT_Dist_To_PoC_ATR,i));
         DotsInsertRollingValue(FEAT_Micro_Amihud,dots_roll_buf_Micro_Amihud,DotsGetFeatureValue(FEAT_Micro_Amihud,i));
         DotsInsertRollingValue(FEAT_Micro_AutoCorr,dots_roll_buf_Micro_AutoCorr,DotsGetFeatureValue(FEAT_Micro_AutoCorr,i));
         DotsInsertRollingValue(FEAT_Micro_BarEntropy,dots_roll_buf_Micro_BarEntropy,DotsGetFeatureValue(FEAT_Micro_BarEntropy,i));
         DotsInsertRollingValue(FEAT_Micro_BarOverlap,dots_roll_buf_Micro_BarOverlap,DotsGetFeatureValue(FEAT_Micro_BarOverlap,i));
         DotsInsertRollingValue(FEAT_Micro_CSSpread,dots_roll_buf_Micro_CSSpread,DotsGetFeatureValue(FEAT_Micro_CSSpread,i));
         DotsInsertRollingValue(FEAT_Micro_Entropy,dots_roll_buf_Micro_Entropy,DotsGetFeatureValue(FEAT_Micro_Entropy,i));
         DotsInsertRollingValue(FEAT_Micro_FailedBreak,dots_roll_buf_Micro_FailedBreak,DotsGetFeatureValue(FEAT_Micro_FailedBreak,i));
         DotsInsertRollingValue(FEAT_Micro_FractalDim,dots_roll_buf_Micro_FractalDim,DotsGetFeatureValue(FEAT_Micro_FractalDim,i));
         DotsInsertRollingValue(FEAT_Micro_GarmanKlass,dots_roll_buf_Micro_GarmanKlass,DotsGetFeatureValue(FEAT_Micro_GarmanKlass,i));
         DotsInsertRollingValue(FEAT_Micro_HLAsymmetry,dots_roll_buf_Micro_HLAsymmetry,DotsGetFeatureValue(FEAT_Micro_HLAsymmetry,i));
         DotsInsertRollingValue(FEAT_Micro_Hurst,dots_roll_buf_Micro_Hurst,DotsGetFeatureValue(FEAT_Micro_Hurst,i));
         DotsInsertRollingValue(FEAT_Micro_IBSP,dots_roll_buf_Micro_IBSP,DotsGetFeatureValue(FEAT_Micro_IBSP,i));
         DotsInsertRollingValue(FEAT_Micro_Lambda,dots_roll_buf_Micro_Lambda,DotsGetFeatureValue(FEAT_Micro_Lambda,i));
         DotsInsertRollingValue(FEAT_Micro_LogReturn,dots_roll_buf_Micro_LogReturn,DotsGetFeatureValue(FEAT_Micro_LogReturn,i));
         DotsInsertRollingValue(FEAT_Micro_MicroGap,dots_roll_buf_Micro_MicroGap,DotsGetFeatureValue(FEAT_Micro_MicroGap,i));
         DotsInsertRollingValue(FEAT_Micro_MomoTransfer,dots_roll_buf_Micro_MomoTransfer,DotsGetFeatureValue(FEAT_Micro_MomoTransfer,i));
         DotsInsertRollingValue(FEAT_Micro_OrderFlowDelta,dots_roll_buf_Micro_OrderFlowDelta,DotsGetFeatureValue(FEAT_Micro_OrderFlowDelta,i));
         DotsInsertRollingValue(FEAT_Micro_PriceAccel,dots_roll_buf_Micro_PriceAccel,DotsGetFeatureValue(FEAT_Micro_PriceAccel,i));
         DotsInsertRollingValue(FEAT_Micro_RangeAccel,dots_roll_buf_Micro_RangeAccel,DotsGetFeatureValue(FEAT_Micro_RangeAccel,i));
         DotsInsertRollingValue(FEAT_Micro_RangeVelocity,dots_roll_buf_Micro_RangeVelocity,DotsGetFeatureValue(FEAT_Micro_RangeVelocity,i));
         DotsInsertRollingValue(FEAT_Micro_Rejection,dots_roll_buf_Micro_Rejection,DotsGetFeatureValue(FEAT_Micro_Rejection,i));
         DotsInsertRollingValue(FEAT_Micro_RollProxy,dots_roll_buf_Micro_RollProxy,DotsGetFeatureValue(FEAT_Micro_RollProxy,i));
         DotsInsertRollingValue(FEAT_Micro_ThrustEff,dots_roll_buf_Micro_ThrustEff,DotsGetFeatureValue(FEAT_Micro_ThrustEff,i));
         DotsInsertRollingValue(FEAT_Micro_TickIntensity,dots_roll_buf_Micro_TickIntensity,DotsGetFeatureValue(FEAT_Micro_TickIntensity,i));
         DotsInsertRollingValue(FEAT_Micro_VPIN,dots_roll_buf_Micro_VPIN,DotsGetFeatureValue(FEAT_Micro_VPIN,i));
         DotsInsertRollingValue(FEAT_Micro_VolAccel,dots_roll_buf_Micro_VolAccel,DotsGetFeatureValue(FEAT_Micro_VolAccel,i));
         DotsInsertRollingValue(FEAT_Micro_VolOfVol,dots_roll_buf_Micro_VolOfVol,DotsGetFeatureValue(FEAT_Micro_VolOfVol,i));
         DotsInsertRollingValue(FEAT_Micro_WickImbalance,dots_roll_buf_Micro_WickImbalance,DotsGetFeatureValue(FEAT_Micro_WickImbalance,i));
         DotsInsertRollingValue(FEAT_VWAP_Dist_ATR,dots_roll_buf_VWAP_Dist_ATR,DotsGetFeatureValue(FEAT_VWAP_Dist_ATR,i));
         DotsInsertRollingValue(FEAT_VAH_Dist_ATR,dots_roll_buf_VAH_Dist_ATR,DotsGetFeatureValue(FEAT_VAH_Dist_ATR,i));
         DotsInsertRollingValue(FEAT_VAL_Dist_ATR,dots_roll_buf_VAL_Dist_ATR,DotsGetFeatureValue(FEAT_VAL_Dist_ATR,i));
         DotsInsertRollingValue(FEAT_PrevDay_High_Dist_ATR,dots_roll_buf_PrevDay_High_Dist_ATR,DotsGetFeatureValue(FEAT_PrevDay_High_Dist_ATR,i));
         DotsInsertRollingValue(FEAT_PrevDay_Low_Dist_ATR,dots_roll_buf_PrevDay_Low_Dist_ATR,DotsGetFeatureValue(FEAT_PrevDay_Low_Dist_ATR,i));
         DotsInsertRollingValue(FEAT_PrevDay_Close_Dist_ATR,dots_roll_buf_PrevDay_Close_Dist_ATR,DotsGetFeatureValue(FEAT_PrevDay_Close_Dist_ATR,i));
         DotsInsertRollingValue(FEAT_DailyOpen_Dist_ATR,dots_roll_buf_DailyOpen_Dist_ATR,DotsGetFeatureValue(FEAT_DailyOpen_Dist_ATR,i));
         DotsInsertRollingValue(FEAT_Round_100_Dist_ATR,dots_roll_buf_Round_100_Dist_ATR,DotsGetFeatureValue(FEAT_Round_100_Dist_ATR,i));
         DotsInsertRollingValue(FEAT_Round_500_Dist_ATR,dots_roll_buf_Round_500_Dist_ATR,DotsGetFeatureValue(FEAT_Round_500_Dist_ATR,i));
         DotsInsertRollingValue(FEAT_Round_1000_Dist_ATR,dots_roll_buf_Round_1000_Dist_ATR,DotsGetFeatureValue(FEAT_Round_1000_Dist_ATR,i));
         DotsInsertRollingValue(FEAT_OR_High_Dist_ATR,dots_roll_buf_OR_High_Dist_ATR,DotsGetFeatureValue(FEAT_OR_High_Dist_ATR,i));
         DotsInsertRollingValue(FEAT_OR_Low_Dist_ATR,dots_roll_buf_OR_Low_Dist_ATR,DotsGetFeatureValue(FEAT_OR_Low_Dist_ATR,i));
         DotsInsertRollingValue(FEAT_Session_High_Dist_ATR,dots_roll_buf_Session_High_Dist_ATR,DotsGetFeatureValue(FEAT_Session_High_Dist_ATR,i));
         DotsInsertRollingValue(FEAT_Session_Low_Dist_ATR,dots_roll_buf_Session_Low_Dist_ATR,DotsGetFeatureValue(FEAT_Session_Low_Dist_ATR,i));
         DotsInsertRollingValue(FEAT_WeeklyOpen_Dist_ATR,dots_roll_buf_WeeklyOpen_Dist_ATR,DotsGetFeatureValue(FEAT_WeeklyOpen_Dist_ATR,i));
         DotsInsertRollingValue(FEAT_MultiDay_Slope,dots_roll_buf_MultiDay_Slope,DotsGetFeatureValue(FEAT_MultiDay_Slope,i));
         DotsInsertRollingValue(FEAT_MultiDay_Position,dots_roll_buf_MultiDay_Position,DotsGetFeatureValue(FEAT_MultiDay_Position,i));
         DotsInsertRollingValue(FEAT_VWAP_Sigma_ATR,dots_roll_buf_VWAP_Sigma_ATR,DotsGetFeatureValue(FEAT_VWAP_Sigma_ATR,i));
         DotsInsertRollingValue(FEAT_VA_Position,dots_roll_buf_VA_Position,DotsGetFeatureValue(FEAT_VA_Position,i));
         DotsInsertRollingValue(FEAT_ADX_Value,dots_roll_buf_ADX_Value,DotsGetFeatureValue(FEAT_ADX_Value,i));
         DotsInsertRollingValue(FEAT_Body_Size,dots_roll_buf_Body_Size,DotsGetFeatureValue(FEAT_Body_Size,i));
         DotsInsertRollingValue(FEAT_Upper_Wick,dots_roll_buf_Upper_Wick,DotsGetFeatureValue(FEAT_Upper_Wick,i));
         DotsInsertRollingValue(FEAT_Lower_Wick,dots_roll_buf_Lower_Wick,DotsGetFeatureValue(FEAT_Lower_Wick,i));
         DotsInsertRollingValue(FEAT_TChan_A15,dots_roll_buf_TChan_A15,DotsGetFeatureValue(FEAT_TChan_A15,i));
         DotsInsertRollingValue(FEAT_VWAP_Sigma,dots_roll_buf_VWAP_Sigma,DotsGetFeatureValue(FEAT_VWAP_Sigma,i));
         DotsInsertRollingValue(FEAT_Volume,dots_roll_buf_Volume,DotsGetFeatureValue(FEAT_Volume,i));
      }
      int barDay=TimeDay(Time[i]);
      if(barDay!=seedDay) { DotsRefreshRollingPercentiles(); seedDay=barDay; }
      string row=TimeToString(Time[i],TIME_DATE|TIME_MINUTES);
      for(int f=0; f<81; f++)
         row=row+","+DoubleToString(dots_threshold[f][THR_HI],6)+","+DoubleToString(dots_threshold[f][THR_LO],6);
      for(int f=83; f<DOTS_NUM_FEATURES; f++)
         row=row+","+DoubleToString(dots_threshold[f][THR_HI],6)+","+DoubleToString(dots_threshold[f][THR_LO],6);
      FileWriteString(fh,row+"\r\n");
   }
   FileClose(fh);
   Print("DOTS| Threshold snapshots exported: ",scanLimit," bars");
}
//+------------------------------------------------------------------+
//| SECTION 6.8 - DOTS RULE TABLE                                    |
//+------------------------------------------------------------------+
struct DotsRuleDef {
   int direction;
   int feat1;
   int dir1;
   int feat2;
   int dir2;
   int feat3;
   int dir3;
};
DotsRuleDef dots_rules[DOTS_NUM_RULES];
void R(int i,int d,int f1,int t1,int f2,int t2,int f3,int t3) {
   dots_rules[i].direction=d;
   dots_rules[i].feat1=f1; dots_rules[i].dir1=t1;
   dots_rules[i].feat2=f2; dots_rules[i].dir2=t2;
   dots_rules[i].feat3=f3; dots_rules[i].dir3=t3;
}
void InitDotsRuleTable() {
   R( 0,-1,FEAT_OBV_Macd,THR_HI,FEAT_OBV_Velocity,THR_LO,FEAT_Volume_Avg_10,THR_LO);
   R( 1,+1,FEAT_ATR_1M,THR_HI,FEAT_RangeOsc_Val,THR_LO,FEAT_Micro_BarOverlap,THR_HI);
   R( 2,+1,FEAT_AT_Slope_ST,THR_LO,FEAT_Micro_Lambda,THR_LO,FEAT_Micro_Hurst,THR_HI);
   R( 3,+1,FEAT_AT_Score_ST,THR_LO,FEAT_Micro_BarEntropy,THR_LO,FEAT_Micro_FailedBreak,THR_LO);
   R( 4,+1,FEAT_AT_Slope_ST,THR_LO,FEAT_Micro_Lambda,THR_LO,FEAT_Micro_AutoCorr,THR_HI);
   R( 5,+1,FEAT_Slope_Accel_LT,THR_LO,FEAT_Micro_PriceAccel,THR_LO,FEAT_Micro_WickImbalance,THR_HI);
   R( 6,+1,FEAT_KAMA_Dist,THR_LO,FEAT_Micro_OrderFlowDelta,THR_HI,FEAT_Micro_MicroGap,THR_LO);
   R( 7,+1,FEAT_KAMA_Dist,THR_LO,FEAT_Micro_TickIntensity,THR_HI,FEAT_Micro_WickImbalance,THR_HI);
   R( 8,+1,FEAT_Volume_Ratio_10,THR_LO,FEAT_Micro_MomoTransfer,THR_HI,FEAT_Micro_RangeAccel,THR_LO);
   R( 9,+1,FEAT_AT_Score_ST,THR_LO,FEAT_Micro_Lambda,THR_HI,FEAT_Micro_FailedBreak,THR_LO);
   R(10,+1,FEAT_AT_Slope_ST,THR_LO,FEAT_Micro_TickIntensity,THR_HI,FEAT_Micro_Rejection,THR_HI);
   R(11,+1,FEAT_AT_Slope_ST,THR_LO,FEAT_Micro_HLAsymmetry,THR_LO,FEAT_Micro_FractalDim,THR_LO);
   R(12,+1,FEAT_OBVf_DirStepCount,THR_HI,FEAT_AT_Score_LT,THR_LO,FEAT_Slope_Accel_ST,THR_LO);
   R(13,-1,FEAT_D2D_Persist,THR_HI,FEAT_Micro_VolAccel,THR_HI,FEAT_Micro_AutoCorr,THR_LO);
   R(14,+1,FEAT_ATR_1M,THR_LO,FEAT_Micro_Entropy,THR_LO,FEAT_Micro_WickImbalance,THR_HI);
   R(15,-1,FEAT_KAMA_Dist,THR_HI,FEAT_Momentum_Value,THR_HI,FEAT_Micro_Hurst,THR_HI);
   R(16,+1,FEAT_OBV_Macd,THR_LO,FEAT_Dist_To_PoC_ATR,THR_HI,FEAT_Micro_IBSP,THR_LO);
   R(17,+1,FEAT_Slope_EMA_ST,THR_LO,FEAT_Micro_OrderFlowDelta,THR_LO,FEAT_Micro_HLAsymmetry,THR_HI);
   R(18,-1,FEAT_KAMA_Dist_ATR,THR_LO,FEAT_Slope_Accel_LT,THR_HI,FEAT_Micro_WickImbalance,THR_LO);
   R(19,-1,FEAT_OBV_Velocity,THR_HI,FEAT_Micro_TickIntensity,THR_LO,FEAT_Micro_MicroGap,THR_HI);
   R(20,-1,FEAT_D2D_ATR,THR_LO,FEAT_EMA_Oscillator,THR_LO,FEAT_Micro_VolOfVol,THR_LO);
   R(21,-1,FEAT_D2D_Persist,THR_HI,FEAT_Micro_HLAsymmetry,THR_HI,FEAT_Micro_RangeAccel,THR_HI);
   R(22,-1,FEAT_Volume_Avg_10,THR_LO,FEAT_Slope_Accel_ST,THR_HI,FEAT_Micro_MicroGap,THR_LO);
   R(23,-1,FEAT_KAMA_Dist_ATR,THR_HI,FEAT_Sqz_Val,THR_HI,FEAT_Bars_Since_Flip,THR_HI);
   R(24,-1,FEAT_AT_Score_LT,THR_LO,FEAT_Micro_AutoCorr,THR_HI,FEAT_Micro_Hurst,THR_LO);
   R(25,+1,FEAT_AT_Score_ST,THR_LO,FEAT_Micro_FractalDim,THR_LO,FEAT_Micro_CSSpread,THR_HI);
   R(26,+1,FEAT_Sqz_Val,THR_LO,FEAT_Slope_Accel_LT,THR_LO,FEAT_Micro_GarmanKlass,THR_HI);
   R(27,+1,FEAT_Bar_Range,THR_HI,FEAT_Efficiency_Ratio,THR_LO,FEAT_Slope_EMA_ST,THR_LO);
   R(28,+1,FEAT_Slope_EMA_ST,THR_LO,FEAT_Micro_Lambda,THR_HI,FEAT_Micro_LogReturn,THR_LO);
   R(29,+1,FEAT_KAMA_Slope,THR_HI,FEAT_KAMA_Dist,THR_LO,FEAT_Harmonic_LLEMA,THR_HI);
   R(30,+1,FEAT_Slope_Accel_LT,THR_LO,FEAT_Micro_ThrustEff,THR_LO,FEAT_Micro_FractalDim,THR_LO);
   R(31,-1,FEAT_AT_Slope_ST,THR_HI,FEAT_Micro_MicroGap,THR_LO,FEAT_Micro_Amihud,THR_HI);
   R(32,-1,FEAT_KAMA_Slope,THR_HI,FEAT_Micro_AutoCorr,THR_HI,FEAT_Micro_Entropy,THR_HI);
   R(33,-1,FEAT_EMA_Oscillator,THR_HI,FEAT_Micro_LogReturn,THR_HI,FEAT_Micro_Hurst,THR_HI);
   R(34,-1,FEAT_Slope_EMA_ST,THR_HI,FEAT_Micro_BarEntropy,THR_HI,FEAT_Micro_MomoTransfer,THR_LO);
   R(35,-1,FEAT_OBV_Velocity,THR_HI,FEAT_AT_Slope_ST,THR_HI,FEAT_AT_Slope_LT,THR_HI);
   R(36,-1,FEAT_Micro_IBSP,THR_HI,FEAT_Micro_BarEntropy,THR_HI,FEAT_Micro_Hurst,THR_HI);
   R(37,+1,FEAT_KAMA_Dist_ATR,THR_LO,FEAT_Micro_TickIntensity,THR_HI,FEAT_Micro_AutoCorr,THR_HI);
   R(38,+1,FEAT_Volume_Ratio_10,THR_HI,FEAT_Micro_BarOverlap,THR_HI,FEAT_Micro_FailedBreak,THR_LO);
   R(39,-1,FEAT_Momentum_Value,THR_HI,FEAT_Micro_FailedBreak,THR_HI,FEAT_Micro_Hurst,THR_HI);
   R(40,-1,FEAT_Sqz_Val,THR_LO,FEAT_Volume_Avg_10,THR_LO,FEAT_Micro_AutoCorr,THR_LO);
   R(41,-1,FEAT_Efficiency_Ratio,THR_HI,FEAT_Micro_Lambda,THR_HI,FEAT_Micro_Rejection,THR_LO);
   R(42,+1,FEAT_Micro_RollProxy,THR_HI,FEAT_Micro_BarOverlap,THR_HI,FEAT_Micro_Amihud,THR_HI);
   R(43,-1,FEAT_Micro_Rejection,THR_HI,FEAT_Micro_MicroGap,THR_LO,FEAT_Micro_ThrustEff,THR_HI);
   R(44,-1,FEAT_D2D_ATR_MA,THR_LO,FEAT_Sqz_Val,THR_LO,FEAT_Micro_BarEntropy,THR_LO);
   R(45,-1,FEAT_Momentum_Value,THR_LO,FEAT_Micro_AutoCorr,THR_LO,FEAT_Micro_Hurst,THR_HI);
   R(46,+1,FEAT_Micro_MomoTransfer,THR_LO,FEAT_Micro_MicroGap,THR_LO,FEAT_Micro_FractalDim,THR_LO);
   R(47,-1,FEAT_AT_Score_LT,THR_HI,FEAT_Slope_Accel_ST,THR_LO,FEAT_Micro_RollProxy,THR_LO);
   R(48,-1,FEAT_Momentum_Value,THR_HI,FEAT_Micro_PriceAccel,THR_HI,FEAT_Micro_Hurst,THR_HI);
   R(49,-1,FEAT_D2D_Dynamic_Sensitivity,THR_LO,FEAT_Micro_Rejection,THR_LO,FEAT_Micro_HLAsymmetry,THR_LO);
   R(50,-1,FEAT_OBVf_DirStepCount,THR_LO,FEAT_Micro_RollProxy,THR_HI,FEAT_Micro_Amihud,THR_LO);
   R(51,+1,FEAT_OBVf_DirStepCount,THR_LO,FEAT_Slope_EMA_ST,THR_LO,FEAT_Micro_BarOverlap,THR_LO);
   R(52,+1,FEAT_Slope_EMA_ST,THR_LO,FEAT_Micro_MomoTransfer,THR_LO,FEAT_Micro_VPIN,THR_LO);
   R(53,+1,FEAT_Slope_EMA_ST,THR_LO,FEAT_Micro_BarOverlap,THR_HI,FEAT_Micro_VolOfVol,THR_HI);
   R(54,+1,FEAT_D2D_ATR,THR_HI,FEAT_KAMA_Dist,THR_LO,FEAT_Micro_ThrustEff,THR_HI);
   R(55,+1,FEAT_AT_Slope_ST,THR_LO,FEAT_Micro_Lambda,THR_HI,FEAT_Micro_FractalDim,THR_HI);
   R(56,-1,FEAT_OBV_Velocity,THR_HI,FEAT_Momentum_Value,THR_LO,FEAT_Micro_RangeVelocity,THR_HI);
   R(57,-1,FEAT_Volume_Avg_10,THR_HI,FEAT_Slope_EMA_ST,THR_HI,FEAT_Micro_VolOfVol,THR_HI);
   R(58,-1,FEAT_OBV_Macd,THR_LO,FEAT_Slope_EMA_LT,THR_HI,FEAT_Micro_FractalDim,THR_HI);
   R(59,-1,FEAT_D2D_ATR_MA,THR_HI,FEAT_AT_Score_LT,THR_HI,FEAT_Micro_FractalDim,THR_HI);
   R(60,+1,FEAT_D2D_Dynamic_Sensitivity,THR_LO,FEAT_Bars_Since_Flip,THR_HI,FEAT_Micro_VPIN,THR_HI);
   R(61,+1,FEAT_OBV_Macd,THR_LO,FEAT_KAMA_Dist_ATR,THR_HI,FEAT_Micro_CSSpread,THR_LO);
   R(62,-1,FEAT_Micro_LogReturn,THR_HI,FEAT_Micro_VolAccel,THR_LO,FEAT_Micro_ThrustEff,THR_LO);
   R(63,+1,FEAT_D2D_Dynamic_Sensitivity,THR_LO,FEAT_Dist_To_PoC_ATR,THR_LO,FEAT_Micro_Hurst,THR_HI);
   R(64,-1,FEAT_RangeOsc_Val,THR_HI,FEAT_Micro_HLAsymmetry,THR_HI,FEAT_Micro_Hurst,THR_HI);
   R(65,-1,FEAT_Bars_Since_Flip,THR_LO,FEAT_AT_Score_ST,THR_LO,FEAT_AT_Score_LT,THR_HI);
   R(66,+1,FEAT_RangeOsc_Val,THR_HI,FEAT_Slope_EMA_LT,THR_LO,FEAT_Micro_Lambda,THR_HI);
   R(67,-1,FEAT_D2D_ATR,THR_LO,FEAT_KAMA_Slope,THR_LO,FEAT_Slope_Accel_ST,THR_LO);
   R(68,+1,FEAT_Micro_VolAccel,THR_LO,FEAT_Micro_RangeVelocity,THR_LO,FEAT_Micro_RangeAccel,THR_HI);
   R(69,+1,FEAT_AT_Lookback_ST,THR_HI,FEAT_Micro_FailedBreak,THR_LO,FEAT_Micro_RangeVelocity,THR_LO);
   R(70,+1,FEAT_D2D_Up_Count,THR_HI,FEAT_KAMA_Dist_ATR,THR_HI,FEAT_Micro_Entropy,THR_LO);
   R(71,-1,FEAT_D2D_Dn_Count,THR_HI,FEAT_Micro_TickIntensity,THR_LO,FEAT_Micro_CSSpread,THR_LO);
   R(72,-1,FEAT_Bar_Range,THR_LO,FEAT_D2D_Dynamic_Sensitivity,THR_LO,FEAT_Micro_RangeVelocity,THR_LO);
   R(73,-1,FEAT_D2D_Persist,THR_LO,FEAT_RangeOsc_Val,THR_LO,FEAT_Bars_Since_Flip,THR_LO);
   R(74,+1,FEAT_OBVf_DirStepCount,THR_LO,FEAT_AT_Lookback_LT,THR_HI,FEAT_Micro_VPIN,THR_HI);
   R(75,+1,FEAT_KAMA_Dist_ATR,THR_LO,FEAT_Micro_GarmanKlass,THR_LO,FEAT_Micro_BarOverlap,THR_HI);
}
//+------------------------------------------------------------------+
//| SECTION 6.9 - DOTS SIGNAL EVALUATION                             |
//+------------------------------------------------------------------+
bool dots_cleared[DOTS_NUM_RULES];
double DotsGetFeatureValue(int featId,int b) {
   switch(featId) {
      case FEAT_ATR_1M:              return ATR_1M_Array[b];
      case FEAT_Bar_Range:           return High[b]-Low[b];
      case FEAT_D2D_ATR:             return U_AtrBuffer[b];
      case FEAT_D2D_ATR_MA:          return U_AtrMaBuffer[b];
      case FEAT_D2D_Dn_Count:        return (double)U_DnCntBuffer[b];
      case FEAT_D2D_Dynamic_Sensitivity: return hist_Brain_Sensitivity[b];
      case FEAT_D2D_Persist:         return U_PersistBuffer[b];
      case FEAT_D2D_Up_Count:        return (double)U_UpCntBuffer[b];
      case FEAT_AT_Lookback_LT:      return (double)hist_LT_detectedAnchorBar_ST[b];
      case FEAT_AT_Lookback_ST:      return (double)hist_detectedAnchorBar_ST[b];
      case FEAT_AT_Score_LT:         return (double)hist_LT_trendStep_ST[b];
      case FEAT_AT_Score_ST:         return (double)hist_trendStep_ST[b];
      case FEAT_AT_Slope_LT:         return hist_LT_detectedSlope_ST[b];
      case FEAT_AT_Slope_ST:         return hist_detectedSlope_ST[b];
      case FEAT_Bars_Since_Flip:     return (double)hist_BarsSinceFlip_ST[b];
      case FEAT_Slope_EMA_LT:        return state_Slope_EMA_LT[b];
      case FEAT_Slope_EMA_ST:        return state_Slope_EMA_ST[b];
      case FEAT_Slope_Accel_LT:      return state_Slope_Accel_LT[b];
      case FEAT_Slope_Accel_ST:      return state_Slope_Accel_ST[b];
      case FEAT_OBV_Macd:            return state_OBV_Macd[b];
      case FEAT_OBV_Velocity:        return state_OBV_Velocity[b];
      case FEAT_OBVf_DirStepCount:   return OBV_DirStepCountBuffer[b];
      case FEAT_KAMA_Dist:           return Close[b]-state_HarmVol_KAMA[b];
      case FEAT_KAMA_Dist_ATR:       return hist_KAMA_Dist_ATR[b];
      case FEAT_KAMA_Slope:          return hist_KAMA_Slope[b];
      case FEAT_EMA_Oscillator:      return state_HarmVol_EMAOsc[b];
      case FEAT_Harmonic_LLEMA:      return state_HarmVol_LLEMA[b];
      case FEAT_Sqz_Val:             return state_Sqz_Val[b];
      case FEAT_RangeOsc_Val:        return state_RangeOsc_Val[b];
      case FEAT_Volume_Avg_10: {
         double sum=0.0; int cnt=0;
         int limV=ArraySize(hist_VolumeValue);
         for(int k=1;k<=10;k++) {
            int idx=b+k;
            if(idx<limV) { double v=hist_VolumeValue[idx]; if(v>0.0){sum+=v;cnt++;} }
         }
         return (cnt>0)?sum/(double)cnt:0.0;
      }
      case FEAT_Volume_Ratio_10:     return hist_VolumeRatio10[b];
      case FEAT_Momentum_Value:      return MomentumBuffer[b];
      case FEAT_Efficiency_Ratio:    return hist_Efficiency_Ratio[b];
      case FEAT_Dist_To_PoC_ATR: {
         double atr=assignedATR[b];
         double poc=hist_Poc_Price[b];
         if(atr<=0.0||poc<=0.0) return 0.0;
         return MathAbs(Close[b]-poc)/atr;
      }
      case FEAT_Micro_Amihud:        return state_Micro_Amihud[b];
      case FEAT_Micro_AutoCorr:      return state_Micro_AutoCorr[b];
      case FEAT_Micro_BarEntropy:    return state_Micro_BarEntropy[b];
      case FEAT_Micro_BarOverlap:    return state_Micro_BarOverlap[b];
      case FEAT_Micro_CSSpread:      return state_Micro_CSSpread[b];
      case FEAT_Micro_Entropy:       return state_Micro_Entropy[b];
      case FEAT_Micro_FailedBreak:   return state_Micro_FailedBreak[b];
      case FEAT_Micro_FractalDim:    return state_Micro_FractalDim[b];
      case FEAT_Micro_GarmanKlass:   return state_Micro_GarmanKlass[b];
      case FEAT_Micro_HLAsymmetry:   return state_Micro_HLAsymmetry[b];
      case FEAT_Micro_Hurst:         return state_Micro_Hurst[b];
      case FEAT_Micro_IBSP:          return state_Micro_IBSP[b];
      case FEAT_Micro_Lambda:        return state_Micro_Lambda[b];
      case FEAT_Micro_LogReturn:     return state_Micro_LogReturn[b];
      case FEAT_Micro_MicroGap:      return state_Micro_MicroGap[b];
      case FEAT_Micro_MomoTransfer:  return state_Micro_MomoTransfer[b];
      case FEAT_Micro_OrderFlowDelta:return state_Micro_OrderFlowDelta[b];
      case FEAT_Micro_PriceAccel:    return state_Micro_PriceAccel[b];
      case FEAT_Micro_RangeAccel:    return state_Micro_RangeAccel[b];
      case FEAT_Micro_RangeVelocity: return state_Micro_RangeVelocity[b];
      case FEAT_Micro_Rejection:     return state_Micro_Rejection[b];
      case FEAT_Micro_RollProxy:     return state_Micro_RollProxy[b];
      case FEAT_Micro_ThrustEff:     return state_Micro_ThrustEff[b];
      case FEAT_Micro_TickIntensity: return state_Micro_TickIntensity[b];
      case FEAT_Micro_VPIN:          return state_Micro_VPIN[b];
      case FEAT_Micro_VolAccel:      return state_Micro_VolAccel[b];
      case FEAT_Micro_VolOfVol:      return state_Micro_VolOfVol[b];
      case FEAT_Micro_WickImbalance: return state_Micro_WickImbalance[b];
      case FEAT_VWAP_Dist_ATR: {
         double atr=assignedATR[b];
         double px=hist_VWAP_Price[b];
         if(atr<=0.0||px<=0.0) return 0.0;
         return MathAbs(Close[b]-px)/atr;
      }
      case FEAT_VAH_Dist_ATR: {
         double atr=assignedATR[b];
         double px=hist_VAH_Price[b];
         if(atr<=0.0||px<=0.0) return 0.0;
         return MathAbs(Close[b]-px)/atr;
      }
      case FEAT_VAL_Dist_ATR: {
         double atr=assignedATR[b];
         double px=hist_VAL_Price[b];
         if(atr<=0.0||px<=0.0) return 0.0;
         return MathAbs(Close[b]-px)/atr;
      }
      case FEAT_PrevDay_High_Dist_ATR: {
         double atr=assignedATR[b];
         double px=hist_PrevDay_High[b];
         if(atr<=0.0||px<=0.0) return 0.0;
         return MathAbs(Close[b]-px)/atr;
      }
      case FEAT_PrevDay_Low_Dist_ATR: {
         double atr=assignedATR[b];
         double px=hist_PrevDay_Low[b];
         if(atr<=0.0||px<=0.0) return 0.0;
         return MathAbs(Close[b]-px)/atr;
      }
      case FEAT_PrevDay_Close_Dist_ATR: {
         double atr=assignedATR[b];
         double px=hist_PrevDay_Close[b];
         if(atr<=0.0||px<=0.0) return 0.0;
         return MathAbs(Close[b]-px)/atr;
      }
      case FEAT_DailyOpen_Dist_ATR: {
         double atr=assignedATR[b];
         double px=hist_DailyOpen_Price[b];
         if(atr<=0.0||px<=0.0) return 0.0;
         return MathAbs(Close[b]-px)/atr;
      }
      case FEAT_Round_100_Dist_ATR: {
         double atr=assignedATR[b];
         if(atr<=0.0) return 0.0;
         return MathAbs(Close[b]-MathRound(Close[b]/100.0)*100.0)/atr;
      }
      case FEAT_Round_500_Dist_ATR: {
         double atr=assignedATR[b];
         if(atr<=0.0) return 0.0;
         return MathAbs(Close[b]-MathRound(Close[b]/500.0)*500.0)/atr;
      }
      case FEAT_Round_1000_Dist_ATR: {
         double atr=assignedATR[b];
         if(atr<=0.0) return 0.0;
         return MathAbs(Close[b]-MathRound(Close[b]/1000.0)*1000.0)/atr;
      }
      case FEAT_OR_High_Dist_ATR: {
         double atr=assignedATR[b];
         double px=hist_OR_High[b];
         if(atr<=0.0||px<=0.0) return 0.0;
         return MathAbs(Close[b]-px)/atr;
      }
      case FEAT_OR_Low_Dist_ATR: {
         double atr=assignedATR[b];
         double px=hist_OR_Low[b];
         if(atr<=0.0||px<=0.0) return 0.0;
         return MathAbs(Close[b]-px)/atr;
      }
      case FEAT_Session_High_Dist_ATR: {
         double atr=assignedATR[b];
         double px=hist_Session_High[b];
         if(atr<=0.0||px<=0.0) return 0.0;
         return MathAbs(Close[b]-px)/atr;
      }
      case FEAT_Session_Low_Dist_ATR: {
         double atr=assignedATR[b];
         double px=hist_Session_Low[b];
         if(atr<=0.0||px<=0.0) return 0.0;
         return MathAbs(Close[b]-px)/atr;
      }
      case FEAT_WeeklyOpen_Dist_ATR: {
         double atr=assignedATR[b];
         double px=hist_WeeklyOpen_Price[b];
         if(atr<=0.0||px<=0.0) return 0.0;
         return MathAbs(Close[b]-px)/atr;
      }
      case FEAT_MultiDay_Slope:      return hist_MultiDay_Slope[b];
      case FEAT_MultiDay_Position:   return hist_MultiDay_Position[b];
      case FEAT_VWAP_Sigma_ATR: {
         double atr=assignedATR[b];
         if(atr<=0.0) return 0.0;
         return hist_VWAP_Sigma[b]/atr;
      }
      case FEAT_VA_Position: {
         double vah=hist_VAH_Price[b];
         double val=hist_VAL_Price[b];
         if(vah>0.0&&val>0.0&&vah>val) return (Close[b]-val)/(vah-val);
         return 0.5;
      }
      case FEAT_VWAP_Z: {
         double sig=hist_VWAP_Sigma[b];
         double px=hist_VWAP_Price[b];
         if(sig>0.0&&px>0.0) return (Close[b]-px)/sig;
         return 0.0;
      }
      case FEAT_OR_Position: {
         double orh=hist_OR_High[b];
         double orl=hist_OR_Low[b];
         if(orh>0.0&&orl>0.0&&orh>orl) return (Close[b]-orl)/(orh-orl);
         return 0.5;
      }
      case FEAT_ADX_Value:            return hist_ADXValue[b];
      case FEAT_Body_Size:            return MathAbs(Close[b]-Open[b]);
      case FEAT_Upper_Wick:           return High[b]-MathMax(Open[b],Close[b]);
      case FEAT_Lower_Wick:           return MathMin(Open[b],Close[b])-Low[b];
      case FEAT_TChan_A15:            return state_TChan_Sum[b];
      case FEAT_VWAP_Sigma:           return hist_VWAP_Sigma[b];
      case FEAT_Volume:               return hist_VolumeValue[b];
      default: return 0.0;
   }
}
bool DotsCheckCondition(double val,int dir,int feat) {
   double thr=dots_threshold[feat][dir];
   if(dir==THR_HI) return (val>thr);
   return (val<thr);
}
bool DotsRuleUsesKama(int r) {
   int f1=dots_rules[r].feat1;
   int f2=dots_rules[r].feat2;
   int f3=dots_rules[r].feat3;
   if(f1==FEAT_KAMA_Dist||f1==FEAT_KAMA_Dist_ATR||f1==FEAT_KAMA_Slope) return true;
   if(f2==FEAT_KAMA_Dist||f2==FEAT_KAMA_Dist_ATR||f2==FEAT_KAMA_Slope) return true;
   if(f3==FEAT_KAMA_Dist||f3==FEAT_KAMA_Dist_ATR||f3==FEAT_KAMA_Slope) return true;
   return false;
}
void Eval_Dots_Signals(int barIdx) {
   int i;
   for(i=0;i<DOTS_NUM_RULES;i++) {
      dots_cleared[i]=false;
      dots_state[i].condA=false;
      dots_state[i].condB=false;
      dots_state[i].condC=false;
   }
   if(hist_ADXValue[barIdx]<15.0) return;
   if(hist_VolumeValue[barIdx]<=50.0) return;
   int d2dDir=Direction[barIdx];
   int qualifying=0;
   bool qualified[DOTS_NUM_RULES];
   for(i=0;i<DOTS_NUM_RULES;i++) qualified[i]=false;
   for(i=0;i<DOTS_NUM_RULES;i++) {
      if(dots_rules[i].direction!=d2dDir) continue;
      if(g_warm_suppress_kama_signals&&DotsRuleUsesKama(i)) continue;
      double v1=DotsGetFeatureValue(dots_rules[i].feat1,barIdx);
      double v2=DotsGetFeatureValue(dots_rules[i].feat2,barIdx);
      double v3=DotsGetFeatureValue(dots_rules[i].feat3,barIdx);
      dots_state[i].condA=DotsCheckCondition(v1,dots_rules[i].dir1,dots_rules[i].feat1);
      dots_state[i].condB=DotsCheckCondition(v2,dots_rules[i].dir2,dots_rules[i].feat2);
      dots_state[i].condC=DotsCheckCondition(v3,dots_rules[i].dir3,dots_rules[i].feat3);
      if(dots_state[i].condA&&dots_state[i].condB&&dots_state[i].condC) {
         qualified[i]=true;
         qualifying++;
      }
   }
   if(qualifying==0) return;
   DotsLog("EVAL: "+IntegerToString(qualifying)+" qual");
   double vol=hist_VolumeValue[barIdx];
   bool gateOpen=(qualifying>=Dots_MinConcurrent)||(qualifying==1&&vol>=(double)Dots_SoloVolumeGate);
   if(!gateOpen) {
      DotsLog("GATE: BLOCK "+IntegerToString(qualifying)+" qual, vol<300");
      return;
   }
   if(qualifying>=Dots_MinConcurrent)
      DotsLog("GATE: PASS "+IntegerToString(qualifying)+" concurrent");
   else
      DotsLog("GATE: PASS solo+vol");
   for(i=0;i<DOTS_NUM_RULES;i++)
      if(qualified[i]) dots_cleared[i]=true;
}
//+------------------------------------------------------------------+
//| SECTION 7.0 - CHART ALERTS & VISUALS                             |
//+------------------------------------------------------------------+
void PlayCommittedSignalAlert(int index) {
   if(lastCalculatedBars==0) return;
   if(index>5) return;
   if(Time[index]<=prevSignalAlertTime) return;
   static int lastAlertedSignalDirection=0;
   if(LockBuffer[index]==lastAlertedSignalDirection) return;
   string alertText=(LockBuffer[index]==1)?"Time to long!":"Time to short!";
   Alert("Looms EA: "+alertText+" @ "+TimeToString(Time[index],TIME_DATE|TIME_MINUTES));
   PlaySound("alert.wav");
   prevSignalAlertTime=Time[index];
   lastAlertedSignalDirection=LockBuffer[index];
}
void PlayTrendFlipAlert(int index) {
   if(lastCalculatedBars==0||index>5) return;
   if(Time[index]<=prevTrendFlipAlertTime) return;
   int flip=hist_ST_Flip_Event[index];
   if(flip==0) return;
   static int lastTrendFlipAlertDir=0;
   if(flip==lastTrendFlipAlertDir) return;
   string text=(flip==1)?"Dot demands you to long this bitch!":"This pancake has turned sour. Short this mofo!";
   Alert("Looms Trend Flip: "+text);
   PlaySound("ok.wav");
   prevTrendFlipAlertTime=Time[index];
   lastTrendFlipAlertDir=flip;
}
void PlayOBVAlert(int index) {
   if(lastCalculatedBars==0||index>5) return;
   if(Time[index]<=prevOBVAlertTime) return;
   if(index+1>=Bars) return;
   double current_oc=state_TChan_OC[index];
   if(current_oc!=0 && current_oc!=lastOBVAlertDirection) {
      string text=(current_oc==1.0)?"OBV Momentum Shift: Buying Pressure Incoming!":"OBV Momentum Shift: Selling Pressure Detected!";
      Alert("Looms OBV: "+text);
      PlaySound("news.wav");
      prevOBVAlertTime=Time[index];
      lastOBVAlertDirection=(int)MathRound(current_oc);
   }
}
void DrawArrow(string name,datetime time,double price,color clr,int width=2,int arrow_code=108) {
   string objName=ea_prefix+name;
   if(ObjectFind(0,objName)<0) {
      ObjectCreate(0,objName,OBJ_ARROW,0,time,price);
      ObjectSetInteger(0,objName,OBJPROP_SELECTABLE,false);
   } else {
      ObjectSetInteger(0,objName,OBJPROP_TIME1,time);
      ObjectSetDouble(0,objName,OBJPROP_PRICE1,price);
   }
   ObjectSetInteger(0,objName,OBJPROP_ARROWCODE,arrow_code);
   ObjectSetInteger(0,objName,OBJPROP_WIDTH,width);
   ObjectSetInteger(0,objName,OBJPROP_COLOR,clr);
   ObjectSetInteger(0,objName,OBJPROP_BACK,true);
   ObjectSetInteger(0,objName,OBJPROP_ZORDER,0);
   ObjectSetInteger(0,objName,OBJPROP_ANCHOR,ANCHOR_CENTER);
}
void DrawT2TFlipOnChart(int index,int flipDirection) {
   datetime signalTime=Time[index];
   color vlineColor=(flipDirection==1)?C'146,134,124':C'89,116,124';
   string vlineName=ea_prefix+"t2t_vline_"+TimeToString(signalTime);
   if(ObjectFind(0,vlineName)<0) {
      ObjectCreate(0,vlineName,OBJ_VLINE,0,signalTime,0);
      ObjectSetInteger(0,vlineName,OBJPROP_COLOR,vlineColor);
      ObjectSetInteger(0,vlineName,OBJPROP_STYLE,STYLE_SOLID);
      ObjectSetInteger(0,vlineName,OBJPROP_WIDTH,1);
      ObjectSetInteger(0,vlineName,OBJPROP_BACK,true);
      ObjectSetInteger(0,vlineName,OBJPROP_SELECTABLE,false);
   }
   int chartHeight=(int)ChartGetInteger(0,CHART_HEIGHT_IN_PIXELS);
   if(chartHeight<=0) return;
   double p2p=(WindowPriceMax()-WindowPriceMin())/chartHeight;
   double topCoord_arrow=WindowPriceMax()-(80*p2p);
   int arrowCode=(flipDirection==1)?233:234;
   DrawArrow("t2t_sig_top_"+TimeToString(signalTime),signalTime,topCoord_arrow,vlineColor,1,arrowCode);
}
void DrawSignalOnChart(datetime signalTime,color signalColor) {
   string vlineName=ea_prefix+"vline_"+TimeToString(signalTime);
   if(ObjectFind(0,vlineName)<0) {
      ObjectCreate(0,vlineName,OBJ_VLINE,0,signalTime,0);
      ObjectSetInteger(0,vlineName,OBJPROP_COLOR,signalColor);
      ObjectSetInteger(0,vlineName,OBJPROP_STYLE,STYLE_DOT);
      ObjectSetInteger(0,vlineName,OBJPROP_WIDTH,1);
      ObjectSetInteger(0,vlineName,OBJPROP_BACK,true);
      ObjectSetInteger(0,vlineName,OBJPROP_SELECTABLE,false);
   }
   int chartHeight=(int)ChartGetInteger(0,CHART_HEIGHT_IN_PIXELS);
   if(chartHeight<=0) return;
   double p2p=(WindowPriceMax()-WindowPriceMin())/chartHeight;
   double topCoord=WindowPriceMax()-(45*p2p);
   DrawArrow("sig_top_"+TimeToString(signalTime),signalTime,topCoord,signalColor,2,108);
}
void DrawCustomCandle(int i,color candleColor) {
   string suffix=TimeToString(Time[i],TIME_DATE|TIME_MINUTES|TIME_SECONDS);
   string bodyName=ea_prefix+"c_body_"+suffix;
   string wickName=ea_prefix+"c_wick_"+suffix;
   double open=Open[i]; double close=Close[i]; double high=High[i]; double low=Low[i];
   if(ObjectFind(0,wickName)<0) {
      ObjectCreate(0,wickName,OBJ_TREND,0,Time[i],high,Time[i],low);
      ObjectSetInteger(0,wickName,OBJPROP_WIDTH,1);
      ObjectSetInteger(0,wickName,OBJPROP_RAY,false);
      ObjectSetInteger(0,wickName,OBJPROP_BACK,true);
      ObjectSetInteger(0,wickName,OBJPROP_SELECTABLE,false);
   }
   ObjectSetInteger(0,wickName,OBJPROP_COLOR,candleColor);
   ObjectSetInteger(0,wickName,OBJPROP_TIME1,Time[i]);
   ObjectSetDouble(0,wickName,OBJPROP_PRICE1,high);
   ObjectSetInteger(0,wickName,OBJPROP_TIME2,Time[i]);
   ObjectSetDouble(0,wickName,OBJPROP_PRICE2,low);
   if(ObjectFind(0,bodyName)<0) {
      ObjectCreate(0,bodyName,OBJ_TREND,0,Time[i],open,Time[i],close);
      ObjectSetInteger(0,bodyName,OBJPROP_WIDTH,3);
      ObjectSetInteger(0,bodyName,OBJPROP_RAY,false);
      ObjectSetInteger(0,bodyName,OBJPROP_BACK,true);
      ObjectSetInteger(0,bodyName,OBJPROP_SELECTABLE,false);
   }
   ObjectSetInteger(0,bodyName,OBJPROP_COLOR,candleColor);
   ObjectSetInteger(0,bodyName,OBJPROP_TIME1,Time[i]);
   ObjectSetDouble(0,bodyName,OBJPROP_PRICE1,open);
   ObjectSetInteger(0,bodyName,OBJPROP_TIME2,Time[i]);
   ObjectSetDouble(0,bodyName,OBJPROP_PRICE2,close);
}
void ColorSignalBars() {
   if(Chart_Visual_Mode==MODE_CANDLES) {
      for(int i=ObjectsTotal(0,-1,-1)-1; i>=0; i--) {
         string name=ObjectName(0,i,-1,-1);
         if(StringFind(name,ea_prefix+"c_")==0) ObjectDelete(0,name);
      }
      int activeDir = 0;
      if(showOBVfCandles) {
         for(int i=Bars-1; i>=1; i--) {
            if(state_TChan_OC[i] == 1.0 || state_TChan_OC[i] == -1.0) {
               activeDir = (int)state_TChan_OC[i];
            }
            if(activeDir != 0) {
               color cColor=(activeDir==1)?clrOrange:clrDodgerBlue;
               DrawCustomCandle(i,cColor);
            }
         }
      } else {
         for(int i=Bars-1; i>=1; i--) {
            if(LockBuffer[i] == 1 || LockBuffer[i] == -1) {
               activeDir = LockBuffer[i];
            }
            if(activeDir != 0) {
               color cColor=(activeDir==1)?clrOrange:clrDodgerBlue;
               DrawCustomCandle(i,cColor);
            }
         }
      }
      return;
   }
   for(int i=ObjectsTotal(0,-1,-1)-1; i>=0; i--) {
      string name=ObjectName(0,i,-1,-1);
      if(StringFind(name,ea_prefix+"line_sig_")==0) ObjectDelete(0,name);
   }
   int activeDir = 0;
   if(showOBVfCandles) {
      for(int i=Bars-1; i>=2; i--) {
         if(state_TChan_OC[i] == 1.0 || state_TChan_OC[i] == -1.0) {
            activeDir = (int)state_TChan_OC[i];
         }
         if(activeDir != 0 && i > 0) {
            string name=ea_prefix+"line_sig_"+TimeToString(Time[i],TIME_DATE|TIME_MINUTES|TIME_SECONDS);
            color lineColor=(activeDir==1)?clrOrange:clrDodgerBlue;
            if(ObjectFind(0,name)<0) {
               ObjectCreate(0,name,OBJ_TREND,0,Time[i],Close[i],Time[i-1],Close[i-1]);
               ObjectSetInteger(0,name,OBJPROP_COLOR,lineColor);
               ObjectSetInteger(0,name,OBJPROP_WIDTH,1);
               ObjectSetInteger(0,name,OBJPROP_STYLE,STYLE_SOLID);
               ObjectSetInteger(0,name,OBJPROP_RAY_RIGHT,false);
               ObjectSetInteger(0,name,OBJPROP_SELECTABLE,false);
               ObjectSetInteger(0,name,OBJPROP_BACK,false);
               ObjectSetInteger(0,name,OBJPROP_ZORDER,1);
            }
         }
      }
   } else {
      for(int i=Bars-1; i>=2; i--) {
         if(LockBuffer[i] == 1 || LockBuffer[i] == -1) {
            activeDir = LockBuffer[i];
         }
         if(activeDir != 0 && i > 0) {
            string name=ea_prefix+"line_sig_"+TimeToString(Time[i],TIME_DATE|TIME_MINUTES|TIME_SECONDS);
            color lineColor=(activeDir==1)?clrOrange:clrDodgerBlue;
            if(ObjectFind(0,name)<0) {
               ObjectCreate(0,name,OBJ_TREND,0,Time[i],Close[i],Time[i-1],Close[i-1]);
               ObjectSetInteger(0,name,OBJPROP_COLOR,lineColor);
               ObjectSetInteger(0,name,OBJPROP_WIDTH,1);
               ObjectSetInteger(0,name,OBJPROP_STYLE,STYLE_SOLID);
               ObjectSetInteger(0,name,OBJPROP_RAY_RIGHT,false);
               ObjectSetInteger(0,name,OBJPROP_SELECTABLE,false);
               ObjectSetInteger(0,name,OBJPROP_BACK,false);
               ObjectSetInteger(0,name,OBJPROP_ZORDER,1);
            }
         }
      }
   }
}
void DrawNewestSignalBarSegment() {
   int activeDir = 0;
   if(showOBVfCandles) {
      for(int i=1; i<Bars; i++) {
         if(state_TChan_OC[i] == 1.0 || state_TChan_OC[i] == -1.0) { activeDir = (int)state_TChan_OC[i]; break; }
      }
   } else {
      for(int i=1; i<Bars; i++) {
         if(LockBuffer[i] == 1 || LockBuffer[i] == -1) { activeDir = LockBuffer[i]; break; }
      }
   }
   if(activeDir == 0) return;
   if(Chart_Visual_Mode==MODE_CANDLES) {
      color candleColor=(activeDir==1)?clrOrange:clrDodgerBlue;
      DrawCustomCandle(1,candleColor);
      return;
   }
   if(Bars<3) return;
   string name=ea_prefix+"line_sig_"+TimeToString(Time[1],TIME_DATE|TIME_MINUTES|TIME_SECONDS);
   color lineColor=(activeDir==1)?clrOrange:clrDodgerBlue;
   if(ObjectFind(0,name)<0) {
      ObjectCreate(0,name,OBJ_TREND,0,Time[2],Close[2],Time[1],Close[1]);
      ObjectSetInteger(0,name,OBJPROP_COLOR,lineColor);
      ObjectSetInteger(0,name,OBJPROP_WIDTH,1);
      ObjectSetInteger(0,name,OBJPROP_STYLE,STYLE_SOLID);
      ObjectSetInteger(0,name,OBJPROP_RAY_RIGHT,false);
      ObjectSetInteger(0,name,OBJPROP_SELECTABLE,false);
      ObjectSetInteger(0,name,OBJPROP_BACK,false);
      ObjectSetInteger(0,name,OBJPROP_ZORDER,1);
   }
}
void DrawLiveSignalBarSegment() {
   int activeDir = 0;
   if(showOBVfCandles) {
      for(int i=1; i<Bars; i++) {
         if(state_TChan_OC[i] == 1.0 || state_TChan_OC[i] == -1.0) { activeDir = (int)state_TChan_OC[i]; break; }
      }
   } else {
      for(int i=1; i<Bars; i++) {
         if(LockBuffer[i] == 1 || LockBuffer[i] == -1) { activeDir = LockBuffer[i]; break; }
      }
   }
   if(activeDir == 0) return;
   if(Chart_Visual_Mode==MODE_CANDLES) {
      color candleColor=(activeDir==1)?clrOrange:clrDodgerBlue;
      DrawCustomCandle(0,candleColor);
   }
}
bool IsVolumeInActiveTier(double vol) {
   if(Tier12_Vol>0.0 && Tier12_Mult>0.0 && vol>=Tier12_Vol) return true;
   if(Tier11_Vol>0.0 && Tier11_Mult>0.0 && vol>=Tier11_Vol) return true;
   if(Tier10_Vol>0.0 && Tier10_Mult>0.0 && vol>=Tier10_Vol) return true;
   if(Tier9_Vol>0.0 && Tier9_Mult>0.0 && vol>=Tier9_Vol) return true;
   if(Tier8_Vol>0.0 && Tier8_Mult>0.0 && vol>=Tier8_Vol) return true;
   if(Tier7_Vol>0.0 && Tier7_Mult>0.0 && vol>=Tier7_Vol) return true;
   if(Tier6_Vol>0.0 && Tier6_Mult>0.0 && vol>=Tier6_Vol) return true;
   if(Tier5_Vol>0.0 && Tier5_Mult>0.0 && vol>=Tier5_Vol) return true;
   if(Tier4_Vol>0.0 && Tier4_Mult>0.0 && vol>=Tier4_Vol) return true;
   if(Tier3_Vol>0.0 && Tier3_Mult>0.0 && vol>=Tier3_Vol) return true;
   if(Tier2_Vol>0.0 && Tier2_Mult>0.0 && vol>=Tier2_Vol) return true;
   if(Tier1_Vol>0.0 && Tier1_Mult>0.0 && vol>=Tier1_Vol) return true;
   return false;
}
void DeleteHarmonicVolumeObjects() {
   for(int i=ObjectsTotal(0,-1,-1)-1; i>=0; i--) {
      string name=ObjectName(0,i,-1,-1);
      if(StringFind(name,ea_prefix+"harm_")==0) {
         ObjectDelete(0,name);
      }
   }
}
void DrawHarmonicVolumeCandles() {
   if(!isHarmonicVolVisible) {
      DeleteHarmonicVolumeObjects();
      return;
   }
   int chartHeight=(int)ChartGetInteger(0,CHART_HEIGHT_IN_PIXELS);
   double p2p=0.0;
   if(chartHeight>0) p2p=(WindowPriceMax()-WindowPriceMin())/chartHeight;
   double offset=p2p*10.0;
   if(offset==0.0) offset=10.0*Point;
   int lastPaintedDirection = 0;
   for(int i=Bars-2; i>=1; i--) { 
      double vol=hist_VolumeValue[i];
      if(vol<=0.0 && Volume[i]>0) vol=(double)Volume[i];
      string suffix=TimeToString(Time[i],TIME_DATE|TIME_MINUTES|TIME_SECONDS);
      string bodyName=ea_prefix+"harm_c_body_"+suffix;
      string wickName=ea_prefix+"harm_c_wick_"+suffix;
      string triName=ea_prefix+"harm_tri_"+suffix;
      if(IsVolumeInActiveTier(vol)) {
         color paintColor;
         double triPrice;
         int arrowCode;
         int currentDir = 0;
         if(Close[i]>=Close[i+1]) {
            paintColor=clrGreen;
            arrowCode=233;
            triPrice=Low[i]-offset;
            currentDir = 1;
         } else {
            paintColor=clrRed;
            arrowCode=234;
            triPrice=High[i]+offset;
            currentDir = -1;
         }
         if(ObjectFind(0,wickName)<0) {
            ObjectCreate(0,wickName,OBJ_TREND,0,Time[i],High[i],Time[i],Low[i]);
            ObjectSetInteger(0,wickName,OBJPROP_WIDTH,1);
            ObjectSetInteger(0,wickName,OBJPROP_RAY,false);
            ObjectSetInteger(0,wickName,OBJPROP_BACK,false);
            ObjectSetInteger(0,wickName,OBJPROP_SELECTABLE,false);
         }
         ObjectSetInteger(0,wickName,OBJPROP_COLOR,paintColor);
         ObjectSetInteger(0,wickName,OBJPROP_TIME1,Time[i]);
         ObjectSetDouble(0,wickName,OBJPROP_PRICE1,High[i]);
         ObjectSetInteger(0,wickName,OBJPROP_TIME2,Time[i]);
         ObjectSetDouble(0,wickName,OBJPROP_PRICE2,Low[i]);
         if(ObjectFind(0,bodyName)<0) {
            ObjectCreate(0,bodyName,OBJ_TREND,0,Time[i],Open[i],Time[i],Close[i]);
            ObjectSetInteger(0,bodyName,OBJPROP_WIDTH,3);
            ObjectSetInteger(0,bodyName,OBJPROP_RAY,false);
            ObjectSetInteger(0,bodyName,OBJPROP_BACK,false);
            ObjectSetInteger(0,bodyName,OBJPROP_SELECTABLE,false);
         }
         ObjectSetInteger(0,bodyName,OBJPROP_COLOR,paintColor);
         ObjectSetInteger(0,bodyName,OBJPROP_TIME1,Time[i]);
         ObjectSetDouble(0,bodyName,OBJPROP_PRICE1,Open[i]);
         ObjectSetInteger(0,bodyName,OBJPROP_TIME2,Time[i]);
         ObjectSetDouble(0,bodyName,OBJPROP_PRICE2,Close[i]);
         if(currentDir != lastPaintedDirection) {
            if(ObjectFind(0,triName)<0) {
               ObjectCreate(0,triName,OBJ_ARROW,0,Time[i],triPrice);
               ObjectSetInteger(0,triName,OBJPROP_SELECTABLE,false);
               ObjectSetInteger(0,triName,OBJPROP_BACK,false);
               ObjectSetInteger(0,triName,OBJPROP_ZORDER,5);
            } else {
               ObjectSetInteger(0,triName,OBJPROP_TIME1,Time[i]);
               ObjectSetDouble(0,triName,OBJPROP_PRICE1,triPrice);
            }
            ObjectSetInteger(0,triName,OBJPROP_ARROWCODE,arrowCode);
            ObjectSetInteger(0,triName,OBJPROP_COLOR,paintColor);
            ObjectSetInteger(0,triName,OBJPROP_WIDTH,1);
            ObjectSetInteger(0,triName,OBJPROP_ANCHOR, (arrowCode==233)?ANCHOR_TOP:ANCHOR_BOTTOM);
            lastPaintedDirection = currentDir;
         } else {
            if(ObjectFind(0,triName)>=0) ObjectDelete(0,triName);
         }
      } else {
         if(ObjectFind(0,bodyName)>=0) ObjectDelete(0,bodyName);
         if(ObjectFind(0,wickName)>=0) ObjectDelete(0,wickName);
         if(ObjectFind(0,triName)>=0) ObjectDelete(0,triName);
      }
   }
}
void DeleteSignalMarkers() {
   string doublePrefix=ea_prefix+ea_prefix;
   for(int i=ObjectsTotal(0,-1,-1)-1; i>=0; i--) {
      string objName=ObjectName(0,i,-1,-1);
      if(StringFind(objName,"t2t_")>0) continue;
      if(StringFind(objName,ea_prefix+"sig_top_")==0||StringFind(objName,ea_prefix+"vline_")==0||StringFind(objName,doublePrefix+"sig_top_")==0) {
         ObjectDelete(0,objName);
      }
   }
}
void DeleteTrendFlipVLines() {
   string doublePrefix=ea_prefix+ea_prefix;
   for(int i=ObjectsTotal(0,-1,-1)-1; i>=0; i--) {
      string objName=ObjectName(0,i,-1,-1);
      if(StringFind(objName,ea_prefix+"trend_flip_vline_")==0||StringFind(objName,ea_prefix+"t2t_vline_")==0||StringFind(objName,ea_prefix+"t2t_sig_top_")==0||StringFind(objName,doublePrefix+"t2t_sig_top_")==0||StringFind(objName,ea_prefix+"decay_sig_st_")==0) {
         ObjectDelete(0,objName);
      }
   }
}
void UpdateSignalMarkerPositions() {
   if(!isSignalDotsVisible&&!isTrendVisualsVisible) {
      if(!isSignalDotsVisible) DeleteSignalMarkers();
      if(!isTrendVisualsVisible) DeleteTrendFlipVLines();
      return;
   }
   int chartHeight=(int)ChartGetInteger(0,CHART_HEIGHT_IN_PIXELS);
   if(chartHeight<=0) return;
   double p2p=(WindowPriceMax()-WindowPriceMin())/chartHeight;
   double d2d_topCoord=WindowPriceMax()-(45*p2p);
   double t2t_topCoord=WindowPriceMax()-(80*p2p);
   double priceRange=WindowPriceMax()-WindowPriceMin();
   double decayCoord=WindowPriceMax()-(priceRange*0.12);
   for(int i=ObjectsTotal(0,-1,-1)-1; i>=0; i--) {
      string objName=ObjectName(0,i,-1,-1);
      if(ObjectType(objName)!=OBJ_ARROW) continue;
      bool isSigTop=StringFind(objName,"sig_top_")>0&&StringFind(objName,"t2t_sig_top_")<0;
      bool isT2TSigTop=StringFind(objName,"t2t_sig_top_")>0;
      bool isDecaySig=StringFind(objName,"decay_sig_st_")>0;
      double currentPrice=ObjectGetDouble(0,objName,OBJPROP_PRICE1);
      if(isSigTop&&isSignalDotsVisible) {
         if(MathAbs(currentPrice-d2d_topCoord)>Point) ObjectSetDouble(0,objName,OBJPROP_PRICE1,d2d_topCoord);
      } else if(isT2TSigTop&&isTrendVisualsVisible) {
         if(MathAbs(currentPrice-t2t_topCoord)>Point) ObjectSetDouble(0,objName,OBJPROP_PRICE1,t2t_topCoord);
      } else if(isDecaySig&&isTrendVisualsVisible) {
         if(MathAbs(currentPrice-decayCoord)>Point) ObjectSetDouble(0,objName,OBJPROP_PRICE1,decayCoord);
      }
   }
}
void UpdateCustomTradeHistoryPositions() {
   double priceTop=WindowPriceMax();
   double priceBottom=WindowPriceMin();
   double priceRange=priceTop-priceBottom;
   double entryTopCoord=priceTop-priceRange*0.075;
   double entryBottomCoord=priceBottom+priceRange*0.10;
   double exitTopCoord=priceTop-priceRange*0.10;
   double exitBottomCoord=priceBottom+priceRange*0.10;
   for(int i=ObjectsTotal(0,-1,-1)-1; i>=0; i--) {
      string objName=ObjectName(0,i,-1,-1);
      if(ObjectType(objName)!=OBJ_ARROW) continue;
      double currentPrice=ObjectGetDouble(0,objName,OBJPROP_PRICE1);
      if(StringFind(objName,ea_prefix+"trade_open_top_")==0) {
         if(MathAbs(currentPrice-entryTopCoord)>Point) ObjectSetDouble(0,objName,OBJPROP_PRICE1,entryTopCoord);
      } else if(StringFind(objName,ea_prefix+"trade_open_bottom_")==0) {
         if(MathAbs(currentPrice-entryBottomCoord)>Point) ObjectSetDouble(0,objName,OBJPROP_PRICE1,entryBottomCoord);
      } else if(StringFind(objName,ea_prefix+"trade_close_top_")==0) {
         if(MathAbs(currentPrice-exitTopCoord)>Point) ObjectSetDouble(0,objName,OBJPROP_PRICE1,exitTopCoord);
      } else if(StringFind(objName,ea_prefix+"trade_close_bottom_")==0) {
         if(MathAbs(currentPrice-exitBottomCoord)>Point) ObjectSetDouble(0,objName,OBJPROP_PRICE1,exitBottomCoord);
      } else if(StringFind(objName,ea_prefix+"trade_obvfriend_close_top_")==0) {
         if(MathAbs(currentPrice-exitTopCoord)>Point) ObjectSetDouble(0,objName,OBJPROP_PRICE1,exitTopCoord);
      } else if(StringFind(objName,ea_prefix+"trade_obvfriend_close_bottom_")==0) {
         if(MathAbs(currentPrice-exitBottomCoord)>Point) ObjectSetDouble(0,objName,OBJPROP_PRICE1,exitBottomCoord);
      }
   }
}
void UpdateST_TrendDirectionIndicatorPositions() {
   if(!isTrendVisualsVisible) {
      DeleteST_TrendDirectionIndicator();
      return;
   }
   int chartHeight=(int)ChartGetInteger(0,CHART_HEIGHT_IN_PIXELS);
   if(chartHeight<=0) return;
   double p2p=(WindowPriceMax()-WindowPriceMin())/chartHeight;
   double topCoord=WindowPriceMax()-(32*p2p);
   for(int i=ObjectsTotal(0,-1,-1)-1; i>=0; i--) {
      string objName=ObjectName(0,i,-1,-1);
      if(ObjectType(objName)!=OBJ_TREND) continue;
      if(StringFind(objName,ea_prefix+"td_top_")==0) {
         double currentP1=ObjectGetDouble(0,objName,OBJPROP_PRICE1);
         if(MathAbs(currentP1-topCoord)>Point) {
            ObjectSetDouble(0,objName,OBJPROP_PRICE1,topCoord);
            ObjectSetDouble(0,objName,OBJPROP_PRICE2,topCoord);
         }
      }
   }
}
void UpdateLT_TrendDirectionIndicatorPositions() {
   if(!isTrendVisualsVisible) {
      DeleteLT_TrendDirectionIndicator();
      return;
   }
   int chartHeight=(int)ChartGetInteger(0,CHART_HEIGHT_IN_PIXELS);
   if(chartHeight<=0) return;
   double p2p=(WindowPriceMax()-WindowPriceMin())/chartHeight;
   double topCoord=WindowPriceMax()-(18*p2p);
   for(int i=ObjectsTotal(0,-1,-1)-1; i>=0; i--) {
      string objName=ObjectName(0,i,-1,-1);
      if(ObjectType(objName)!=OBJ_TREND) continue;
      if(StringFind(objName,ea_prefix+"lttd_top_")==0) {
         double currentP1=ObjectGetDouble(0,objName,OBJPROP_PRICE1);
         if(MathAbs(currentP1-topCoord)>Point) {
            ObjectSetDouble(0,objName,OBJPROP_PRICE1,topCoord);
            ObjectSetDouble(0,objName,OBJPROP_PRICE2,topCoord);
         }
      }
   }
}
void DrawLiveOBVfriendVLine(int index,int dir) {
   datetime signalTime=Time[index];
   color vlineColor=(dir==1)?C'146,134,124':C'89,116,124';
   string vlineName=ea_prefix+"obvfriend_vline_"+TimeToString(signalTime);
   if(ObjectFind(0,vlineName)<0) {
      ObjectCreate(0,vlineName,OBJ_VLINE,0,signalTime,0);
      ObjectSetInteger(0,vlineName,OBJPROP_COLOR,vlineColor);
      ObjectSetInteger(0,vlineName,OBJPROP_STYLE,STYLE_DOT);
      ObjectSetInteger(0,vlineName,OBJPROP_WIDTH,1);
      ObjectSetInteger(0,vlineName,OBJPROP_BACK,true);
      ObjectSetInteger(0,vlineName,OBJPROP_SELECTABLE,false);
   }
}
//+------------------------------------------------------------------+
//| SECTION 7.1 - SUPERTREND VISUALS                                 |
//+------------------------------------------------------------------+
void DrawSuperTrendLine() {
   const int Z_ORDER_CHART_DECORATION=0;
   for(int i=Bars-2; i>=1; i--) {
      if(SuperTrend[i]==0.0||SuperTrend[i+1]==0.0||SuperTrend[i]==EMPTY_VALUE||SuperTrend[i+1]==EMPTY_VALUE) continue;
      string lineName=ea_prefix+"st_line_"+TimeToString(Time[i],TIME_DATE|TIME_MINUTES|TIME_SECONDS);
      if(ObjectFind(0,lineName)<0) {
         if(!ObjectCreate(0,lineName,OBJ_TREND,0,Time[i+1],SuperTrend[i+1],Time[i],SuperTrend[i])) continue;
         color stColor=(Direction[i]==1)?clrOrange:clrDodgerBlue;
         ObjectSetInteger(0,lineName,OBJPROP_COLOR,stColor);
         ObjectSetInteger(0,lineName,OBJPROP_WIDTH,1);
         ObjectSetInteger(0,lineName,OBJPROP_STYLE,STYLE_DOT);
         ObjectSetInteger(0,lineName,OBJPROP_RAY_RIGHT,false);
         ObjectSetInteger(0,lineName,OBJPROP_SELECTABLE,false);
         ObjectSetInteger(0,lineName,OBJPROP_BACK,true);
         ObjectSetInteger(0,lineName,OBJPROP_ZORDER,Z_ORDER_CHART_DECORATION);
      } else {
         ObjectSetInteger(0,lineName,OBJPROP_TIME1,Time[i+1]);
         ObjectSetDouble(0,lineName,OBJPROP_PRICE1,SuperTrend[i+1]);
         ObjectSetInteger(0,lineName,OBJPROP_TIME2,Time[i]);
         ObjectSetDouble(0,lineName,OBJPROP_PRICE2,SuperTrend[i]);
      }
   }
}
void DeleteSuperTrendLine() {
   for(int i=ObjectsTotal()-1; i>=0; i--) {
      string name=ObjectName(0,i,-1,-1);
      if(StringFind(name,ea_prefix+"st_line_")==0) ObjectDelete(0,name);
   }
}
void DrawOBVfriendSuperTrendLine() {
   for(int i=Bars-2; i>=1; i--) {
      if(OBVfriend_SuperTrend[i]==0.0||OBVfriend_SuperTrend[i+1]==0.0||OBVfriend_SuperTrend[i]==EMPTY_VALUE||OBVfriend_SuperTrend[i+1]==EMPTY_VALUE) continue;
      double pixel_per_price=(double)ChartGetInteger(0,CHART_HEIGHT_IN_PIXELS)/(WindowPriceMax()-WindowPriceMin());
      double padding_in_price=0.0;
      if(pixel_per_price>0) padding_in_price=10.0/pixel_per_price;
      double val1=OBVfriend_SuperTrend[i+1];
      double val2=OBVfriend_SuperTrend[i];
      if(OBVfriend_Direction[i+1]==1) val1-=padding_in_price;
      else val1+=padding_in_price;
      if(OBVfriend_Direction[i]==1) val2-=padding_in_price;
      else val2+=padding_in_price;
      string lineName=ea_prefix+"obvfriend_st_line_"+TimeToString(Time[i],TIME_DATE|TIME_MINUTES|TIME_SECONDS);
      if(ObjectFind(0,lineName)<0) {
         ObjectCreate(0,lineName,OBJ_TREND,0,Time[i+1],val1,Time[i],val2);
         color stColor=(OBVfriend_Direction[i]==1)?C'146,134,124':C'89,116,124';
         ObjectSetInteger(0,lineName,OBJPROP_COLOR,stColor);
         ObjectSetInteger(0,lineName,OBJPROP_WIDTH,1);
         ObjectSetInteger(0,lineName,OBJPROP_STYLE,STYLE_DOT);
         ObjectSetInteger(0,lineName,OBJPROP_RAY_RIGHT,false);
         ObjectSetInteger(0,lineName,OBJPROP_SELECTABLE,false);
         ObjectSetInteger(0,lineName,OBJPROP_BACK,true);
      } else {
         ObjectSetInteger(0,lineName,OBJPROP_TIME1,Time[i+1]);
         ObjectSetDouble(0,lineName,OBJPROP_PRICE1,val1);
         ObjectSetInteger(0,lineName,OBJPROP_TIME2,Time[i]);
         ObjectSetDouble(0,lineName,OBJPROP_PRICE2,val2);
      }
   }
}
void DeleteOBVfriendSuperTrendLine() {
   for(int i=ObjectsTotal()-1; i>=0; i--) {
      string name=ObjectName(0,i,-1,-1);
      if(StringFind(name,ea_prefix+"obvfriend_st_line_")==0) ObjectDelete(0,name);
   }
}
void DrawNewestSuperTrendSegment() {
   if(Bars<3||SuperTrend[1]==0.0||SuperTrend[2]==0.0||SuperTrend[1]==EMPTY_VALUE||SuperTrend[2]==EMPTY_VALUE) return;
   const int Z_ORDER_CHART_DECORATION=0;
   string lineName=ea_prefix+"st_line_"+TimeToString(Time[1],TIME_DATE|TIME_MINUTES|TIME_SECONDS);
   color stColor=(Direction[1]==1)?clrOrange:clrDodgerBlue;
   if(ObjectFind(0,lineName)<0) {
      ObjectCreate(0,lineName,OBJ_TREND,0,Time[2],SuperTrend[2],Time[1],SuperTrend[1]);
      ObjectSetInteger(0,lineName,OBJPROP_COLOR,stColor);
      ObjectSetInteger(0,lineName,OBJPROP_WIDTH,1);
      ObjectSetInteger(0,lineName,OBJPROP_STYLE,STYLE_DOT);
      ObjectSetInteger(0,lineName,OBJPROP_RAY_RIGHT,false);
      ObjectSetInteger(0,lineName,OBJPROP_SELECTABLE,false);
      ObjectSetInteger(0,lineName,OBJPROP_BACK,true);
      ObjectSetInteger(0,lineName,OBJPROP_ZORDER,Z_ORDER_CHART_DECORATION);
   } else {
      ObjectSetInteger(0,lineName,OBJPROP_TIME1,Time[2]);
      ObjectSetDouble(0,lineName,OBJPROP_PRICE1,SuperTrend[2]);
      ObjectSetInteger(0,lineName,OBJPROP_TIME2,Time[1]);
      ObjectSetDouble(0,lineName,OBJPROP_PRICE2,SuperTrend[1]);
   }
}
void DrawNewestOBVfriendSuperTrendSegment() {
   if(Bars<3||OBVfriend_SuperTrend[1]==0.0||OBVfriend_SuperTrend[2]==0.0||OBVfriend_SuperTrend[1]==EMPTY_VALUE||OBVfriend_SuperTrend[2]==EMPTY_VALUE) return;
   double pixel_per_price=(double)ChartGetInteger(0,CHART_HEIGHT_IN_PIXELS)/(WindowPriceMax()-WindowPriceMin());
   double padding_in_price=0.0;
   if(pixel_per_price>0) padding_in_price=10.0/pixel_per_price;
   double val1=OBVfriend_SuperTrend[2];
   double val2=OBVfriend_SuperTrend[1];
   if(OBVfriend_Direction[2]==1) val1-=padding_in_price;
   else val1+=padding_in_price;
   if(OBVfriend_Direction[1]==1) val2-=padding_in_price;
   else val2+=padding_in_price;
   string lineName=ea_prefix+"obvfriend_st_line_"+TimeToString(Time[1],TIME_DATE|TIME_MINUTES|TIME_SECONDS);
   color stColor=(OBVfriend_Direction[1]==1)?C'146,134,124':C'89,116,124';
   if(ObjectFind(0,lineName)<0) {
      ObjectCreate(0,lineName,OBJ_TREND,0,Time[2],val1,Time[1],val2);
      ObjectSetInteger(0,lineName,OBJPROP_COLOR,stColor);
      ObjectSetInteger(0,lineName,OBJPROP_WIDTH,1);
      ObjectSetInteger(0,lineName,OBJPROP_STYLE,STYLE_DOT);
      ObjectSetInteger(0,lineName,OBJPROP_RAY_RIGHT,false);
      ObjectSetInteger(0,lineName,OBJPROP_SELECTABLE,false);
      ObjectSetInteger(0,lineName,OBJPROP_BACK,true);
   } else {
      ObjectSetInteger(0,lineName,OBJPROP_TIME1,Time[2]);
      ObjectSetDouble(0,lineName,OBJPROP_PRICE1,val1);
      ObjectSetInteger(0,lineName,OBJPROP_TIME2,Time[1]);
      ObjectSetDouble(0,lineName,OBJPROP_PRICE2,val2);
   }
}
void DrawLiveSuperTrendSegment() {
   if(Bars<2||SuperTrend[0]==0.0||SuperTrend[1]==0.0||SuperTrend[0]==EMPTY_VALUE||SuperTrend[1]==EMPTY_VALUE) return;
   const int Z_ORDER_CHART_DECORATION=0;
   string lineName=ea_prefix+"st_line_LIVE";
   color stColor=(Direction[0]==1)?clrOrange:clrDodgerBlue;
   if(ObjectFind(0,lineName)<0) {
      ObjectCreate(0,lineName,OBJ_TREND,0,Time[1],SuperTrend[1],Time[0],SuperTrend[0]);
      ObjectSetInteger(0,lineName,OBJPROP_COLOR,stColor);
      ObjectSetInteger(0,lineName,OBJPROP_WIDTH,1);
      ObjectSetInteger(0,lineName,OBJPROP_STYLE,STYLE_DOT);
      ObjectSetInteger(0,lineName,OBJPROP_RAY_RIGHT,false);
      ObjectSetInteger(0,lineName,OBJPROP_SELECTABLE,false);
      ObjectSetInteger(0,lineName,OBJPROP_BACK,true);
      ObjectSetInteger(0,lineName,OBJPROP_ZORDER,Z_ORDER_CHART_DECORATION);
   } else {
      ObjectSetInteger(0,lineName,OBJPROP_TIME1,Time[1]);
      ObjectSetDouble(0,lineName,OBJPROP_PRICE1,SuperTrend[1]);
      ObjectSetInteger(0,lineName,OBJPROP_TIME2,Time[0]);
      ObjectSetDouble(0,lineName,OBJPROP_PRICE2,SuperTrend[0]);
      ObjectSetInteger(0,lineName,OBJPROP_COLOR,stColor);
   }
}
void DrawLiveOBVfriendSuperTrendSegment() {
   if(Bars<2||OBVfriend_SuperTrend[0]==0.0||OBVfriend_SuperTrend[1]==0.0||OBVfriend_SuperTrend[0]==EMPTY_VALUE||OBVfriend_SuperTrend[1]==EMPTY_VALUE) return;
   double pixel_per_price=(double)ChartGetInteger(0,CHART_HEIGHT_IN_PIXELS)/(WindowPriceMax()-WindowPriceMin());
   double padding_in_price=0.0;
   if(pixel_per_price>0) padding_in_price=10.0/pixel_per_price;
   double val1=OBVfriend_SuperTrend[1];
   double val2=OBVfriend_SuperTrend[0];
   string prevLineName=ea_prefix+"obvfriend_st_line_"+TimeToString(Time[1],TIME_DATE|TIME_MINUTES|TIME_SECONDS);
   if(ObjectFind(0,prevLineName)>=0) {
      val1=ObjectGetDouble(0,prevLineName,OBJPROP_PRICE2);
   } else {
      if(OBVfriend_Direction[1]==1) val1-=padding_in_price;
      else val1+=padding_in_price;
   }
   if(OBVfriend_Direction[0]==1) val2-=padding_in_price;
   else val2+=padding_in_price;
   string lineName=ea_prefix+"obvfriend_st_line_LIVE";
   color stColor=(OBVfriend_Direction[0]==1)?C'146,134,124':C'89,116,124';
   if(ObjectFind(0,lineName)<0) {
      ObjectCreate(0,lineName,OBJ_TREND,0,Time[1],val1,Time[0],val2);
      ObjectSetInteger(0,lineName,OBJPROP_COLOR,stColor);
      ObjectSetInteger(0,lineName,OBJPROP_WIDTH,1);
      ObjectSetInteger(0,lineName,OBJPROP_STYLE,STYLE_DOT);
      ObjectSetInteger(0,lineName,OBJPROP_RAY_RIGHT,false);
      ObjectSetInteger(0,lineName,OBJPROP_SELECTABLE,false);
      ObjectSetInteger(0,lineName,OBJPROP_BACK,true);
   } else {
      ObjectSetInteger(0,lineName,OBJPROP_TIME1,Time[1]);
      ObjectSetDouble(0,lineName,OBJPROP_PRICE1,val1);
      ObjectSetInteger(0,lineName,OBJPROP_TIME2,Time[0]);
      ObjectSetDouble(0,lineName,OBJPROP_PRICE2,val2);
      ObjectSetInteger(0,lineName,OBJPROP_COLOR,stColor);
   }
}
void SyncLiveSuperTrendVisuals() {
   if(!isSuperTrendVisible&&!isOBVfriendSuperTrendVisible) return;
   for(int i=1; i<=2; i++) {
      if(i+1>=Bars) continue;
      if(isSuperTrendVisible) {
         string lineName=ea_prefix+"st_line_"+TimeToString(Time[i],TIME_DATE|TIME_MINUTES|TIME_SECONDS);
         if(SuperTrend[i]!=0.0&&SuperTrend[i+1]!=0.0&&SuperTrend[i]!=EMPTY_VALUE&&SuperTrend[i+1]!=EMPTY_VALUE) {
            color stColor=(Direction[i]==1)?clrOrange:clrDodgerBlue;
            if(ObjectFind(0,lineName)<0) {
               ObjectCreate(0,lineName,OBJ_TREND,0,Time[i+1],SuperTrend[i+1],Time[i],SuperTrend[i]);
               ObjectSetInteger(0,lineName,OBJPROP_COLOR,stColor);
               ObjectSetInteger(0,lineName,OBJPROP_WIDTH,1);
               ObjectSetInteger(0,lineName,OBJPROP_STYLE,STYLE_DOT);
               ObjectSetInteger(0,lineName,OBJPROP_RAY_RIGHT,false);
               ObjectSetInteger(0,lineName,OBJPROP_SELECTABLE,false);
               ObjectSetInteger(0,lineName,OBJPROP_BACK,true);
               ObjectSetInteger(0,lineName,OBJPROP_ZORDER,0);
            } else {
               ObjectSetInteger(0,lineName,OBJPROP_TIME1,Time[i+1]);
               ObjectSetDouble(0,lineName,OBJPROP_PRICE1,SuperTrend[i+1]);
               ObjectSetInteger(0,lineName,OBJPROP_TIME2,Time[i]);
               ObjectSetDouble(0,lineName,OBJPROP_PRICE2,SuperTrend[i]);
               ObjectSetInteger(0,lineName,OBJPROP_COLOR,stColor);
               ObjectSetInteger(0,lineName,OBJPROP_STYLE,STYLE_DOT);
            }
         } else {
            if(ObjectFind(0,lineName)>=0) ObjectDelete(0,lineName);
         }
      }
      if(isOBVfriendSuperTrendVisible) {
         string lineName=ea_prefix+"obvfriend_st_line_"+TimeToString(Time[i],TIME_DATE|TIME_MINUTES|TIME_SECONDS);
         if(OBVfriend_SuperTrend[i]!=0.0&&OBVfriend_SuperTrend[i+1]!=0.0&&OBVfriend_SuperTrend[i]!=EMPTY_VALUE&&OBVfriend_SuperTrend[i+1]!=EMPTY_VALUE) {
            double pixel_per_price=(double)ChartGetInteger(0,CHART_HEIGHT_IN_PIXELS)/(WindowPriceMax()-WindowPriceMin());
            double padding_in_price=0.0;
            if(pixel_per_price>0) padding_in_price=10.0/pixel_per_price;
            double val1=OBVfriend_SuperTrend[i+1];
            double val2=OBVfriend_SuperTrend[i];
            string prevLineName=ea_prefix+"obvfriend_st_line_"+TimeToString(Time[i+1],TIME_DATE|TIME_MINUTES|TIME_SECONDS);
            if(ObjectFind(0,prevLineName)>=0) {
               val1=ObjectGetDouble(0,prevLineName,OBJPROP_PRICE2);
            } else {
               if(OBVfriend_Direction[i+1]==1) val1-=padding_in_price;
               else val1+=padding_in_price;
            }
            if(OBVfriend_Direction[i]==1) val2-=padding_in_price;
            else val2+=padding_in_price;
            color stColor=(OBVfriend_Direction[i]==1)?C'146,134,124':C'89,116,124';
            if(ObjectFind(0,lineName)<0) {
               ObjectCreate(0,lineName,OBJ_TREND,0,Time[i+1],val1,Time[i],val2);
               ObjectSetInteger(0,lineName,OBJPROP_COLOR,stColor);
               ObjectSetInteger(0,lineName,OBJPROP_WIDTH,1);
               ObjectSetInteger(0,lineName,OBJPROP_STYLE,STYLE_DOT);
               ObjectSetInteger(0,lineName,OBJPROP_RAY_RIGHT,false);
               ObjectSetInteger(0,lineName,OBJPROP_SELECTABLE,false);
               ObjectSetInteger(0,lineName,OBJPROP_BACK,true);
            } else {
               ObjectSetInteger(0,lineName,OBJPROP_TIME1,Time[i+1]);
               ObjectSetDouble(0,lineName,OBJPROP_PRICE1,val1);
               ObjectSetInteger(0,lineName,OBJPROP_TIME2,Time[i]);
               ObjectSetDouble(0,lineName,OBJPROP_PRICE2,val2);
               ObjectSetInteger(0,lineName,OBJPROP_COLOR,stColor);
               ObjectSetInteger(0,lineName,OBJPROP_STYLE,STYLE_DOT);
            }
         } else {
            if(ObjectFind(0,lineName)>=0) ObjectDelete(0,lineName);
         }
      }
   }
}
//+------------------------------------------------------------------+
//| SECTION 7.2 - CUSTOM TRADE HISTORY VISUALS                       |
//+------------------------------------------------------------------+
void DrawCustomTradeHistory() {
   if(!isTradeHistoryVisible) {
      DeleteTradeHistoryObjects();
      return;
   }
   for(int i=0; i<OrdersHistoryTotal(); i++) {
      if(!OrderSelect(i,SELECT_BY_POS,MODE_HISTORY)) continue;
      if(OrderMagicNumber()!=MagicNumber && OrderMagicNumber()!=OBVfriendMagicNumber) continue;
      int ticket=OrderTicket();
      bool isDrawn=false;
      for(int j=0; j<ArraySize(drawnHistoryTickets); j++) {
         if(ticket==drawnHistoryTickets[j]) { isDrawn=true; break; }
      }
      if(isDrawn) continue;
      datetime openTime=OrderOpenTime(); datetime closeTime=OrderCloseTime();
      double priceTop=WindowPriceMax(); double priceBottom=WindowPriceMin(); double priceRange=priceTop-priceBottom;
      if(OrderMagicNumber()==MagicNumber) {
         color entryColor=(OrderType()==OP_BUY)?C'146,134,124':C'89,116,124';
         double entryTopCoord=priceTop-priceRange*0.075; double entryBottomCoord=priceBottom+priceRange*0.075;
         string openMarkerTop=ea_prefix+"trade_open_top_"+IntegerToString(ticket);
         ObjectCreate(0,openMarkerTop,OBJ_ARROW,0,openTime,entryTopCoord);
         ObjectSetInteger(0,openMarkerTop,OBJPROP_ARROWCODE,(OrderType()==OP_BUY)?241:242);
         ObjectSetInteger(0,openMarkerTop,OBJPROP_COLOR,entryColor);
         ObjectSetInteger(0,openMarkerTop,OBJPROP_BACK,true);
         string openMarkerBottom=ea_prefix+"trade_open_bottom_"+IntegerToString(ticket);
         ObjectCreate(0,openMarkerBottom,OBJ_ARROW,0,openTime,entryBottomCoord);
         ObjectSetInteger(0,openMarkerBottom,OBJPROP_ARROWCODE,(OrderType()==OP_BUY)?241:242);
         ObjectSetInteger(0,openMarkerBottom,OBJPROP_COLOR,entryColor);
         ObjectSetInteger(0,openMarkerBottom,OBJPROP_BACK,true);
         string vlineName=ea_prefix+"trade_close_vline_"+IntegerToString(ticket);
         ObjectCreate(0,vlineName,OBJ_VLINE,0,closeTime,0);
         ObjectSetInteger(0,vlineName,OBJPROP_COLOR,clrGray);
         ObjectSetInteger(0,vlineName,OBJPROP_STYLE,STYLE_DOT);
         ObjectSetInteger(0,vlineName,OBJPROP_WIDTH,1);
         ObjectSetInteger(0,vlineName,OBJPROP_BACK,true);
         double exitTopCoord=priceTop-priceRange*0.10; double exitBottomCoord=priceBottom+priceRange*0.10;
         string closeMarkerTop=ea_prefix+"trade_close_top_"+IntegerToString(ticket);
         ObjectCreate(0,closeMarkerTop,OBJ_ARROW,0,closeTime,exitTopCoord);
         ObjectSetInteger(0,closeMarkerTop,OBJPROP_ARROWCODE,108);
         ObjectSetInteger(0,closeMarkerTop,OBJPROP_COLOR,clrRed);
         ObjectSetInteger(0,closeMarkerTop,OBJPROP_BACK,true);
         string closeMarkerBottom=ea_prefix+"trade_close_bottom_"+IntegerToString(ticket);
         ObjectCreate(0,closeMarkerBottom,OBJ_ARROW,0,closeTime,exitBottomCoord);
         ObjectSetInteger(0,closeMarkerBottom,OBJPROP_ARROWCODE,108);
         ObjectSetInteger(0,closeMarkerBottom,OBJPROP_COLOR,clrRed);
         ObjectSetInteger(0,closeMarkerBottom,OBJPROP_BACK,true);
      } else if(OrderMagicNumber()==OBVfriendMagicNumber) {
         color obvfriendEntryColor=(OrderType()==OP_BUY)?C'146,134,124':C'89,116,124';
         double entryTopCoord=priceTop-priceRange*0.075; double entryBottomCoord=priceBottom+priceRange*0.075;
         string openVlineName=ea_prefix+"trade_obvfriend_open_vline_"+IntegerToString(ticket);
         ObjectCreate(0,openVlineName,OBJ_VLINE,0,openTime,0);
         ObjectSetInteger(0,openVlineName,OBJPROP_COLOR,obvfriendEntryColor);
         ObjectSetInteger(0,openVlineName,OBJPROP_STYLE,STYLE_SOLID);
         ObjectSetInteger(0,openVlineName,OBJPROP_WIDTH,1);
         ObjectSetInteger(0,openVlineName,OBJPROP_BACK,true);
         string openMarkerTop=ea_prefix+"trade_obvfriend_open_top_"+IntegerToString(ticket);
         ObjectCreate(0,openMarkerTop,OBJ_ARROW,0,openTime,entryTopCoord);
         ObjectSetInteger(0,openMarkerTop,OBJPROP_ARROWCODE,(OrderType()==OP_BUY)?241:242);
         ObjectSetInteger(0,openMarkerTop,OBJPROP_COLOR,obvfriendEntryColor);
         ObjectSetInteger(0,openMarkerTop,OBJPROP_BACK,true);
         string openMarkerBottom=ea_prefix+"trade_obvfriend_open_bottom_"+IntegerToString(ticket);
         ObjectCreate(0,openMarkerBottom,OBJ_ARROW,0,openTime,entryBottomCoord);
         ObjectSetInteger(0,openMarkerBottom,OBJPROP_ARROWCODE,(OrderType()==OP_BUY)?241:242);
         ObjectSetInteger(0,openMarkerBottom,OBJPROP_COLOR,obvfriendEntryColor);
         ObjectSetInteger(0,openMarkerBottom,OBJPROP_BACK,true);
         string vlineName=ea_prefix+"trade_obvfriend_close_vline_"+IntegerToString(ticket);
         ObjectCreate(0,vlineName,OBJ_VLINE,0,closeTime,0);
         ObjectSetInteger(0,vlineName,OBJPROP_COLOR,clrWhite);
         ObjectSetInteger(0,vlineName,OBJPROP_STYLE,STYLE_SOLID);
         ObjectSetInteger(0,vlineName,OBJPROP_WIDTH,1);
         ObjectSetInteger(0,vlineName,OBJPROP_BACK,true);
         double exitTopCoord=priceTop-priceRange*0.10; double exitBottomCoord=priceBottom+priceRange*0.10;
         string closeMarkerTop=ea_prefix+"trade_obvfriend_close_top_"+IntegerToString(ticket);
         ObjectCreate(0,closeMarkerTop,OBJ_ARROW,0,closeTime,exitTopCoord);
         ObjectSetInteger(0,closeMarkerTop,OBJPROP_ARROWCODE,108);
         ObjectSetInteger(0,closeMarkerTop,OBJPROP_COLOR,clrRed);
         ObjectSetInteger(0,closeMarkerTop,OBJPROP_BACK,true);
         string closeMarkerBottom=ea_prefix+"trade_obvfriend_close_bottom_"+IntegerToString(ticket);
         ObjectCreate(0,closeMarkerBottom,OBJ_ARROW,0,closeTime,exitBottomCoord);
         ObjectSetInteger(0,closeMarkerBottom,OBJPROP_ARROWCODE,108);
         ObjectSetInteger(0,closeMarkerBottom,OBJPROP_COLOR,clrRed);
         ObjectSetInteger(0,closeMarkerBottom,OBJPROP_BACK,true);
      }
      int last=ArraySize(drawnHistoryTickets);
      ArrayResize(drawnHistoryTickets,last+1);
      drawnHistoryTickets[last]=ticket;
   }
}
void DeleteTradeHistoryObjects() {
   for(int i=ObjectsTotal(0,-1,-1)-1; i>=0; i--) {
      string objName=ObjectName(0,i,-1,-1);
      if(StringFind(objName,ea_prefix+"trade_")==0) ObjectDelete(0,objName);
   }
   ArrayResize(drawnHistoryTickets,0);
}
//+------------------------------------------------------------------+
//| SECTION 7.3 - ADAPTIVE TREND VISUALS                             |
//+------------------------------------------------------------------+
void DeleteST_AdaptiveTrendChannel() {
   ObjectDelete(0,ea_prefix+"at_line_upper");
   ObjectDelete(0,ea_prefix+"at_line_lower");
   ObjectDelete(0,ea_prefix+"at_line_mid");
}
void DeleteLT_AdaptiveTrendChannel() {
   ObjectDelete(0,ea_prefix+"lt_at_line_upper");
   ObjectDelete(0,ea_prefix+"lt_at_line_lower");
   ObjectDelete(0,ea_prefix+"lt_at_line_mid");
}
void DrawST_AdaptiveTrendChannel() {
   if(!isTrendVisualsVisible) { DeleteST_AdaptiveTrendChannel(); return; }
   if(detectedAnchorBar_ST<=1||detectedAnchorBar_ST>Bars-1||Close[1]<=0.0) return;
   int length=detectedAnchorBar_ST;
   double sumW=0.0,sumWX=0.0,sumWY=0.0,sumWXY=0.0,sumWXX=0.0;
   int points=0;
   for(int k=0; k<length; k++) {
      int bar_idx=1+k;
      if(bar_idx>=Bars) break;
      double val=MathLog(Close[bar_idx]);
      double x=(double)k+1.0;
      double weight=(double)(length-k)/(double)length;
      sumW+=weight; sumWX+=weight*x; sumWY+=weight*val; sumWXY+=weight*x*val; sumWXX+=weight*x*x;
      points++;
   }
   if(points<2) return;
   double denomW=(sumW*sumWXX-sumWX*sumWX);
   if(denomW==0.0) return;
   double slope=(sumW*sumWXY-sumWX*sumWY)/denomW;
   double intercept=(sumWY/sumW)-slope*(sumWX/sumW)+slope;
   double max_dev=0.0;
   for(int k=0; k<length; k++) {
      int bar_idx=1+k;
      if(bar_idx>=Bars) break;
      double reg_y=intercept+slope*(double)k;
      double h_log=MathLog(High[bar_idx]);
      double l_log=MathLog(Low[bar_idx]);
      double dev_h=MathAbs(h_log-reg_y);
      double dev_l=MathAbs(l_log-reg_y);
      max_dev=MathMax(max_dev,MathMax(dev_h,dev_l));
   }
   double mid_start_log=intercept+slope*(double)(points-1);
   double mid_end_log=intercept;
   double up_start=MathExp(mid_start_log+max_dev);
   double up_end=MathExp(mid_end_log+max_dev);
   double dn_start=MathExp(mid_start_log-max_dev);
   double dn_end=MathExp(mid_end_log-max_dev);
   double mid_start=MathExp(mid_start_log);
   double mid_end=MathExp(mid_end_log);
   datetime t_start=Time[detectedAnchorBar_ST];
   datetime t_end=Time[1];
   color channelColor=(slope>0.0)?C'89,116,124':C'146,134,124';
   string upName=ea_prefix+"at_line_upper";
   if(ObjectFind(0,upName)<0) {
      ObjectCreate(0,upName,OBJ_TREND,0,t_start,up_start,t_end,up_end);
      ObjectSetInteger(0,upName,OBJPROP_COLOR,channelColor);
      ObjectSetInteger(0,upName,OBJPROP_WIDTH,1);
      ObjectSetInteger(0,upName,OBJPROP_STYLE,STYLE_SOLID);
      ObjectSetInteger(0,upName,OBJPROP_RAY_RIGHT,true);
      ObjectSetInteger(0,upName,OBJPROP_SELECTABLE,false);
      ObjectSetInteger(0,upName,OBJPROP_BACK,true);
   } else {
      ObjectSetInteger(0,upName,OBJPROP_TIME1,t_start);
      ObjectSetDouble(0,upName,OBJPROP_PRICE1,up_start);
      ObjectSetInteger(0,upName,OBJPROP_TIME2,t_end);
      ObjectSetDouble(0,upName,OBJPROP_PRICE2,up_end);
      ObjectSetInteger(0,upName,OBJPROP_COLOR,channelColor);
   }
   string dnName=ea_prefix+"at_line_lower";
   if(ObjectFind(0,dnName)<0) {
      ObjectCreate(0,dnName,OBJ_TREND,0,t_start,dn_start,t_end,dn_end);
      ObjectSetInteger(0,dnName,OBJPROP_COLOR,channelColor);
      ObjectSetInteger(0,dnName,OBJPROP_WIDTH,1);
      ObjectSetInteger(0,dnName,OBJPROP_STYLE,STYLE_SOLID);
      ObjectSetInteger(0,dnName,OBJPROP_RAY_RIGHT,true);
      ObjectSetInteger(0,dnName,OBJPROP_SELECTABLE,false);
      ObjectSetInteger(0,dnName,OBJPROP_BACK,true);
   } else {
      ObjectSetInteger(0,dnName,OBJPROP_TIME1,t_start);
      ObjectSetDouble(0,dnName,OBJPROP_PRICE1,dn_start);
      ObjectSetInteger(0,dnName,OBJPROP_TIME2,t_end);
      ObjectSetDouble(0,dnName,OBJPROP_PRICE2,dn_end);
      ObjectSetInteger(0,dnName,OBJPROP_COLOR,channelColor);
   }
   string midName=ea_prefix+"at_line_mid";
   if(ObjectFind(0,midName)<0) {
      ObjectCreate(0,midName,OBJ_TREND,0,t_start,mid_start,t_end,mid_end);
      ObjectSetInteger(0,midName,OBJPROP_COLOR,channelColor);
      ObjectSetInteger(0,midName,OBJPROP_WIDTH,1);
      ObjectSetInteger(0,midName,OBJPROP_STYLE,STYLE_DOT);
      ObjectSetInteger(0,midName,OBJPROP_RAY_RIGHT,true);
      ObjectSetInteger(0,midName,OBJPROP_SELECTABLE,false);
      ObjectSetInteger(0,midName,OBJPROP_BACK,true);
   } else {
      ObjectSetInteger(0,midName,OBJPROP_TIME1,t_start);
      ObjectSetDouble(0,midName,OBJPROP_PRICE1,mid_start);
      ObjectSetInteger(0,midName,OBJPROP_TIME2,t_end);
      ObjectSetDouble(0,midName,OBJPROP_PRICE2,mid_end);
      ObjectSetInteger(0,midName,OBJPROP_COLOR,channelColor);
   }
}
void DrawLT_AdaptiveTrendChannel() {
   if(!isTrendVisualsVisible) { DeleteLT_AdaptiveTrendChannel(); return; }
   if(LT_detectedAnchorBar_ST<=1||LT_detectedAnchorBar_ST>Bars-1||Close[1]<=0.0) return;
   int length=LT_detectedAnchorBar_ST;
   double sumW=0.0,sumWX=0.0,sumWY=0.0,sumWXY=0.0,sumWXX=0.0;
   int points=0;
   for(int k=0; k<length; k++) {
      int bar_idx=1+k;
      if(bar_idx>=Bars) break;
      double val=MathLog(Close[bar_idx]);
      double x=(double)k+1.0;
      double weight=(double)(length-k)/(double)length;
      sumW+=weight; sumWX+=weight*x; sumWY+=weight*val; sumWXY+=weight*x*val; sumWXX+=weight*x*x;
      points++;
   }
   if(points<2) return;
   double denomW=(sumW*sumWXX-sumWX*sumWX);
   if(denomW==0.0) return;
   double slope=(sumW*sumWXY-sumWX*sumWY)/denomW;
   double intercept=(sumWY/sumW)-slope*(sumWX/sumW)+slope;
   double max_dev=0.0;
   for(int k=0; k<length; k++) {
      int bar_idx=1+k;
      if(bar_idx>=Bars) break;
      double reg_y=intercept+slope*(double)k;
      double h_log=MathLog(High[bar_idx]);
      double l_log=MathLog(Low[bar_idx]);
      double dev_h=MathAbs(h_log-reg_y);
      double dev_l=MathAbs(l_log-reg_y);
      max_dev=MathMax(max_dev,MathMax(dev_h,dev_l));
   }
   double mid_start_log=intercept+slope*(double)(points-1);
   double mid_end_log=intercept;
   double up_start=MathExp(mid_start_log+max_dev);
   double up_end=MathExp(mid_end_log+max_dev);
   double dn_start=MathExp(mid_start_log-max_dev);
   double dn_end=MathExp(mid_end_log-max_dev);
   double mid_start=MathExp(mid_start_log);
   double mid_end=MathExp(mid_end_log);
   datetime t_start=Time[LT_detectedAnchorBar_ST];
   datetime t_end=Time[1];
   color channelColor=(slope>0.0)?C'89,116,124':C'146,134,124';
   string upName=ea_prefix+"lt_at_line_upper";
   if(ObjectFind(0,upName)<0) {
      ObjectCreate(0,upName,OBJ_TREND,0,t_start,up_start,t_end,up_end);
      ObjectSetInteger(0,upName,OBJPROP_COLOR,channelColor);
      ObjectSetInteger(0,upName,OBJPROP_WIDTH,2);
      ObjectSetInteger(0,upName,OBJPROP_STYLE,STYLE_SOLID);
      ObjectSetInteger(0,upName,OBJPROP_RAY_RIGHT,true);
      ObjectSetInteger(0,upName,OBJPROP_SELECTABLE,false);
      ObjectSetInteger(0,upName,OBJPROP_BACK,true);
   } else {
      ObjectSetInteger(0,upName,OBJPROP_TIME1,t_start);
      ObjectSetDouble(0,upName,OBJPROP_PRICE1,up_start);
      ObjectSetInteger(0,upName,OBJPROP_TIME2,t_end);
      ObjectSetDouble(0,upName,OBJPROP_PRICE2,up_end);
      ObjectSetInteger(0,upName,OBJPROP_COLOR,channelColor);
   }
   string dnName=ea_prefix+"lt_at_line_lower";
   if(ObjectFind(0,dnName)<0) {
      ObjectCreate(0,dnName,OBJ_TREND,0,t_start,dn_start,t_end,dn_end);
      ObjectSetInteger(0,dnName,OBJPROP_COLOR,channelColor);
      ObjectSetInteger(0,dnName,OBJPROP_WIDTH,2);
      ObjectSetInteger(0,dnName,OBJPROP_STYLE,STYLE_SOLID);
      ObjectSetInteger(0,dnName,OBJPROP_RAY_RIGHT,true);
      ObjectSetInteger(0,dnName,OBJPROP_SELECTABLE,false);
      ObjectSetInteger(0,dnName,OBJPROP_BACK,true);
   } else {
      ObjectSetInteger(0,dnName,OBJPROP_TIME1,t_start);
      ObjectSetDouble(0,dnName,OBJPROP_PRICE1,dn_start);
      ObjectSetInteger(0,dnName,OBJPROP_TIME2,t_end);
      ObjectSetDouble(0,dnName,OBJPROP_PRICE2,dn_end);
      ObjectSetInteger(0,dnName,OBJPROP_COLOR,channelColor);
   }
   string midName=ea_prefix+"lt_at_line_mid";
   if(ObjectFind(0,midName)<0) {
      ObjectCreate(0,midName,OBJ_TREND,0,t_start,mid_start,t_end,mid_end);
      ObjectSetInteger(0,midName,OBJPROP_COLOR,channelColor);
      ObjectSetInteger(0,midName,OBJPROP_WIDTH,1);
      ObjectSetInteger(0,midName,OBJPROP_STYLE,STYLE_DOT);
      ObjectSetInteger(0,midName,OBJPROP_RAY_RIGHT,true);
      ObjectSetInteger(0,midName,OBJPROP_SELECTABLE,false);
      ObjectSetInteger(0,midName,OBJPROP_BACK,true);
   } else {
      ObjectSetInteger(0,midName,OBJPROP_TIME1,t_start);
      ObjectSetDouble(0,midName,OBJPROP_PRICE1,mid_start);
      ObjectSetInteger(0,midName,OBJPROP_TIME2,t_end);
      ObjectSetDouble(0,midName,OBJPROP_PRICE2,mid_end);
      ObjectSetInteger(0,midName,OBJPROP_COLOR,channelColor);
   }
}
//+------------------------------------------------------------------+
//| SECTION 7.4 - ADAPTIVE TREND                                     |
//+------------------------------------------------------------------+
void DeleteST_TrendDirectionIndicator() {
   for(int i=ObjectsTotal(0,-1,-1)-1; i>=0; i--) {
      string objName=ObjectName(0,i,-1,-1);
      if(StringFind(objName,ea_prefix+"td_top_")==0) ObjectDelete(0,objName);
   }
}
void DeleteLT_TrendDirectionIndicator() {
   for(int i=ObjectsTotal(0,-1,-1)-1; i>=0; i--) {
      string objName=ObjectName(0,i,-1,-1);
      if(StringFind(objName,ea_prefix+"lttd_top_")==0) ObjectDelete(0,objName);
   }
}
void DrawST_TrendDirectionIndicator() {
   if(!trendDirection||!isTrendVisualsVisible) { DeleteST_TrendDirectionIndicator(); return; }
   if(Bars<3) return;
   color lineColor;
   if(hist_AnchorType_ST[1]==0) lineColor=C'146,134,124';
   else if(hist_AnchorType_ST[1]==1) lineColor=C'89,116,124';
   else return;
   int lineWidth=5;
   int chartHeight=(int)ChartGetInteger(0,CHART_HEIGHT_IN_PIXELS);
   if(chartHeight<=0) return;
   double p2p=(WindowPriceMax()-WindowPriceMin())/chartHeight;
   double topCoord=WindowPriceMax()-(32*p2p);
   datetime barTime=Time[1];
   string timeStr=TimeToString(barTime);
   string topName=ea_prefix+"td_top_"+timeStr;
   if(ObjectFind(0,topName)<0) {
      ObjectCreate(0,topName,OBJ_TREND,0,Time[2],topCoord,Time[1],topCoord);
      ObjectSetInteger(0,topName,OBJPROP_COLOR,lineColor);
      ObjectSetInteger(0,topName,OBJPROP_WIDTH,lineWidth);
      ObjectSetInteger(0,topName,OBJPROP_STYLE,STYLE_SOLID);
      ObjectSetInteger(0,topName,OBJPROP_SELECTABLE,false);
      ObjectSetInteger(0,topName,OBJPROP_BACK,true);
      ObjectSetInteger(0,topName,OBJPROP_RAY_RIGHT,false);
      ObjectSetInteger(0,topName,OBJPROP_ZORDER,0);
   }
}
void DrawLT_TrendDirectionIndicator() {
   if(!trendDirection||!isTrendVisualsVisible) { DeleteLT_TrendDirectionIndicator(); return; }
   if(Bars<3) return;
   color lineColor;
   if(hist_AnchorType_LT[1]==0) lineColor=C'146,134,124';
   else if(hist_AnchorType_LT[1]==1) lineColor=C'89,116,124';
   else return;
   int lineWidth=5;
   int chartHeight=(int)ChartGetInteger(0,CHART_HEIGHT_IN_PIXELS);
   if(chartHeight<=0) return;
   double p2p=(WindowPriceMax()-WindowPriceMin())/chartHeight;
   double topCoord=WindowPriceMax()-(18*p2p);
   datetime barTime=Time[1];
   string timeStr=TimeToString(barTime);
   string topName=ea_prefix+"lttd_top_"+timeStr;
   if(ObjectFind(0,topName)<0) {
      ObjectCreate(0,topName,OBJ_TREND,0,Time[2],topCoord,Time[1],topCoord);
      ObjectSetInteger(0,topName,OBJPROP_COLOR,lineColor);
      ObjectSetInteger(0,topName,OBJPROP_WIDTH,lineWidth);
      ObjectSetInteger(0,topName,OBJPROP_STYLE,STYLE_SOLID);
      ObjectSetInteger(0,topName,OBJPROP_SELECTABLE,false);
      ObjectSetInteger(0,topName,OBJPROP_BACK,true);
      ObjectSetInteger(0,topName,OBJPROP_RAY_RIGHT,false);
      ObjectSetInteger(0,topName,OBJPROP_ZORDER,0);
   }
}
void DrawLiveST_TrendDirectionIndicator() {
   if(!trendDirection||!isTrendVisualsVisible) return;
   if(Bars<2) return;
   color lineColor;
   if(hist_AnchorType_ST[0]==0) lineColor=C'146,134,124';
   else if(hist_AnchorType_ST[0]==1) lineColor=C'89,116,124';
   else return;
   int lineWidth=5;
   int chartHeight=(int)ChartGetInteger(0,CHART_HEIGHT_IN_PIXELS);
   if(chartHeight<=0) return;
   double p2p=(WindowPriceMax()-WindowPriceMin())/chartHeight;
   double topCoord=WindowPriceMax()-(32*p2p);
   string topName=ea_prefix+"td_top_LIVE";
   if(ObjectFind(0,topName)<0) {
      ObjectCreate(0,topName,OBJ_TREND,0,Time[1],topCoord,Time[0],topCoord);
      ObjectSetInteger(0,topName,OBJPROP_STYLE,STYLE_SOLID);
      ObjectSetInteger(0,topName,OBJPROP_SELECTABLE,false);
      ObjectSetInteger(0,topName,OBJPROP_BACK,true);
      ObjectSetInteger(0,topName,OBJPROP_RAY_RIGHT,false);
      ObjectSetInteger(0,topName,OBJPROP_ZORDER,0);
   } else {
      ObjectSetInteger(0,topName,OBJPROP_TIME1,Time[1]);
      ObjectSetDouble(0,topName,OBJPROP_PRICE1,topCoord);
      ObjectSetInteger(0,topName,OBJPROP_TIME2,Time[0]);
      ObjectSetDouble(0,topName,OBJPROP_PRICE2,topCoord);
   }
   ObjectSetInteger(0,topName,OBJPROP_COLOR,lineColor);
   ObjectSetInteger(0,topName,OBJPROP_WIDTH,lineWidth);
}
void DrawLiveLT_TrendDirectionIndicator() {
   if(!trendDirection||!isTrendVisualsVisible) return;
   if(Bars<2) return;
   color lineColor;
   if(hist_AnchorType_LT[0]==0) lineColor=C'146,134,124';
   else if(hist_AnchorType_LT[0]==1) lineColor=C'89,116,124';
   else return;
   int lineWidth=5;
   int chartHeight=(int)ChartGetInteger(0,CHART_HEIGHT_IN_PIXELS);
   if(chartHeight<=0) return;
   double p2p=(WindowPriceMax()-WindowPriceMin())/chartHeight;
   double topCoord=WindowPriceMax()-(18*p2p);
   string topName=ea_prefix+"lttd_top_LIVE";
   if(ObjectFind(0,topName)<0) {
      ObjectCreate(0,topName,OBJ_TREND,0,Time[1],topCoord,Time[0],topCoord);
      ObjectSetInteger(0,topName,OBJPROP_STYLE,STYLE_SOLID);
      ObjectSetInteger(0,topName,OBJPROP_SELECTABLE,false);
      ObjectSetInteger(0,topName,OBJPROP_BACK,true);
      ObjectSetInteger(0,topName,OBJPROP_RAY_RIGHT,false);
      ObjectSetInteger(0,topName,OBJPROP_ZORDER,0);
   } else {
      ObjectSetInteger(0,topName,OBJPROP_TIME1,Time[1]);
      ObjectSetDouble(0,topName,OBJPROP_PRICE1,topCoord);
      ObjectSetInteger(0,topName,OBJPROP_TIME2,Time[0]);
      ObjectSetDouble(0,topName,OBJPROP_PRICE2,topCoord);
   }
   ObjectSetInteger(0,topName,OBJPROP_COLOR,lineColor);
   ObjectSetInteger(0,topName,OBJPROP_WIDTH,lineWidth);
}
//+------------------------------------------------------------------+
//| SECTION 7.5 - POINT OF CONTROL CALCULATION & VISUALS             |
//+------------------------------------------------------------------+
void Calc_PoC_State_OnBar(int i) {
   if(i>=Bars-1) return;
   hist_VAH_Price[i]=Close[i]; hist_VAL_Price[i]=Close[i];
   datetime dt=Time[i];
   int startBar=i;
   while(startBar+1<Bars && (Time[startBar+1]/86400)==(dt/86400)) startBar++;
   double maxP=0,minP=999999;
   for(int k=i; k<=startBar; k++) {
      if(High[k]>maxP) maxP=High[k];
      if(Low[k]<minP) minP=Low[k];
   }
   double range=maxP-minP;
   if(range<=0) { hist_Poc_Price[i]=Close[i]; return; }
   int numBins=(int)(range/Point)+1;
   if(numBins>5000) numBins=5000;
   int bins[]; ArrayResize(bins,numBins); ArrayInitialize(bins,0);
   for(int k=i; k<=startBar; k++) {
      int idx=(int)((Close[k]-minP)/Point);
      if(idx>=0 && idx<numBins) {
         double vol = hist_VolumeValue[k];
         if(vol <= 0.0 && Volume[k] > 0) vol = (double)Volume[k];
         bins[idx]+=(int)vol;
      }
   }
   int maxVol=0; int maxIdx=0;
   for(int b=0; b<numBins; b++) {
      if(bins[b]>maxVol) { maxVol=bins[b]; maxIdx=b; }
   }
   hist_Poc_Price[i]=minP+(maxIdx*Point);
   long total=0; for(int b=0; b<numBins; b++) total+=(long)bins[b];
   double target=(double)total*0.70;
   long acc=(long)bins[maxIdx]; int vah=maxIdx; int val=maxIdx;
   while(acc<target&&(val>0||vah<numBins-1)) {
      long v_up=(vah+1<numBins)?(long)bins[vah+1]:-1;
      long v_dn=(val-1>=0)?(long)bins[val-1]:-1;
      if(v_up>v_dn) { acc+=v_up; vah++; }
      else if(v_dn!=-1) { acc+=v_dn; val--; }
      else if(v_up!=-1) { acc+=v_up; vah++; }
      else break;
   }
   hist_VAH_Price[i]=minP+vah*Point;
   hist_VAL_Price[i]=minP+val*Point;
}
void Calc_VWAP_OnBar(int i) {
   if(i>=Bars-1) return;
   datetime dt=Time[i];
   int startBar=i;
   while(startBar+1<Bars && (Time[startBar+1]/86400)==(dt/86400)) startBar++;
   double sumVol=0.0,sumPV=0.0,sumPPV=0.0;
   for(int k=i; k<=startBar; k++) {
      double vol = hist_VolumeValue[k];
      if(vol <= 0.0 && Volume[k] > 0) vol = (double)Volume[k];
      sumVol+=vol;
      sumPV+=Close[k]*vol;
      sumPPV+=Close[k]*Close[k]*vol;
   }
   if(sumVol<=0.0) { hist_VWAP_Price[i]=Close[i]; hist_VWAP_Sigma[i]=0.0; return; }
   double vwap=sumPV/sumVol;
   double variance=sumPPV/sumVol-vwap*vwap;
   if(variance<0.0) variance=0.0;
   hist_VWAP_Price[i]=vwap;
   hist_VWAP_Sigma[i]=MathSqrt(variance);
}
void Calc_RefLevels_OnBar(int i) {
   if(i>=Bars-1) return;
   long anchorSec=(long)LevelDayAnchorHourEST*3600;
   long offI=_GetEstOffsetForTime(Time[i]);
   long estI=(long)Time[i]+offI;
   long curDay=(estI-anchorSec)/86400;
   int dayStart=i;
   while(dayStart+1<Bars) {
      long offN=_GetEstOffsetForTime(Time[dayStart+1]);
      if(((long)Time[dayStart+1]+offN-anchorSec)/86400!=curDay) break;
      dayStart++;
   }
   hist_DailyOpen_Price[i]=Open[dayStart];
   double sHigh=High[i],sLow=Low[i];
   double orH=0.0,orL=0.0;
   bool orSet=false;
   MqlDateTime kdt;
   for(int k=i; k<=dayStart; k++) {
      if(High[k]>sHigh) sHigh=High[k];
      if(Low[k]<sLow) sLow=Low[k];
      long offK=_GetEstOffsetForTime(Time[k]);
      ZeroMemory(kdt);
      TimeToStruct((datetime)((long)Time[k]+offK),kdt);
      int mod=kdt.hour*60+kdt.min;
      if(mod>=570 && mod<630) {
         if(!orSet) { orH=High[k]; orL=Low[k]; orSet=true; }
         else { if(High[k]>orH) orH=High[k]; if(Low[k]<orL) orL=Low[k]; }
      }
   }
   hist_Session_High[i]=sHigh;
   hist_Session_Low[i]=sLow;
   hist_OR_High[i]=orH;
   hist_OR_Low[i]=orL;
   if(dayStart+1<Bars) {
      long offP=_GetEstOffsetForTime(Time[dayStart+1]);
      long prevDay=((long)Time[dayStart+1]+offP-anchorSec)/86400;
      int pEnd=dayStart+1;
      while(pEnd+1<Bars) {
         long offN=_GetEstOffsetForTime(Time[pEnd+1]);
         if(((long)Time[pEnd+1]+offN-anchorSec)/86400!=prevDay) break;
         pEnd++;
      }
      double pH=High[dayStart+1],pL=Low[dayStart+1];
      for(int k=dayStart+1; k<=pEnd; k++) {
         if(High[k]>pH) pH=High[k];
         if(Low[k]<pL) pL=Low[k];
      }
      hist_PrevDay_High[i]=pH;
      hist_PrevDay_Low[i]=pL;
      hist_PrevDay_Close[i]=Close[dayStart+1];
   } else {
      hist_PrevDay_High[i]=0.0;
      hist_PrevDay_Low[i]=0.0;
      hist_PrevDay_Close[i]=0.0;
   }
   long curWk=(estI-259200-anchorSec)/604800;
   int wkStart=i;
   while(wkStart+1<Bars) {
      long offN=_GetEstOffsetForTime(Time[wkStart+1]);
      if(((long)Time[wkStart+1]+offN-259200-anchorSec)/604800!=curWk) break;
      wkStart++;
   }
   hist_WeeklyOpen_Price[i]=Open[wkStart];
}
void Calc_MultiDay_OnBar(int i) {
   if(i>=Bars-1) return;
   const int lookback=2760;
   int endBar=i+lookback-1;
   if(endBar>Bars-1) endBar=Bars-1;
   int n=endBar-i+1;
   if(n<2) { hist_MultiDay_Slope[i]=0.0; hist_MultiDay_Position[i]=0.5; return; }
   double hh=High[i],ll=Low[i];
   double sx=0.0,sy=0.0,sxy=0.0,sxx=0.0;
   for(int k=i; k<=endBar; k++) {
      if(High[k]>hh) hh=High[k];
      if(Low[k]<ll) ll=Low[k];
      double x=(double)((lookback-1)-(k-i));
      double y=Close[k];
      sx+=x;
      sy+=y;
      sxy+=x*y;
      sxx+=x*x;
   }
   hist_MultiDay_Position[i]=(hh>ll)?(Close[i]-ll)/(hh-ll):0.5;
   double denom=(double)n*sxx-sx*sx;
   double slope=(denom!=0.0)?((double)n*sxy-sx*sy)/denom:0.0;
   int la=ArraySize(assignedATR);
   double atr=(i<la)?assignedATR[i]:0.0;
   hist_MultiDay_Slope[i]=(atr>0.0)?slope/atr:0.0;
}
void CalculateDailyPoC() {
   Calc_PoC_State_OnBar(0);
   dailyPoCPrice=hist_Poc_Price[0];
   if(g_isLoading) LogBootMessage("PoC: Live Update "+DoubleToString(dailyPoCPrice,Digits));
   dailyVAHPrice=hist_VAH_Price[0];
   dailyVALPrice=hist_VAL_Price[0];
}
void DeleteDailyPoC() {
   ObjectDelete(0,ea_prefix+"poc_line"); ObjectDelete(0,ea_prefix+"poc_label");
   ObjectDelete(0,ea_prefix+"vah_line"); ObjectDelete(0,ea_prefix+"vah_label");
   ObjectDelete(0,ea_prefix+"val_line"); ObjectDelete(0,ea_prefix+"val_label");
}
void DrawDailyPoC() {
   if(!isPocVisualsVisible) { DeleteDailyPoC(); return; }
   string lineName=ea_prefix+"poc_line"; string labelName=ea_prefix+"poc_label";
   string vahLine=ea_prefix+"vah_line"; string vahLabel=ea_prefix+"vah_label";
   string valLine=ea_prefix+"val_line"; string valLabel=ea_prefix+"val_label";
   if(ObjectFind(0,lineName)>=0) ObjectDelete(0,lineName);
   if(ObjectFind(0,labelName)>=0) ObjectDelete(0,labelName);
   if(ObjectFind(0,vahLine)>=0) ObjectDelete(0,vahLine);
   if(ObjectFind(0,vahLabel)>=0) ObjectDelete(0,vahLabel);
   if(ObjectFind(0,valLine)>=0) ObjectDelete(0,valLine);
   if(ObjectFind(0,valLabel)>=0) ObjectDelete(0,valLabel);
   if(dailyPoCPrice<=0) return;
   int firstVisible=WindowFirstVisibleBar();
   int barsPerChart=WindowBarsPerChart();
   int targetBarVAH=firstVisible-(int)((double)barsPerChart*0.825);
   int targetBarPoC=firstVisible-(int)((double)barsPerChart*0.865);
   int targetBarVAL=firstVisible-(int)((double)barsPerChart*0.975);
   datetime timeVAH; if(targetBarVAH>=Bars) timeVAH=Time[Bars-1]; else if(targetBarVAH>=0) timeVAH=Time[targetBarVAH]; else timeVAH=Time[0]+Period()*60*MathAbs(targetBarVAH);
   datetime timePoC; if(targetBarPoC>=Bars) timePoC=Time[Bars-1]; else if(targetBarPoC>=0) timePoC=Time[targetBarPoC]; else timePoC=Time[0]+Period()*60*MathAbs(targetBarPoC);
   datetime timeVAL; if(targetBarVAL>=Bars) timeVAL=Time[Bars-1]; else if(targetBarVAL>=0) timeVAL=Time[targetBarVAL]; else timeVAL=Time[0]+Period()*60*MathAbs(targetBarVAL);
   int chartHeight=(int)ChartGetInteger(0,CHART_HEIGHT_IN_PIXELS);
   double p2p=0.0;
   if(chartHeight>0) p2p=(WindowPriceMax()-WindowPriceMin())/(double)chartHeight;
   double pixelOffset=p2p*2.0;
   ObjectCreate(0,lineName,OBJ_HLINE,0,0,dailyPoCPrice);
   ObjectSetInteger(0,lineName,OBJPROP_COLOR,C'96,95,113');
   ObjectCreate(0,labelName,OBJ_TEXT,0,timePoC,dailyPoCPrice+pixelOffset);
   ObjectSetString(0,labelName,OBJPROP_TEXT,"POC: "+DoubleToString(dailyPoCPrice,Digits));
   ObjectSetInteger(0,labelName,OBJPROP_COLOR,C'96,95,113');
   ObjectSetInteger(0,labelName,OBJPROP_FONTSIZE,8);
   ObjectSetInteger(0,labelName,OBJPROP_ANCHOR,ANCHOR_LEFT_LOWER);
   if(dailyVAHPrice>0) {
      ObjectCreate(0,vahLine,OBJ_HLINE,0,0,dailyVAHPrice);
      ObjectSetInteger(0,vahLine,OBJPROP_COLOR,C'96,95,113');
      ObjectSetInteger(0,vahLine,OBJPROP_STYLE,STYLE_DOT);
      ObjectCreate(0,vahLabel,OBJ_TEXT,0,timeVAH,dailyVAHPrice+pixelOffset);
      ObjectSetString(0,vahLabel,OBJPROP_TEXT,"VAH");
      ObjectSetInteger(0,vahLabel,OBJPROP_COLOR,C'96,95,113');
      ObjectSetInteger(0,vahLabel,OBJPROP_FONTSIZE,8);
      ObjectSetInteger(0,vahLabel,OBJPROP_ANCHOR,ANCHOR_LEFT_LOWER);
   }
   if(dailyVALPrice>0) {
      ObjectCreate(0,valLine,OBJ_HLINE,0,0,dailyVALPrice);
      ObjectSetInteger(0,valLine,OBJPROP_COLOR,C'96,95,113');
      ObjectSetInteger(0,valLine,OBJPROP_STYLE,STYLE_DOT);
      ObjectCreate(0,valLabel,OBJ_TEXT,0,timeVAL,dailyVALPrice+pixelOffset);
      ObjectSetString(0,valLabel,OBJPROP_TEXT,"VAL");
      ObjectSetInteger(0,valLabel,OBJPROP_COLOR,C'96,95,113');
      ObjectSetInteger(0,valLabel,OBJPROP_FONTSIZE,8);
      ObjectSetInteger(0,valLabel,OBJPROP_ANCHOR,ANCHOR_LEFT_LOWER);
   }
}
void UpdateDailyPoCPositions() {
   if(!isPocVisualsVisible) return;
   int firstVisible=WindowFirstVisibleBar();
   int barsPerChart=WindowBarsPerChart();
   int targetBarVAH=firstVisible-(int)((double)barsPerChart*0.825);
   int targetBarPoC=firstVisible-(int)((double)barsPerChart*0.865);
   int targetBarVAL=firstVisible-(int)((double)barsPerChart*0.975);
   datetime timeVAH; if(targetBarVAH>=Bars) timeVAH=Time[Bars-1]; else if(targetBarVAH>=0) timeVAH=Time[targetBarVAH]; else timeVAH=Time[0]+Period()*60*MathAbs(targetBarVAH);
   datetime timePoC; if(targetBarPoC>=Bars) timePoC=Time[Bars-1]; else if(targetBarPoC>=0) timePoC=Time[targetBarPoC]; else timePoC=Time[0]+Period()*60*MathAbs(targetBarPoC);
   datetime timeVAL; if(targetBarVAL>=Bars) timeVAL=Time[Bars-1]; else if(targetBarVAL>=0) timeVAL=Time[targetBarVAL]; else timeVAL=Time[0]+Period()*60*MathAbs(targetBarVAL);
   int chartHeight=(int)ChartGetInteger(0,CHART_HEIGHT_IN_PIXELS);
   double p2p=0.0;
   if(chartHeight>0) p2p=(WindowPriceMax()-WindowPriceMin())/(double)chartHeight;
   double pixelOffset=p2p*2.0;
   if(ObjectFind(0,ea_prefix+"vah_label")>=0 && dailyVAHPrice>0) {
      ObjectSetInteger(0,ea_prefix+"vah_label",OBJPROP_TIME1,timeVAH);
      ObjectSetDouble(0,ea_prefix+"vah_label",OBJPROP_PRICE1,dailyVAHPrice+pixelOffset);
   }
   if(ObjectFind(0,ea_prefix+"poc_label")>=0 && dailyPoCPrice>0) {
      ObjectSetInteger(0,ea_prefix+"poc_label",OBJPROP_TIME1,timePoC);
      ObjectSetDouble(0,ea_prefix+"poc_label",OBJPROP_PRICE1,dailyPoCPrice+pixelOffset);
   }
   if(ObjectFind(0,ea_prefix+"val_label")>=0 && dailyVALPrice>0) {
      ObjectSetInteger(0,ea_prefix+"val_label",OBJPROP_TIME1,timeVAL);
      ObjectSetDouble(0,ea_prefix+"val_label",OBJPROP_PRICE1,dailyVALPrice+pixelOffset);
   }
}
//+------------------------------------------------------------------+
//| SECTION 7.6 - SESSION VISUALS                                    |
//+------------------------------------------------------------------+
bool _DoesAssetTradeWeekends() {
   static int does_trade_weekends=-1;
   if(does_trade_weekends!=-1) return (bool)does_trade_weekends;
   for(int i=1; i<MathMin(Bars-1,30*1440/Period()); i++) {
      int dayOfWeek=TimeDayOfWeek(Time[i]);
      if(dayOfWeek==0||dayOfWeek==6) {
         if(Volume[i]>0||High[i]!=Low[i]) { does_trade_weekends=1; return true; }
      }
   }
   does_trade_weekends=0;
   return false;
}
long TimeToMinutes(string time_str) {
   int separator_pos=StringFind(time_str,":");
   if(separator_pos<0) return -1;
   long hour=StringToInteger(StringSubstr(time_str,0,separator_pos));
   long minute=StringToInteger(StringSubstr(time_str,separator_pos+1));
   return hour*60+minute;
}
void DrawSingleSession(datetime day_start_est,string session_name,string start_time_str,string end_time_str) {
   color session_color;
   if(session_name=="Asia") { start_time_str="20:00"; end_time_str="05:00"; session_color=C'15,25,38'; }
   else if(session_name=="London") { start_time_str="03:00"; end_time_str="11:30"; session_color=C'15,25,38'; }
   else if(session_name=="NY") { start_time_str="09:30"; end_time_str="16:00"; session_color=C'15,25,38'; }
   else { session_color=C'15,25,38'; }
   long start_minutes=TimeToMinutes(start_time_str);
   long end_minutes=TimeToMinutes(end_time_str);
   if(start_minutes<0||end_minutes<0) return;
   MqlDateTime dt;
   TimeToStruct(day_start_est,dt);
   string date_suffix=StringFormat("%d.%d.%d",dt.year,dt.mon,dt.day);
   string obj_prefix=ea_prefix+session_name+"_"+date_suffix;
   datetime session_start_est,session_end_est;
   if(start_minutes>end_minutes) {
      session_start_est=(datetime)(day_start_est-86400+(start_minutes*60));
      session_end_est=(datetime)(day_start_est+(end_minutes*60));
   } else {
      session_start_est=(datetime)(day_start_est+(start_minutes*60));
      session_end_est=(datetime)(day_start_est+(end_minutes*60));
   }
   long server_gmt_offset=TimeCurrent()-TimeGMT();
   long est_gmt_offset=GetUSEasternOffsetSeconds();
   datetime plot_start_time=(datetime)(session_start_est-est_gmt_offset+server_gmt_offset);
   datetime plot_end_time=(datetime)(session_end_est-est_gmt_offset+server_gmt_offset);
   datetime now_server=TimeCurrent();
   datetime dynamic_plot_end_time=plot_end_time;
   if(now_server>=plot_start_time&&now_server<plot_end_time) dynamic_plot_end_time=now_server;
   int start_bar=iBarShift(NULL,0,plot_start_time,false);
   int end_bar=iBarShift(NULL,0,dynamic_plot_end_time,false);
   if(start_bar<0||end_bar<0) return;
   int bars_in_session=start_bar-end_bar+1;
   if(bars_in_session<=0) return;
   int high_bar_idx=iHighest(NULL,0,MODE_HIGH,bars_in_session,end_bar);
   int low_bar_idx=iLowest(NULL,0,MODE_LOW,bars_in_session,end_bar);
   if(high_bar_idx<0||low_bar_idx<0) return;
   double session_high=High[high_bar_idx];
   double session_low=Low[low_bar_idx];
   color chart_bg_color=(color)ChartGetInteger(0,CHART_COLOR_BACKGROUND);
   string box_name=obj_prefix+"_Box";
   if(ObjectFind(0,box_name)<0) ObjectCreate(0,box_name,OBJ_RECTANGLE,0,0,0);
   ObjectSetInteger(0,box_name,OBJPROP_TIME1,plot_start_time);
   ObjectSetDouble(0,box_name,OBJPROP_PRICE1,session_high);
   ObjectSetInteger(0,box_name,OBJPROP_TIME2,dynamic_plot_end_time);
   ObjectSetDouble(0,box_name,OBJPROP_PRICE2,session_low);
   ObjectSetInteger(0,box_name,OBJPROP_COLOR,session_color);
   ObjectSetInteger(0,box_name,OBJPROP_BGCOLOR,chart_bg_color);
   ObjectSetInteger(0,box_name,OBJPROP_STYLE,STYLE_SOLID);
   ObjectSetInteger(0,box_name,OBJPROP_WIDTH,1);
   ObjectSetInteger(0,box_name,OBJPROP_BACK,true);
   ObjectSetInteger(0,box_name,OBJPROP_FILL,true);
   ObjectSetInteger(0,box_name,OBJPROP_SELECTABLE,false);
   ObjectSetInteger(0,box_name,OBJPROP_ZORDER,0);
   string high_line_name=obj_prefix+"_High";
   if(ObjectFind(0,high_line_name)<0) ObjectCreate(0,high_line_name,OBJ_TREND,0,0,0,0,0);
   ObjectSetInteger(0,high_line_name,OBJPROP_TIME1,plot_start_time);
   ObjectSetDouble(0,high_line_name,OBJPROP_PRICE1,session_high);
   ObjectSetInteger(0,high_line_name,OBJPROP_TIME2,dynamic_plot_end_time);
   ObjectSetDouble(0,high_line_name,OBJPROP_PRICE2,session_high);
   ObjectSetInteger(0,high_line_name,OBJPROP_COLOR,session_color);
   ObjectSetInteger(0,high_line_name,OBJPROP_WIDTH,1);
   ObjectSetInteger(0,high_line_name,OBJPROP_BACK,true);
   ObjectSetInteger(0,high_line_name,OBJPROP_SELECTABLE,false);
   ObjectSetInteger(0,high_line_name,OBJPROP_RAY_RIGHT,false);
   ObjectSetInteger(0,high_line_name,OBJPROP_ZORDER,0);
   string low_line_name=obj_prefix+"_Low";
   if(ObjectFind(0,low_line_name)<0) ObjectCreate(0,low_line_name,OBJ_TREND,0,0,0,0,0);
   ObjectSetInteger(0,low_line_name,OBJPROP_TIME1,plot_start_time);
   ObjectSetDouble(0,low_line_name,OBJPROP_PRICE1,session_low);
   ObjectSetInteger(0,low_line_name,OBJPROP_TIME2,dynamic_plot_end_time);
   ObjectSetDouble(0,low_line_name,OBJPROP_PRICE2,session_low);
   ObjectSetInteger(0,low_line_name,OBJPROP_COLOR,session_color);
   ObjectSetInteger(0,low_line_name,OBJPROP_WIDTH,1);
   ObjectSetInteger(0,low_line_name,OBJPROP_BACK,true);
   ObjectSetInteger(0,low_line_name,OBJPROP_SELECTABLE,false);
   ObjectSetInteger(0,low_line_name,OBJPROP_RAY_RIGHT,false);
   ObjectSetInteger(0,low_line_name,OBJPROP_ZORDER,0);
   string label_name=obj_prefix+"_Label";
   if(ObjectFind(0,label_name)<0) ObjectCreate(0,label_name,OBJ_TEXT,0,0,0);
   ObjectSetInteger(0,label_name,OBJPROP_TIME1,plot_start_time);
   ObjectSetDouble(0,label_name,OBJPROP_PRICE1,session_high+5*Point);
   string label_text_upper=session_name; StringToUpper(label_text_upper);
   ObjectSetString(0,label_name,OBJPROP_TEXT,label_text_upper);
   ObjectSetInteger(0,label_name,OBJPROP_COLOR,C'146,134,124');
   ObjectSetInteger(0,label_name,OBJPROP_FONTSIZE,12);
   ObjectSetString(0,label_name,OBJPROP_FONT,"Segoe UI");
   ObjectSetInteger(0,label_name,OBJPROP_ANCHOR,ANCHOR_LEFT_UPPER);
   ObjectSetInteger(0,label_name,OBJPROP_BACK,true);
   ObjectSetInteger(0,label_name,OBJPROP_SELECTABLE,false);
   ObjectSetInteger(0,label_name,OBJPROP_ZORDER,0);
}
void DeleteSessionObjects() {
   for(int i=ObjectsTotal(0,-1,-1)-1; i>=0; i--) {
      string objName=ObjectName(0,i,-1,-1);
      if(StringFind(objName,ea_prefix+"Asia_")==0||StringFind(objName,ea_prefix+"London_")==0||
      StringFind(objName,ea_prefix+"NY_")==0||StringFind(objName,ea_prefix+"Weekend_")==0) {
         ObjectDelete(0,objName);
      }
   }
}
void DrawSessionBoxes() {
   if(!isSessionVisualsVisible) { DeleteSessionObjects(); return; }
   bool tradesOnWeekends=_DoesAssetTradeWeekends();
   if(g_isLoading) LogBootMessage("Sessions: Weekend Trading? "+(tradesOnWeekends?"YES":"NO"));
   long est_offset_seconds=GetUSEasternOffsetSeconds();
   datetime est_now=(datetime)(TimeGMT()+est_offset_seconds);
   MqlDateTime dt_est; TimeToStruct(est_now,dt_est);
   dt_est.hour=0; dt_est.min=0; dt_est.sec=0;
   datetime today_start_est=StructToTime(dt_est);
   datetime oldest_bar_server_time=Time[Bars-1];
   long server_gmt_offset=TimeCurrent()-TimeGMT();
   datetime oldest_bar_est=(datetime)(oldest_bar_server_time-server_gmt_offset+est_offset_seconds);
   for(int i=0; ; i++) {
      datetime day_to_draw_est=(datetime)(today_start_est-(i*86400));
      if(day_to_draw_est<oldest_bar_est-86400) break;
      MqlDateTime dt; TimeToStruct(day_to_draw_est,dt);
      int dayOfWeek=dt.day_of_week;
      bool isWeekday=(dayOfWeek!=0&&dayOfWeek!=6);
      if(tradesOnWeekends) {
         if(isWeekday) {
            DrawSingleSession(day_to_draw_est,"Asia","","");
            DrawSingleSession(day_to_draw_est,"London","","");
            DrawSingleSession(day_to_draw_est,"NY","","");
         } else {
            DrawSingleSession(day_to_draw_est,"Weekend","00:00","24:00");
         }
      } else {
         if(isWeekday) {
            DrawSingleSession(day_to_draw_est,"Asia","","");
            DrawSingleSession(day_to_draw_est,"London","","");
            DrawSingleSession(day_to_draw_est,"NY","","");
         }
      }
   }
   datetime potential_starts[10],potential_ends[10],potential_day_starts[10];
   string potential_names[10],potential_start_strings[10],potential_end_strings[10];
   ArrayInitialize(potential_starts,0); ArrayInitialize(potential_ends,0); ArrayInitialize(potential_day_starts,0);
   int count=0;
   datetime days_to_check[2];
   days_to_check[0]=today_start_est; days_to_check[1]=(datetime)(today_start_est+86400);
   for(int d=0; d<2; d++) {
      datetime day_start=days_to_check[d];
      MqlDateTime dt; TimeToStruct(day_start,dt);
      int dayOfWeek=dt.day_of_week;
      bool isWeekday=(dayOfWeek!=0&&dayOfWeek!=6);
      if(tradesOnWeekends&&!isWeekday) {
         long start_m=TimeToMinutes("00:00"); long end_m=TimeToMinutes("24:00");
         potential_starts[count]=(datetime)(day_start+(start_m*60));
         potential_ends[count]=(datetime)(day_start+(end_m*60));
         potential_names[count]="Weekend"; potential_day_starts[count]=day_start; potential_start_strings[count]="00:00"; potential_end_strings[count]="24:00";
         count++;
      } else if(isWeekday||(tradesOnWeekends&&isWeekday)) {
         long start_m_asia=TimeToMinutes("20:00"); long end_m_asia=TimeToMinutes("05:00");
         if(start_m_asia>end_m_asia) {
            potential_starts[count]=(datetime)(day_start-86400+(start_m_asia*60));
            potential_ends[count]=(datetime)(day_start+(end_m_asia*60));
         } else {
            potential_starts[count]=(datetime)(day_start+(start_m_asia*60));
            potential_ends[count]=(datetime)(day_start+(end_m_asia*60));
         }
         potential_names[count]="Asia"; potential_day_starts[count]=day_start; potential_start_strings[count]=""; potential_end_strings[count]="";
         count++;
         long start_m_lon=TimeToMinutes("03:00"); long end_m_lon=TimeToMinutes("11:30");
         if(start_m_lon>end_m_lon) {
            potential_starts[count]=(datetime)(day_start-86400+(start_m_lon*60));
            potential_ends[count]=(datetime)(day_start+(end_m_lon*60));
         } else {
            potential_starts[count]=(datetime)(day_start+(start_m_lon*60));
            potential_ends[count]=(datetime)(day_start+(end_m_lon*60));
         }
         potential_names[count]="London"; potential_day_starts[count]=day_start; potential_start_strings[count]=""; potential_end_strings[count]="";
         count++;
         long start_m_ny=TimeToMinutes("09:30"); long end_m_ny=TimeToMinutes("16:00");
         if(start_m_ny>end_m_ny) {
            potential_starts[count]=(datetime)(day_start-86400+(start_m_ny*60));
            potential_ends[count]=(datetime)(day_start+(end_m_ny*60));
         } else {
            potential_starts[count]=(datetime)(day_start+(start_m_ny*60));
            potential_ends[count]=(datetime)(day_start+(end_m_ny*60));
         }
         potential_names[count]="NY"; potential_day_starts[count]=day_start; potential_start_strings[count]=""; potential_end_strings[count]="";
         count++;
      }
   }
   datetime next_session_start_est=0;
   int next_session_index=-1;
   int current_session_index=-1;
   for(int i=0; i<count; i++) {
      if(est_now>=potential_starts[i]&&est_now<potential_ends[i]) { current_session_index=i; break; }
      if(potential_starts[i]>est_now) {
         if(next_session_index==-1||potential_starts[i]<next_session_start_est) {
            next_session_start_est=potential_starts[i]; next_session_index=i;
         }
      }
   }
   int session_to_draw_index=-1;
   if(current_session_index!=-1) session_to_draw_index=current_session_index;
   else if(next_session_index!=-1) session_to_draw_index=next_session_index;
   if(session_to_draw_index!=-1) {
      MqlDateTime dt_next_start; TimeToStruct(potential_starts[session_to_draw_index],dt_next_start);
      int next_start_dayOfWeek=dt_next_start.day_of_week;
      bool can_draw=true;
      if(!tradesOnWeekends) {
         if(potential_names[session_to_draw_index]=="Weekend") can_draw=false;
         if(next_start_dayOfWeek==0) {
            if(potential_names[session_to_draw_index]!="Asia") can_draw=false;
         } else if(next_start_dayOfWeek==6) {
            can_draw=false;
         }
      }
      if(can_draw) {
         DrawSingleSession(potential_day_starts[session_to_draw_index],potential_names[session_to_draw_index],potential_start_strings[session_to_draw_index],potential_end_strings[session_to_draw_index]);
      }
   }
}
//+------------------------------------------------------------------+
//| SECTION 7.7 - OBV VISUALS                                        |
//+------------------------------------------------------------------+
void DrawOBV_Visuals_Historical(int barIndex) {
   if(!isOBVVisualsVisible) return;
   if(barIndex<1||barIndex>=ArraySize(state_OBV_Final)-1||barIndex>=ArraySize(state_HarmVol_LLEMA)-1) return;
   double priceRange=WindowPriceMax()-WindowPriceMin();
   if(priceRange<=0) return;
   string bgName=ea_prefix+"obv_bg_pane";
   if(ObjectFind(0,bgName)<0) {
      ObjectCreate(0,bgName,OBJ_RECTANGLE,0,0,0);
      ObjectSetInteger(0,bgName,OBJPROP_COLOR,C'15,25,38');
      ObjectSetInteger(0,bgName,OBJPROP_BGCOLOR,C'15,25,38');
      ObjectSetInteger(0,bgName,OBJPROP_STYLE,STYLE_SOLID);
      ObjectSetInteger(0,bgName,OBJPROP_WIDTH,1);
      ObjectSetInteger(0,bgName,OBJPROP_FILL,true);
      ObjectSetInteger(0,bgName,OBJPROP_BACK,true);
      ObjectSetInteger(0,bgName,OBJPROP_ZORDER,30000);
   }
   int leftBar=WindowFirstVisibleBar()+100;
   if(leftBar>=Bars) leftBar=Bars-1;
   datetime leftTime=Time[leftBar];
   datetime rightTime=Time[0];
   ObjectMove(0,bgName,0,leftTime,WindowPriceMin());
   ObjectMove(0,bgName,1,rightTime,WindowPriceMin()+(priceRange*0.25));
   string borderName=ea_prefix+"obv_top_border";
   if(ObjectFind(0,borderName)<0) {
      ObjectCreate(0,borderName,OBJ_TREND,0,leftTime,WindowPriceMin()+(priceRange*0.25),rightTime,WindowPriceMin()+(priceRange*0.25));
      ObjectSetInteger(0,borderName,OBJPROP_COLOR,C'95,107,119');
      ObjectSetInteger(0,borderName,OBJPROP_WIDTH,1);
      ObjectSetInteger(0,borderName,OBJPROP_STYLE,STYLE_SOLID);
      ObjectSetInteger(0,borderName,OBJPROP_RAY_RIGHT,true);
      ObjectSetInteger(0,borderName,OBJPROP_BACK,false);
      ObjectSetInteger(0,borderName,OBJPROP_ZORDER,30001);
   } else {
      ObjectSetInteger(0,borderName,OBJPROP_TIME1,leftTime);
      ObjectSetDouble(0,borderName,OBJPROP_PRICE1,WindowPriceMin()+(priceRange*0.25));
      ObjectSetInteger(0,borderName,OBJPROP_TIME2,rightTime);
      ObjectSetDouble(0,borderName,OBJPROP_PRICE2,WindowPriceMin()+(priceRange*0.25));
   }
   double zeroLine=WindowPriceMin()+(priceRange*0.125);
   string zeroName=ea_prefix+"obv_zero_line";
   if(ObjectFind(0,zeroName)<0) {
      ObjectCreate(0,zeroName,OBJ_TREND,0,leftTime,zeroLine,rightTime,zeroLine);
      ObjectSetInteger(0,zeroName,OBJPROP_COLOR,C'96,95,113');
      ObjectSetInteger(0,zeroName,OBJPROP_RAY_RIGHT,true);
      ObjectSetInteger(0,zeroName,OBJPROP_BACK,false);
      ObjectSetInteger(0,zeroName,OBJPROP_ZORDER,30001);
   } else {
      ObjectSetInteger(0,zeroName,OBJPROP_TIME1,leftTime);
      ObjectSetDouble(0,zeroName,OBJPROP_PRICE1,zeroLine);
      ObjectSetInteger(0,zeroName,OBJPROP_TIME2,rightTime);
      ObjectSetDouble(0,zeroName,OBJPROP_PRICE2,zeroLine);
   }
   double maxAbsDelta=0.0;
   double maxAbsLlema=0.0;
   double maxAbsEmaOsc=0.0;
   double maxAbsRangeOsc=0.0;
   int visibleBars=WindowFirstVisibleBar();
   int limit=MathMin(Bars-1,visibleBars);
   int rightBar=(int)MathMax(0.0,(double)(visibleBars-WindowBarsPerChart()+1));
   for(int k=rightBar; k<=limit; k++) {
      if(k<ArraySize(state_OBV_Final)) {
         double absVal=MathAbs(state_OBV_Final[k]);
         if(absVal>maxAbsDelta) maxAbsDelta=absVal;
      }
      if(k<ArraySize(state_HarmVol_LLEMA)) {
         double absLlema=MathAbs(state_HarmVol_LLEMA[k]);
         if(absLlema>maxAbsLlema) maxAbsLlema=absLlema;
      }
      if(k<ArraySize(state_HarmVol_EMAOsc)) {
         double absEma=MathAbs(state_HarmVol_EMAOsc[k]);
         if(absEma>maxAbsEmaOsc) maxAbsEmaOsc=absEma;
      }
      if(k<ArraySize(state_RangeOsc_Val)) {
         double absRosc=MathAbs(state_RangeOsc_Val[k]);
         if(absRosc>maxAbsRangeOsc) maxAbsRangeOsc=absRosc;
      }
   }
   if(maxAbsDelta==0.0) maxAbsDelta=1.0;
   if(maxAbsLlema==0.0) maxAbsLlema=1.0;
   if(maxAbsEmaOsc==0.0) maxAbsEmaOsc=1.0;
   if(maxAbsRangeOsc==0.0) maxAbsRangeOsc=1.0;
   datetime barTime=Time[barIndex];
   datetime prevTime=Time[barIndex+1];
   string timeStr=TimeToString(barTime);
   string signalName=ea_prefix+"obv_signal_"+timeStr;
   if(isOBVfLineVisible) {
      double mappedPrice=zeroLine+((state_OBV_Final[barIndex]/maxAbsDelta)*(priceRange*0.11));
      double mappedPricePrev=zeroLine+((state_OBV_Final[barIndex+1]/maxAbsDelta)*(priceRange*0.11));
      color signalColor=(state_TChan_OC[barIndex]==1.0)?clrOrange:clrDodgerBlue;
      if(ObjectFind(0,signalName)<0) {
         ObjectCreate(0,signalName,OBJ_TREND,0,prevTime,mappedPricePrev,barTime,mappedPrice);
         ObjectSetInteger(0,signalName,OBJPROP_COLOR,signalColor);
         ObjectSetInteger(0,signalName,OBJPROP_WIDTH,2);
         ObjectSetInteger(0,signalName,OBJPROP_STYLE,STYLE_SOLID);
         ObjectSetInteger(0,signalName,OBJPROP_SELECTABLE,false);
         ObjectSetInteger(0,signalName,OBJPROP_BACK,false);
         ObjectSetInteger(0,signalName,OBJPROP_RAY_RIGHT,false);
         ObjectSetInteger(0,signalName,OBJPROP_ZORDER,30002);
      } else {
         ObjectSetInteger(0,signalName,OBJPROP_TIME1,prevTime);
         ObjectSetDouble(0,signalName,OBJPROP_PRICE1,mappedPricePrev);
         ObjectSetInteger(0,signalName,OBJPROP_TIME2,barTime);
         ObjectSetDouble(0,signalName,OBJPROP_PRICE2,mappedPrice);
         ObjectSetInteger(0,signalName,OBJPROP_COLOR,signalColor);
      }
   } else {
      if(ObjectFind(0,signalName)>=0) ObjectDelete(0,signalName);
   }
   string llemaName=ea_prefix+"obv_llema_"+timeStr;
   if(isKamaHistoVisible) {
      if(state_HarmVol_LLEMA[barIndex]==0.0) {
         if(ObjectFind(0,llemaName)>=0) ObjectDelete(0,llemaName);
      } else {
         double mappedLlemaPrice=zeroLine+((state_HarmVol_LLEMA[barIndex]/maxAbsLlema)*(priceRange*0.11));
         color llemaColor=(state_HarmVol_LLEMA[barIndex]>0.0)?C'146,134,124':C'89,116,124';
         if(ObjectFind(0,llemaName)<0) {
            ObjectCreate(0,llemaName,OBJ_TREND,0,barTime,zeroLine,barTime,mappedLlemaPrice);
            ObjectSetInteger(0,llemaName,OBJPROP_COLOR,llemaColor);
            ObjectSetInteger(0,llemaName,OBJPROP_WIDTH,3);
            ObjectSetInteger(0,llemaName,OBJPROP_STYLE,STYLE_SOLID);
            ObjectSetInteger(0,llemaName,OBJPROP_SELECTABLE,false);
            ObjectSetInteger(0,llemaName,OBJPROP_BACK,false);
            ObjectSetInteger(0,llemaName,OBJPROP_RAY_RIGHT,false);
            ObjectSetInteger(0,llemaName,OBJPROP_ZORDER,30003);
         } else {
            ObjectSetInteger(0,llemaName,OBJPROP_TIME1,barTime);
            ObjectSetDouble(0,llemaName,OBJPROP_PRICE1,zeroLine);
            ObjectSetInteger(0,llemaName,OBJPROP_TIME2,barTime);
            ObjectSetDouble(0,llemaName,OBJPROP_PRICE2,mappedLlemaPrice);
            ObjectSetInteger(0,llemaName,OBJPROP_COLOR,llemaColor);
         }
      }
   } else {
      if(ObjectFind(0,llemaName)>=0) ObjectDelete(0,llemaName);
   }
   string emaLineName=ea_prefix+"obv_emaline_"+timeStr;
   if(isPriceTrackerVisible) {
      if(barIndex+1<ArraySize(state_HarmVol_EMAOsc)) {
         double mappedEma=zeroLine+((state_HarmVol_EMAOsc[barIndex]/maxAbsEmaOsc)*(priceRange*0.11));
         double mappedEmaPrev=zeroLine+((state_HarmVol_EMAOsc[barIndex+1]/maxAbsEmaOsc)*(priceRange*0.11));
         color emaColor=(state_HarmVol_EMAOsc[barIndex]>=0.0)?clrOrange:clrDodgerBlue;
         if(ObjectFind(0,emaLineName)<0) {
            ObjectCreate(0,emaLineName,OBJ_TREND,0,prevTime,mappedEmaPrev,barTime,mappedEma);
            ObjectSetInteger(0,emaLineName,OBJPROP_COLOR,emaColor);
            ObjectSetInteger(0,emaLineName,OBJPROP_WIDTH,1);
            ObjectSetInteger(0,emaLineName,OBJPROP_STYLE,STYLE_DOT);
            ObjectSetInteger(0,emaLineName,OBJPROP_SELECTABLE,false);
            ObjectSetInteger(0,emaLineName,OBJPROP_BACK,false);
            ObjectSetInteger(0,emaLineName,OBJPROP_RAY_RIGHT,false);
            ObjectSetInteger(0,emaLineName,OBJPROP_ZORDER,30004);
         } else {
            ObjectSetInteger(0,emaLineName,OBJPROP_TIME1,prevTime);
            ObjectSetDouble(0,emaLineName,OBJPROP_PRICE1,mappedEmaPrev);
            ObjectSetInteger(0,emaLineName,OBJPROP_TIME2,barTime);
            ObjectSetDouble(0,emaLineName,OBJPROP_PRICE2,mappedEma);
            ObjectSetInteger(0,emaLineName,OBJPROP_COLOR,emaColor);
         }
      }
   } else {
      if(ObjectFind(0,emaLineName)>=0) ObjectDelete(0,emaLineName);
   }
   string roscName=ea_prefix+"obv_rosc_"+timeStr;
   if(isRangeOscVisible) {
      if(barIndex<ArraySize(state_RangeOsc_Val)&&barIndex+1<ArraySize(state_RangeOsc_Val)) {
         double mappedRosc=zeroLine+((state_RangeOsc_Val[barIndex]/maxAbsRangeOsc)*(priceRange*0.11));
         double mappedRoscPrev=zeroLine+((state_RangeOsc_Val[barIndex+1]/maxAbsRangeOsc)*(priceRange*0.11));
         int roscState=(barIndex<ArraySize(state_RangeOsc_State))?state_RangeOsc_State[barIndex]:0;
         color roscColor=C'96,95,113';
         if(roscState==1||roscState==2) roscColor=clrOrange;
         else if(roscState==-1||roscState==-2) roscColor=clrDodgerBlue;
         if(ObjectFind(0,roscName)<0) {
            ObjectCreate(0,roscName,OBJ_TREND,0,prevTime,mappedRoscPrev,barTime,mappedRosc);
            ObjectSetInteger(0,roscName,OBJPROP_COLOR,roscColor);
            ObjectSetInteger(0,roscName,OBJPROP_WIDTH,1);
            ObjectSetInteger(0,roscName,OBJPROP_STYLE,STYLE_SOLID);
            ObjectSetInteger(0,roscName,OBJPROP_SELECTABLE,false);
            ObjectSetInteger(0,roscName,OBJPROP_BACK,false);
            ObjectSetInteger(0,roscName,OBJPROP_RAY_RIGHT,false);
            ObjectSetInteger(0,roscName,OBJPROP_ZORDER,30005);
         } else {
            ObjectSetInteger(0,roscName,OBJPROP_TIME1,prevTime);
            ObjectSetDouble(0,roscName,OBJPROP_PRICE1,mappedRoscPrev);
            ObjectSetInteger(0,roscName,OBJPROP_TIME2,barTime);
            ObjectSetDouble(0,roscName,OBJPROP_PRICE2,mappedRosc);
            ObjectSetInteger(0,roscName,OBJPROP_COLOR,roscColor);
         }
      }
   } else {
      if(ObjectFind(0,roscName)>=0) ObjectDelete(0,roscName);
   }
   DrawVolumeDotMatrix(barIndex);
}
void UpdateOBVPositions() {
   if(!isOBVVisualsVisible) return;
   double priceRange=WindowPriceMax()-WindowPriceMin();
   if(priceRange<=0) return;
   int leftBar=WindowFirstVisibleBar()+100;
   if(leftBar>=Bars) leftBar=Bars-1;
   datetime leftTime=Time[leftBar];
   datetime rightTime=Time[0];
   string bgName=ea_prefix+"obv_bg_pane";
   if(ObjectFind(0,bgName)>=0) ObjectDelete(0,bgName);
   ObjectCreate(0,bgName,OBJ_RECTANGLE,0,0,0);
   ObjectSetInteger(0,bgName,OBJPROP_COLOR,C'15,25,38');
   ObjectSetInteger(0,bgName,OBJPROP_BGCOLOR,C'15,25,38');
   ObjectSetInteger(0,bgName,OBJPROP_STYLE,STYLE_SOLID);
   ObjectSetInteger(0,bgName,OBJPROP_WIDTH,1);
   ObjectSetInteger(0,bgName,OBJPROP_FILL,true);
   ObjectSetInteger(0,bgName,OBJPROP_BACK,true);
   ObjectSetInteger(0,bgName,OBJPROP_ZORDER,30000);
   ObjectMove(0,bgName,0,leftTime,WindowPriceMin());
   ObjectMove(0,bgName,1,rightTime,WindowPriceMin()+(priceRange*0.25));
   string borderName=ea_prefix+"obv_top_border";
   if(ObjectFind(0,borderName)>=0) {
      ObjectSetInteger(0,borderName,OBJPROP_TIME1,leftTime);
      ObjectSetDouble(0,borderName,OBJPROP_PRICE1,WindowPriceMin()+(priceRange*0.25));
      ObjectSetInteger(0,borderName,OBJPROP_TIME2,rightTime);
      ObjectSetDouble(0,borderName,OBJPROP_PRICE2,WindowPriceMin()+(priceRange*0.25));
   }
   double zeroLine=WindowPriceMin()+(priceRange*0.125);
   string zeroName=ea_prefix+"obv_zero_line";
   if(ObjectFind(0,zeroName)>=0) {
      ObjectSetInteger(0,zeroName,OBJPROP_TIME1,leftTime);
      ObjectSetDouble(0,zeroName,OBJPROP_PRICE1,zeroLine);
      ObjectSetInteger(0,zeroName,OBJPROP_TIME2,rightTime);
      ObjectSetDouble(0,zeroName,OBJPROP_PRICE2,zeroLine);
   }
   double maxAbsDelta=0.0;
   double maxAbsLlema=0.0;
   double maxAbsEmaOsc=0.0;
   double maxAbsRangeOsc=0.0;
   int visibleBars=WindowFirstVisibleBar();
   int limit=MathMin(Bars-1,visibleBars);
   int rightBar=(int)MathMax(0.0,(double)(visibleBars-WindowBarsPerChart()+1));
   for(int k=rightBar; k<=limit; k++) {
      if(k<ArraySize(state_OBV_Final)) {
         double absVal=MathAbs(state_OBV_Final[k]);
         if(absVal>maxAbsDelta) maxAbsDelta=absVal;
      }
      if(k<ArraySize(state_HarmVol_LLEMA)) {
         double absLlema=MathAbs(state_HarmVol_LLEMA[k]);
         if(absLlema>maxAbsLlema) maxAbsLlema=absLlema;
      }
      if(k<ArraySize(state_HarmVol_EMAOsc)) {
         double absEma=MathAbs(state_HarmVol_EMAOsc[k]);
         if(absEma>maxAbsEmaOsc) maxAbsEmaOsc=absEma;
      }
      if(k<ArraySize(state_RangeOsc_Val)) {
         double absRosc=MathAbs(state_RangeOsc_Val[k]);
         if(absRosc>maxAbsRangeOsc) maxAbsRangeOsc=absRosc;
      }
   }
   if(maxAbsDelta==0.0) maxAbsDelta=1.0;
   if(maxAbsLlema==0.0) maxAbsLlema=1.0;
   if(maxAbsEmaOsc==0.0) maxAbsEmaOsc=1.0;
   if(maxAbsRangeOsc==0.0) maxAbsRangeOsc=1.0;
   for(int i=ObjectsTotal(0,-1,-1)-1; i>=0; i--) {
      string name=ObjectName(0,i,-1,-1);
      if(StringFind(name,ea_prefix+"obv_signal_")==0) {
         if(!isOBVfLineVisible) { ObjectDelete(0,name); continue; }
         datetime t2=(datetime)ObjectGetInteger(0,name,OBJPROP_TIME2);
         int barIdx=iBarShift(NULL,0,t2);
         if(barIdx<0||barIdx+1>=Bars||barIdx+1>=ArraySize(state_OBV_Final)) continue;
         double mappedPrice=zeroLine+((state_OBV_Final[barIdx]/maxAbsDelta)*(priceRange*0.11));
         double mappedPricePrev=zeroLine+((state_OBV_Final[barIdx+1]/maxAbsDelta)*(priceRange*0.11));
         double currentP1=ObjectGetDouble(0,name,OBJPROP_PRICE1);
         double currentP2=ObjectGetDouble(0,name,OBJPROP_PRICE2);
         if(MathAbs(currentP1-mappedPricePrev)>Point||MathAbs(currentP2-mappedPrice)>Point) {
            ObjectSetDouble(0,name,OBJPROP_PRICE1,mappedPricePrev);
            ObjectSetDouble(0,name,OBJPROP_PRICE2,mappedPrice);
         }
      }
      else if(StringFind(name,ea_prefix+"obv_llema_")==0) {
         if(!isKamaHistoVisible) { ObjectDelete(0,name); continue; }
         datetime t2=(datetime)ObjectGetInteger(0,name,OBJPROP_TIME2);
         int barIdx=iBarShift(NULL,0,t2);
         if(barIdx<0||barIdx>=ArraySize(state_HarmVol_LLEMA)) continue;
         if(state_HarmVol_LLEMA[barIdx]==0.0) { ObjectDelete(0,name); continue; }
         double mappedLlemaPrice=zeroLine+((state_HarmVol_LLEMA[barIdx]/maxAbsLlema)*(priceRange*0.11));
         double currentP1=ObjectGetDouble(0,name,OBJPROP_PRICE1);
         double currentP2=ObjectGetDouble(0,name,OBJPROP_PRICE2);
         if(MathAbs(currentP1-zeroLine)>Point||MathAbs(currentP2-mappedLlemaPrice)>Point) {
            ObjectSetDouble(0,name,OBJPROP_PRICE1,zeroLine);
            ObjectSetDouble(0,name,OBJPROP_PRICE2,mappedLlemaPrice);
         }
      }
      else if(StringFind(name,ea_prefix+"obv_emaline_")==0) {
         if(!isPriceTrackerVisible) { ObjectDelete(0,name); continue; }
         datetime t2=(datetime)ObjectGetInteger(0,name,OBJPROP_TIME2);
         int barIdx=iBarShift(NULL,0,t2);
         if(barIdx<0||barIdx+1>=Bars||barIdx+1>=ArraySize(state_HarmVol_EMAOsc)) continue;
         double mappedEma=zeroLine+((state_HarmVol_EMAOsc[barIdx]/maxAbsEmaOsc)*(priceRange*0.11));
         double mappedEmaPrev=zeroLine+((state_HarmVol_EMAOsc[barIdx+1]/maxAbsEmaOsc)*(priceRange*0.11));
         double currentP1=ObjectGetDouble(0,name,OBJPROP_PRICE1);
         double currentP2=ObjectGetDouble(0,name,OBJPROP_PRICE2);
         if(MathAbs(currentP1-mappedEmaPrev)>Point||MathAbs(currentP2-mappedEma)>Point) {
            ObjectSetDouble(0,name,OBJPROP_PRICE1,mappedEmaPrev);
            ObjectSetDouble(0,name,OBJPROP_PRICE2,mappedEma);
         }
      }
      else if(StringFind(name,ea_prefix+"obv_rosc_")==0) {
         if(!isRangeOscVisible) { ObjectDelete(0,name); continue; }
         datetime t2=(datetime)ObjectGetInteger(0,name,OBJPROP_TIME2);
         int barIdx=iBarShift(NULL,0,t2);
         if(barIdx<0||barIdx+1>=Bars||barIdx+1>=ArraySize(state_RangeOsc_Val)) continue;
         double mappedRosc=zeroLine+((state_RangeOsc_Val[barIdx]/maxAbsRangeOsc)*(priceRange*0.11));
         double mappedRoscPrev=zeroLine+((state_RangeOsc_Val[barIdx+1]/maxAbsRangeOsc)*(priceRange*0.11));
         double currentP1=ObjectGetDouble(0,name,OBJPROP_PRICE1);
         double currentP2=ObjectGetDouble(0,name,OBJPROP_PRICE2);
         if(MathAbs(currentP1-mappedRoscPrev)>Point||MathAbs(currentP2-mappedRosc)>Point) {
            ObjectSetDouble(0,name,OBJPROP_PRICE1,mappedRoscPrev);
            ObjectSetDouble(0,name,OBJPROP_PRICE2,mappedRosc);
         }
      }
   }
   UpdateVolumeDotMatrixPositions();
}
void DeleteOBV_Visuals() {
   for(int i=ObjectsTotal(0,-1,-1)-1; i>=0; i--) {
      string objName=ObjectName(0,i,-1,-1);
      if(StringFind(objName,ea_prefix+"obv_")==0) ObjectDelete(0,objName);
   }
   DeleteVolumeDotMatrix();
}
void DrawLiveOBVSegment() {
   if(!isOBVVisualsVisible) return;
   if(Bars<2||ArraySize(state_OBV_Final)<2||ArraySize(state_HarmVol_LLEMA)<2) return;
   double priceRange=WindowPriceMax()-WindowPriceMin();
   if(priceRange<=0) return;
   string bgName=ea_prefix+"obv_bg_pane";
   if(ObjectFind(0,bgName)<0) {
      ObjectCreate(0,bgName,OBJ_RECTANGLE,0,0,0);
      ObjectSetInteger(0,bgName,OBJPROP_COLOR,C'15,25,38');
      ObjectSetInteger(0,bgName,OBJPROP_BGCOLOR,C'15,25,38');
      ObjectSetInteger(0,bgName,OBJPROP_STYLE,STYLE_SOLID);
      ObjectSetInteger(0,bgName,OBJPROP_WIDTH,1);
      ObjectSetInteger(0,bgName,OBJPROP_FILL,true);
      ObjectSetInteger(0,bgName,OBJPROP_BACK,true);
      ObjectSetInteger(0,bgName,OBJPROP_ZORDER,30000);
   }
   int leftBar=WindowFirstVisibleBar()+100;
   if(leftBar>=Bars) leftBar=Bars-1;
   datetime leftTime=Time[leftBar];
   datetime rightTime=Time[0];
   ObjectMove(0,bgName,0,leftTime,WindowPriceMin());
   ObjectMove(0,bgName,1,rightTime,WindowPriceMin()+(priceRange*0.25));
   string borderName=ea_prefix+"obv_top_border";
   if(ObjectFind(0,borderName)<0) {
      ObjectCreate(0,borderName,OBJ_TREND,0,leftTime,WindowPriceMin()+(priceRange*0.25),rightTime,WindowPriceMin()+(priceRange*0.25));
      ObjectSetInteger(0,borderName,OBJPROP_COLOR,C'95,107,119');
      ObjectSetInteger(0,borderName,OBJPROP_WIDTH,1);
      ObjectSetInteger(0,borderName,OBJPROP_STYLE,STYLE_SOLID);
      ObjectSetInteger(0,borderName,OBJPROP_RAY_RIGHT,true);
      ObjectSetInteger(0,borderName,OBJPROP_BACK,false);
      ObjectSetInteger(0,borderName,OBJPROP_ZORDER,30001);
   } else {
      ObjectSetInteger(0,borderName,OBJPROP_TIME1,leftTime);
      ObjectSetDouble(0,borderName,OBJPROP_PRICE1,WindowPriceMin()+(priceRange*0.25));
      ObjectSetInteger(0,borderName,OBJPROP_TIME2,rightTime);
      ObjectSetDouble(0,borderName,OBJPROP_PRICE2,WindowPriceMin()+(priceRange*0.25));
   }
   double zeroLine=WindowPriceMin()+(priceRange*0.125);
   string zeroName=ea_prefix+"obv_zero_line";
   if(ObjectFind(0,zeroName)<0) {
      ObjectCreate(0,zeroName,OBJ_TREND,0,leftTime,zeroLine,rightTime,zeroLine);
      ObjectSetInteger(0,zeroName,OBJPROP_COLOR,C'96,95,113');
      ObjectSetInteger(0,zeroName,OBJPROP_RAY_RIGHT,true);
      ObjectSetInteger(0,zeroName,OBJPROP_BACK,false);
      ObjectSetInteger(0,zeroName,OBJPROP_ZORDER,30001);
   } else {
      ObjectSetInteger(0,zeroName,OBJPROP_TIME1,leftTime);
      ObjectSetDouble(0,zeroName,OBJPROP_PRICE1,zeroLine);
      ObjectSetInteger(0,zeroName,OBJPROP_TIME2,rightTime);
      ObjectSetDouble(0,zeroName,OBJPROP_PRICE2,zeroLine);
   }
   double maxAbsDelta=0.0;
   double maxAbsLlema=0.0;
   double maxAbsEmaOsc=0.0;
   double maxAbsRangeOsc=0.0;
   int visibleBars=WindowFirstVisibleBar();
   int limit=MathMin(Bars-1,visibleBars);
   int rightBar=(int)MathMax(0.0,(double)(visibleBars-WindowBarsPerChart()+1));
   for(int k=rightBar; k<=limit; k++) {
      if(k<ArraySize(state_OBV_Final)) {
         double absVal=MathAbs(state_OBV_Final[k]);
         if(absVal>maxAbsDelta) maxAbsDelta=absVal;
      }
      if(k<ArraySize(state_HarmVol_LLEMA)) {
         double absLlema=MathAbs(state_HarmVol_LLEMA[k]);
         if(absLlema>maxAbsLlema) maxAbsLlema=absLlema;
      }
      if(k<ArraySize(state_HarmVol_EMAOsc)) {
         double absEma=MathAbs(state_HarmVol_EMAOsc[k]);
         if(absEma>maxAbsEmaOsc) maxAbsEmaOsc=absEma;
      }
      if(k<ArraySize(state_RangeOsc_Val)) {
         double absRosc=MathAbs(state_RangeOsc_Val[k]);
         if(absRosc>maxAbsRangeOsc) maxAbsRangeOsc=absRosc;
      }
   }
   if(maxAbsDelta==0.0) maxAbsDelta=1.0;
   if(maxAbsLlema==0.0) maxAbsLlema=1.0;
   if(maxAbsEmaOsc==0.0) maxAbsEmaOsc=1.0;
   if(maxAbsRangeOsc==0.0) maxAbsRangeOsc=1.0;
   datetime barTime=Time[0];
   datetime prevTime=Time[1];
   string signalName=ea_prefix+"obv_signal_LIVE";
   if(isOBVfLineVisible) {
      double mappedPrice=zeroLine+((state_OBV_Final[0]/maxAbsDelta)*(priceRange*0.11));
      double mappedPricePrev=zeroLine+((state_OBV_Final[1]/maxAbsDelta)*(priceRange*0.11));
      color signalColor=(state_TChan_OC[0]==1.0)?clrOrange:clrDodgerBlue;
      if(ObjectFind(0,signalName)<0) {
         ObjectCreate(0,signalName,OBJ_TREND,0,prevTime,mappedPricePrev,barTime,mappedPrice);
         ObjectSetInteger(0,signalName,OBJPROP_COLOR,signalColor);
         ObjectSetInteger(0,signalName,OBJPROP_WIDTH,2);
         ObjectSetInteger(0,signalName,OBJPROP_STYLE,STYLE_SOLID);
         ObjectSetInteger(0,signalName,OBJPROP_SELECTABLE,false);
         ObjectSetInteger(0,signalName,OBJPROP_BACK,false);
         ObjectSetInteger(0,signalName,OBJPROP_RAY_RIGHT,false);
         ObjectSetInteger(0,signalName,OBJPROP_ZORDER,30002);
      } else {
         ObjectSetInteger(0,signalName,OBJPROP_TIME1,prevTime);
         ObjectSetDouble(0,signalName,OBJPROP_PRICE1,mappedPricePrev);
         ObjectSetInteger(0,signalName,OBJPROP_TIME2,barTime);
         ObjectSetDouble(0,signalName,OBJPROP_PRICE2,mappedPrice);
         ObjectSetInteger(0,signalName,OBJPROP_COLOR,signalColor);
      }
   } else {
      if(ObjectFind(0,signalName)>=0) ObjectDelete(0,signalName);
   }
   string llemaName=ea_prefix+"obv_llema_LIVE";
   if(isKamaHistoVisible) {
      if(state_HarmVol_LLEMA[0]==0.0) {
         if(ObjectFind(0,llemaName)>=0) ObjectDelete(0,llemaName);
      } else {
         double mappedLlemaPrice=zeroLine+((state_HarmVol_LLEMA[0]/maxAbsLlema)*(priceRange*0.11));
         color llemaColor=(state_HarmVol_LLEMA[0]>0.0)?C'146,134,124':C'89,116,124';
         if(ObjectFind(0,llemaName)<0) {
            ObjectCreate(0,llemaName,OBJ_TREND,0,barTime,zeroLine,barTime,mappedLlemaPrice);
            ObjectSetInteger(0,llemaName,OBJPROP_COLOR,llemaColor);
            ObjectSetInteger(0,llemaName,OBJPROP_WIDTH,3);
            ObjectSetInteger(0,llemaName,OBJPROP_STYLE,STYLE_SOLID);
            ObjectSetInteger(0,llemaName,OBJPROP_SELECTABLE,false);
            ObjectSetInteger(0,llemaName,OBJPROP_BACK,false);
            ObjectSetInteger(0,llemaName,OBJPROP_RAY_RIGHT,false);
            ObjectSetInteger(0,llemaName,OBJPROP_ZORDER,30003);
         } else {
            ObjectSetInteger(0,llemaName,OBJPROP_TIME1,barTime);
            ObjectSetDouble(0,llemaName,OBJPROP_PRICE1,zeroLine);
            ObjectSetInteger(0,llemaName,OBJPROP_TIME2,barTime);
            ObjectSetDouble(0,llemaName,OBJPROP_PRICE2,mappedLlemaPrice);
            ObjectSetInteger(0,llemaName,OBJPROP_COLOR,llemaColor);
         }
      }
   } else {
      if(ObjectFind(0,llemaName)>=0) ObjectDelete(0,llemaName);
   }
   string emaLineName=ea_prefix+"obv_emaline_LIVE";
   if(isPriceTrackerVisible) {
      if(ArraySize(state_HarmVol_EMAOsc)>=2) {
         double mappedEma=zeroLine+((state_HarmVol_EMAOsc[0]/maxAbsEmaOsc)*(priceRange*0.11));
         double mappedEmaPrev=zeroLine+((state_HarmVol_EMAOsc[1]/maxAbsEmaOsc)*(priceRange*0.11));
         color emaColor=(state_HarmVol_EMAOsc[0]>=0.0)?clrOrange:clrDodgerBlue;
         if(ObjectFind(0,emaLineName)<0) {
            ObjectCreate(0,emaLineName,OBJ_TREND,0,prevTime,mappedEmaPrev,barTime,mappedEma);
            ObjectSetInteger(0,emaLineName,OBJPROP_COLOR,emaColor);
            ObjectSetInteger(0,emaLineName,OBJPROP_WIDTH,1);
            ObjectSetInteger(0,emaLineName,OBJPROP_STYLE,STYLE_DOT);
            ObjectSetInteger(0,emaLineName,OBJPROP_SELECTABLE,false);
            ObjectSetInteger(0,emaLineName,OBJPROP_BACK,false);
            ObjectSetInteger(0,emaLineName,OBJPROP_RAY_RIGHT,false);
            ObjectSetInteger(0,emaLineName,OBJPROP_ZORDER,30004);
         } else {
            ObjectSetInteger(0,emaLineName,OBJPROP_TIME1,prevTime);
            ObjectSetDouble(0,emaLineName,OBJPROP_PRICE1,mappedEmaPrev);
            ObjectSetInteger(0,emaLineName,OBJPROP_TIME2,barTime);
            ObjectSetDouble(0,emaLineName,OBJPROP_PRICE2,mappedEma);
            ObjectSetInteger(0,emaLineName,OBJPROP_COLOR,emaColor);
         }
      }
   } else {
      if(ObjectFind(0,emaLineName)>=0) ObjectDelete(0,emaLineName);
   }
   string roscNameLive=ea_prefix+"obv_rosc_LIVE";
   if(isRangeOscVisible) {
      if(ArraySize(state_RangeOsc_Val)>=2) {
         double mappedRosc=zeroLine+((state_RangeOsc_Val[0]/maxAbsRangeOsc)*(priceRange*0.11));
         double mappedRoscPrev=zeroLine+((state_RangeOsc_Val[1]/maxAbsRangeOsc)*(priceRange*0.11));
         int roscState=(ArraySize(state_RangeOsc_State)>=1)?state_RangeOsc_State[0]:0;
         color roscColor=C'96,95,113';
         if(roscState==1||roscState==2) roscColor=clrOrange;
         else if(roscState==-1||roscState==-2) roscColor=clrDodgerBlue;
         if(ObjectFind(0,roscNameLive)<0) {
            ObjectCreate(0,roscNameLive,OBJ_TREND,0,prevTime,mappedRoscPrev,barTime,mappedRosc);
            ObjectSetInteger(0,roscNameLive,OBJPROP_COLOR,roscColor);
            ObjectSetInteger(0,roscNameLive,OBJPROP_WIDTH,1);
            ObjectSetInteger(0,roscNameLive,OBJPROP_STYLE,STYLE_SOLID);
            ObjectSetInteger(0,roscNameLive,OBJPROP_SELECTABLE,false);
            ObjectSetInteger(0,roscNameLive,OBJPROP_BACK,false);
            ObjectSetInteger(0,roscNameLive,OBJPROP_RAY_RIGHT,false);
            ObjectSetInteger(0,roscNameLive,OBJPROP_ZORDER,30005);
         } else {
            ObjectSetInteger(0,roscNameLive,OBJPROP_TIME1,prevTime);
            ObjectSetDouble(0,roscNameLive,OBJPROP_PRICE1,mappedRoscPrev);
            ObjectSetInteger(0,roscNameLive,OBJPROP_TIME2,barTime);
            ObjectSetDouble(0,roscNameLive,OBJPROP_PRICE2,mappedRosc);
            ObjectSetInteger(0,roscNameLive,OBJPROP_COLOR,roscColor);
         }
      }
   } else {
      if(ObjectFind(0,roscNameLive)>=0) ObjectDelete(0,roscNameLive);
   }
   DrawVolumeDotMatrix(0);
}
//+------------------------------------------------------------------+
//| SECTION 7.8 - UI ANIMATION & TICK TRACKING                       |
//+------------------------------------------------------------------+
bool GetHeaderToggleState() {
   return (GetTickCount()/2000)%2==0;
}
double GetCurrentMarketSpreadUSD(double lots) {
   RefreshRates();
   double spreadPoints=MarketInfo(Symbol(),MODE_SPREAD);
   double tickValue=MarketInfo(Symbol(),MODE_TICKVALUE);
   double tickSize=MarketInfo(Symbol(),MODE_TICKSIZE);
   if(tickSize<=0) return 0.0;
   return (spreadPoints*Point*tickValue/tickSize)*lots;
}
//+------------------------------------------------------------------+
//| SECTION 7.9 - VOLUME DOT MATRIX VISUALS                          |
//+------------------------------------------------------------------+
void DrawVolumeDotMatrix(int barIndex) {
   if(barIndex<0||barIndex>=Bars) return;
   double vol=0.0;
   if(barIndex<ArraySize(hist_VolumeValue)) vol=hist_VolumeValue[barIndex];
   if(vol<=0.0&&Volume[barIndex]>0) vol=(double)Volume[barIndex];
   double tierGates[12];
   tierGates[0]=Tier1_Vol;
   tierGates[1]=Tier2_Vol;
   tierGates[2]=Tier3_Vol;
   tierGates[3]=Tier4_Vol;
   tierGates[4]=Tier5_Vol;
   tierGates[5]=Tier6_Vol;
   tierGates[6]=Tier7_Vol;
   tierGates[7]=Tier8_Vol;
   tierGates[8]=Tier9_Vol;
   tierGates[9]=Tier10_Vol;
   tierGates[10]=Tier11_Vol;
   tierGates[11]=Tier12_Vol;
   int activeTierCount=0;
   int tiersCleared=0;
   for(int t=0; t<12; t++) {
      if(tierGates[t]>0.0) {
         activeTierCount++;
         if(vol>=tierGates[t]) tiersCleared++;
      }
   }
   int dotCount=0;
   if(activeTierCount>0&&tiersCleared>0) {
      dotCount=(int)MathCeil((double)tiersCleared*5.0/(double)activeTierCount);
      if(dotCount>5) dotCount=5;
   }
   double priceRange=WindowPriceMax()-WindowPriceMin();
   if(priceRange<=0) return;
   int chartHeight=(int)ChartGetInteger(0,CHART_HEIGHT_IN_PIXELS);
   if(chartHeight<=0) return;
   double p2p=priceRange/(double)chartHeight;
   double basePrice=WindowPriceMin()+(priceRange*0.25)+(p2p*13.0);
   double dotSpacing=p2p*10.0;
   color dotColor=(Close[barIndex]>=Close[barIndex+1])?C'146,134,124':C'89,116,124';
   string timeSuffix=TimeToString(Time[barIndex],TIME_DATE|TIME_MINUTES|TIME_SECONDS);
   for(int d=0; d<5; d++) {
      string dotName=ea_prefix+"vdot_"+IntegerToString(d)+"_"+timeSuffix;
      if(d<dotCount) {
         double dotPrice=basePrice+(dotSpacing*(double)d);
         if(ObjectFind(0,dotName)<0) {
            ObjectCreate(0,dotName,OBJ_ARROW,0,Time[barIndex],dotPrice);
            ObjectSetInteger(0,dotName,OBJPROP_ARROWCODE,159);
            ObjectSetInteger(0,dotName,OBJPROP_WIDTH,1);
            ObjectSetInteger(0,dotName,OBJPROP_COLOR,dotColor);
            ObjectSetInteger(0,dotName,OBJPROP_ANCHOR,ANCHOR_CENTER);
            ObjectSetInteger(0,dotName,OBJPROP_BACK,false);
            ObjectSetInteger(0,dotName,OBJPROP_SELECTABLE,false);
            ObjectSetInteger(0,dotName,OBJPROP_ZORDER,30004);
         } else {
            ObjectSetDouble(0,dotName,OBJPROP_PRICE1,dotPrice);
            ObjectSetInteger(0,dotName,OBJPROP_COLOR,dotColor);
         }
      } else {
         if(ObjectFind(0,dotName)>=0) ObjectDelete(0,dotName);
      }
   }
}
void UpdateVolumeDotMatrixPositions() {
   double priceRange=WindowPriceMax()-WindowPriceMin();
   if(priceRange<=0) return;
   int chartHeight=(int)ChartGetInteger(0,CHART_HEIGHT_IN_PIXELS);
   if(chartHeight<=0) return;
   double p2p=priceRange/(double)chartHeight;
   double basePrice=WindowPriceMin()+(priceRange*0.25)+(p2p*13.0);
   double dotSpacing=p2p*10.0;
   for(int i=ObjectsTotal(0,-1,-1)-1; i>=0; i--) {
      string name=ObjectName(0,i,-1,-1);
      if(StringFind(name,ea_prefix+"vdot_")!=0) continue;
      datetime t1=(datetime)ObjectGetInteger(0,name,OBJPROP_TIME1);
      int barIdx=iBarShift(NULL,0,t1);
      if(barIdx<0||barIdx>=Bars) continue;
      int dotIdx=(int)StringToInteger(StringSubstr(name,(int)StringLen(ea_prefix)+5,1));
      double dotPrice=basePrice+(dotSpacing*(double)dotIdx);
      double currentPrice=ObjectGetDouble(0,name,OBJPROP_PRICE1);
      if(MathAbs(currentPrice-dotPrice)>Point) {
         ObjectSetDouble(0,name,OBJPROP_PRICE1,dotPrice);
      }
   }
}
void DeleteVolumeDotMatrix() {
   for(int i=ObjectsTotal(0,-1,-1)-1; i>=0; i--) {
      string objName=ObjectName(0,i,-1,-1);
      if(StringFind(objName,ea_prefix+"vdot_")==0) ObjectDelete(0,objName);
   }
}
//+------------------------------------------------------------------+
//| SECTION 7.10 - DOTS VISUAL PANEL                                  |
//+------------------------------------------------------------------+
#define DOTS_LOG_MAX 20
#define DOTS_GRID_COLS 38
#define DOTS_GRID_ROWS 42
string dots_log_text[DOTS_LOG_MAX];
color  dots_log_clr[DOTS_LOG_MAX];
int    dots_log_count=0;
double dots_gross_win=0.0;
double dots_gross_loss=0.0;
int dots_grid_left[DOTS_GRID_COLS]={39,4,41,2,28,64,66,11,52,10,5,30,31,34,55,36,48,45,13,40,27,53,57,21,26,56,49,47,60,9,3,63,73,65,59,71,25,24};
int dots_grid_right[DOTS_GRID_COLS]={68,69,44,23,20,75,8,72,15,50,61,7,6,54,33,29,67,35,37,70,18,51,12,74,32,16,58,19,0,17,38,22,46,62,43,1,42,14};
int dots_grid_lhdr[4]={0,17,21,29};
int dots_grid_rhdr[4]={0,9,31,35};
string dots_grid_lnames[4]={"TREND CONTINUATION (16)","TREND EXHAUSTION (3)","SQUEEZE BREAKOUT (7)","STRUCTURAL ENTRY (12)"};
string dots_grid_rnames[4]={"BREAKOUT EXPANSION (8)","MOMENTUM IGNITION (21)","VOLUME CONFIRMED (3)","PRICE ACTION (6)"};
void InitDotsSignalNames() {
   dots_ruleName[0]="MI_OBVDivergence_S";
   dots_ruleName[1]="PA_VolCompression_L";
   dots_ruleName[2]="TC_PullbackLiquid_L";
   dots_ruleName[3]="SE_DecisiveBreak_L";
   dots_ruleName[4]="TC_PullbackRebuild_L";
   dots_ruleName[5]="TC_DecelConviction_L";
   dots_ruleName[6]="MI_OverextBuyFlow_L";
   dots_ruleName[7]="MI_OversoldActive_L";
   dots_ruleName[8]="BX_VolConfirm_L";
   dots_ruleName[9]="SE_ThinBreakout_L";
   dots_ruleName[10]="TC_ActivePullback_L";
   dots_ruleName[11]="TC_BearPullback_L";
   dots_ruleName[12]="MI_SlopeAccel_L";
   dots_ruleName[13]="TE_PullbackRebuild_S";
   dots_ruleName[14]="PA_QuietConviction_L";
   dots_ruleName[15]="MI_PersistMomo_S";
   dots_ruleName[16]="MI_OBVBearish_L";
   dots_ruleName[17]="VC_FlowReversal_L";
   dots_ruleName[18]="MI_NearKAMATurn_S";
   dots_ruleName[19]="MI_QuietGapUp_S";
   dots_ruleName[20]="BX_StableStretch_S";
   dots_ruleName[21]="SB_HighConviction_S";
   dots_ruleName[22]="VC_VolConfirm_S";
   dots_ruleName[23]="BX_SqueezeRelease_S";
   dots_ruleName[24]="SE_PullbackRebuild_S";
   dots_ruleName[25]="SE_SmoothWideSpread_L";
   dots_ruleName[26]="SB_SqueezeRelease_L";
   dots_ruleName[27]="SB_ChoppySetup_L";
   dots_ruleName[28]="TC_ThinSelloff_L";
   dots_ruleName[29]="MI_KAMAStretch_L";
   dots_ruleName[30]="TC_SmoothDecel_L";
   dots_ruleName[31]="TC_ThinLiquidity_S";
   dots_ruleName[32]="MI_PullbackRebuild_S";
   dots_ruleName[33]="MI_StretchedImpulse_S";
   dots_ruleName[34]="TC_UncertainShift_S";
   dots_ruleName[35]="MI_FullAlignment_S";
   dots_ruleName[36]="TC_OrderImbalance_S";
   dots_ruleName[37]="MI_PullbackRebuild_L";
   dots_ruleName[38]="VC_VolConfirm_L";
   dots_ruleName[39]="TC_PersistMomo_S";
   dots_ruleName[40]="SB_PullbackRebuild_S";
   dots_ruleName[41]="TC_EfficientThin_S";
   dots_ruleName[42]="PA_ThinLiquidity_L";
   dots_ruleName[43]="PA_RejectionGap_S";
   dots_ruleName[44]="BX_SqueezeRelease_S2";
   dots_ruleName[45]="TE_PullbackRebuild_S2";
   dots_ruleName[46]="PA_SmoothGapReversal_L";
   dots_ruleName[47]="SE_SlopeAccel_S";
   dots_ruleName[48]="TE_AccelTrend_S";
   dots_ruleName[49]="SE_StableBearPress_S";
   dots_ruleName[50]="MI_FreshFlowLiquid_S";
   dots_ruleName[51]="MI_FreshImpulse_L";
   dots_ruleName[52]="TC_RetailReversal_L";
   dots_ruleName[53]="SB_ErraticBuild_L";
   dots_ruleName[54]="MI_KAMAStretch_L2";
   dots_ruleName[55]="TC_JaggedPullback_L";
   dots_ruleName[56]="SB_OBVMomoSplit_S";
   dots_ruleName[57]="SB_VolConfirm_S";
   dots_ruleName[58]="MI_OBVBearish_S";
   dots_ruleName[59]="SE_VolatileTrend_S";
   dots_ruleName[60]="SE_MatureToxic_L";
   dots_ruleName[61]="MI_OBVBearish_L2";
   dots_ruleName[62]="PA_FadingThrust_S";
   dots_ruleName[63]="SE_PoCPersist_L";
   dots_ruleName[64]="TC_RangeExtended_S";
   dots_ruleName[65]="SE_FreshFlip_S";
   dots_ruleName[66]="TC_RangeExtended_L";
   dots_ruleName[67]="MI_SlopeAccel_S";
   dots_ruleName[68]="BX_RangeShift_L";
   dots_ruleName[69]="BX_SlowCleanBreak_L";
   dots_ruleName[70]="MI_ClearUptrend_L";
   dots_ruleName[71]="SE_QuietDowntrend_S";
   dots_ruleName[72]="BX_StableCompress_S";
   dots_ruleName[73]="SE_RangeFloor_S";
   dots_ruleName[74]="MI_InformedFlow_L";
   dots_ruleName[75]="BX_QuietMeanRevert_L";
}
color DotsLogColor(string msg) {
   if(StringFind(msg,"BOOT:")>=0) return C'95,107,119';
   if(StringFind(msg,"EVAL:")>=0) return C'96,95,113';
   if(StringFind(msg,"GATE: PASS")>=0) return C'146,134,124';
   if(StringFind(msg,"GATE:")>=0) return C'96,95,113';
   if(StringFind(msg,"OPEN:")>=0) return C'146,134,124';
   if(StringFind(msg,"BE:")>=0) return C'89,116,124';
   if(StringFind(msg,"LF:")>=0) return C'89,116,124';
   if(StringFind(msg,"T+")>=0) return C'95,107,119';
   if(StringFind(msg,"EXIT: +")>=0) return C'146,134,124';
   if(StringFind(msg,"EXIT:")>=0) return C'96,95,113';
   if(StringFind(msg,"CAP:")>=0) return C'96,95,113';
   if(StringFind(msg,"CLOSE:")>=0) return C'96,95,113';
   if(StringFind(msg,"LIVE:")>=0) return C'146,134,124';
   return C'146,134,124';
}
void DotsLog(string msg) {
   datetime estNow=GetEstTime();
   string ts=StringFormat("%02d:%02d",TimeHour(estNow),TimeMinute(estNow));
   string line=ts+" "+msg;
   color clr=DotsLogColor(msg);
   if(dots_log_count<DOTS_LOG_MAX) {
      dots_log_text[dots_log_count]=line;
      dots_log_clr[dots_log_count]=clr;
      dots_log_count++;
   } else {
      for(int i=0;i<DOTS_LOG_MAX-1;i++) {
         dots_log_text[i]=dots_log_text[i+1];
         dots_log_clr[i]=dots_log_clr[i+1];
      }
      dots_log_text[DOTS_LOG_MAX-1]=line;
      dots_log_clr[DOTS_LOG_MAX-1]=clr;
   }
   DotsRenderLog();
}
void DotsRenderLog() {
   string P=ea_prefix+"Dots_";
   int margin=15;
   int c3W=200;
   int c2W=260;
   int gap=3;
   int hdrH=28;
   int pad=6;
   int lineH=12;
   int c1W=200;
   int c1X=margin+c3W+gap+c2W+gap;
   int chartH=(int)ChartGetInteger(0,CHART_HEIGHT_IN_PIXELS);
   int topY=45;
   int panelH=chartH-topY-15;
   if(panelH<200) panelH=200;
   int contentTop=topY+hdrH+pad;
   int contentBot=topY+panelH-pad;
   int visibleLines=(contentBot-contentTop)/lineH;
   if(visibleLines>DOTS_LOG_MAX) visibleLines=DOTS_LOG_MAX;
   if(visibleLines<1) visibleLines=1;
   int lx=c1X+c1W-pad;
   int startIdx=dots_log_count-visibleLines;
   if(startIdx<0) startIdx=0;
   for(int v=0;v<visibleLines;v++) {
      string objName=P+"Log_"+IntegerToString(v);
      int idx=startIdx+v;
      int yPos=contentTop+(v*lineH);
      if(idx<dots_log_count) {
         if(ObjectFind(0,objName)<0) {
            ObjectCreate(0,objName,OBJ_LABEL,0,0,0);
            ObjectSetInteger(0,objName,OBJPROP_CORNER,CORNER_RIGHT_UPPER);
            ObjectSetString(0,objName,OBJPROP_FONT,"Consolas");
            ObjectSetInteger(0,objName,OBJPROP_FONTSIZE,8);
            ObjectSetInteger(0,objName,OBJPROP_ANCHOR,ANCHOR_LEFT_UPPER);
            ObjectSetInteger(0,objName,OBJPROP_BACK,false);
            ObjectSetInteger(0,objName,OBJPROP_ZORDER,40007);
         }
         ObjectSetInteger(0,objName,OBJPROP_XDISTANCE,lx);
         ObjectSetInteger(0,objName,OBJPROP_YDISTANCE,yPos);
         ObjectSetString(0,objName,OBJPROP_TEXT,dots_log_text[idx]);
         ObjectSetInteger(0,objName,OBJPROP_COLOR,dots_log_clr[idx]);
      } else {
         if(ObjectFind(0,objName)>=0) ObjectSetString(0,objName,OBJPROP_TEXT,"");
      }
   }
}
void DotsLabel(string name,int x,int y,string text,color clr,int fontSize,string font) {
   if(ObjectFind(0,name)<0) {
      ObjectCreate(0,name,OBJ_LABEL,0,0,0);
      ObjectSetInteger(0,name,OBJPROP_CORNER,CORNER_RIGHT_UPPER);
      ObjectSetString(0,name,OBJPROP_FONT,font);
      ObjectSetInteger(0,name,OBJPROP_FONTSIZE,fontSize);
      ObjectSetInteger(0,name,OBJPROP_ANCHOR,ANCHOR_LEFT_UPPER);
      ObjectSetInteger(0,name,OBJPROP_BACK,false);
      ObjectSetInteger(0,name,OBJPROP_ZORDER,40006);
   }
   ObjectSetInteger(0,name,OBJPROP_XDISTANCE,x);
   ObjectSetInteger(0,name,OBJPROP_YDISTANCE,y);
   ObjectSetString(0,name,OBJPROP_TEXT,text);
   ObjectSetInteger(0,name,OBJPROP_COLOR,clr);
}
void DotsLabelR(string name,int x,int y,string text,color clr,int fontSize,string font) {
   if(ObjectFind(0,name)<0) {
      ObjectCreate(0,name,OBJ_LABEL,0,0,0);
      ObjectSetInteger(0,name,OBJPROP_CORNER,CORNER_RIGHT_UPPER);
      ObjectSetString(0,name,OBJPROP_FONT,font);
      ObjectSetInteger(0,name,OBJPROP_FONTSIZE,fontSize);
      ObjectSetInteger(0,name,OBJPROP_ANCHOR,ANCHOR_RIGHT_UPPER);
      ObjectSetInteger(0,name,OBJPROP_BACK,false);
      ObjectSetInteger(0,name,OBJPROP_ZORDER,40006);
   }
   ObjectSetInteger(0,name,OBJPROP_XDISTANCE,x);
   ObjectSetInteger(0,name,OBJPROP_YDISTANCE,y);
   ObjectSetString(0,name,OBJPROP_TEXT,text);
   ObjectSetInteger(0,name,OBJPROP_COLOR,clr);
}
void DotsLabelC(string name,int x,int y,string text,color clr,int fontSize,string font) {
   if(ObjectFind(0,name)<0) {
      ObjectCreate(0,name,OBJ_LABEL,0,0,0);
      ObjectSetInteger(0,name,OBJPROP_CORNER,CORNER_RIGHT_UPPER);
      ObjectSetString(0,name,OBJPROP_FONT,font);
      ObjectSetInteger(0,name,OBJPROP_FONTSIZE,fontSize);
      ObjectSetInteger(0,name,OBJPROP_ANCHOR,ANCHOR_CENTER);
      ObjectSetInteger(0,name,OBJPROP_BACK,false);
      ObjectSetInteger(0,name,OBJPROP_ZORDER,40006);
   }
   ObjectSetInteger(0,name,OBJPROP_XDISTANCE,x);
   ObjectSetInteger(0,name,OBJPROP_YDISTANCE,y);
   ObjectSetString(0,name,OBJPROP_TEXT,text);
   ObjectSetInteger(0,name,OBJPROP_COLOR,clr);
}
void DotsRect(string name,int x,int y,int w,int h,color bg,color bd) {
   if(ObjectFind(0,name)<0) {
      ObjectCreate(0,name,OBJ_RECTANGLE_LABEL,0,0,0);
      ObjectSetInteger(0,name,OBJPROP_CORNER,CORNER_RIGHT_UPPER);
      ObjectSetInteger(0,name,OBJPROP_BORDER_TYPE,BORDER_FLAT);
      ObjectSetInteger(0,name,OBJPROP_BACK,false);
      ObjectSetInteger(0,name,OBJPROP_ZORDER,40005);
   }
   ObjectSetInteger(0,name,OBJPROP_XDISTANCE,x);
   ObjectSetInteger(0,name,OBJPROP_YDISTANCE,y);
   ObjectSetInteger(0,name,OBJPROP_XSIZE,w);
   ObjectSetInteger(0,name,OBJPROP_YSIZE,h);
   ObjectSetInteger(0,name,OBJPROP_BGCOLOR,bg);
   ObjectSetInteger(0,name,OBJPROP_BORDER_COLOR,bd);
}
void DotsRenderGridCol(string tag,int &grid[],int &hdr[],string &hnames[],int colX,int contentY,int contentH,int maxRows) {
   color cBD  =C'65,71,83';
   color cPri =C'146,134,124';
   color cMut =C'95,107,119';
   color cBtn =C'89,116,124';
   color cLong=C'30,144,255';
   color cShort=C'255,141,30';
   string P=ea_prefix+"Dots_";
   int sigIdx=0;
   int hdrIdx=0;
   for(int vr=0;vr<DOTS_GRID_ROWS&&vr<maxRows;vr++) {
      int yPos=contentY+vr*contentH/DOTS_GRID_ROWS;
      bool isHdr=false;
      if(hdrIdx<4&&vr==hdr[hdrIdx]) {
         isHdr=true;
         string hObj=P+tag+"H_"+IntegerToString(hdrIdx);
         DotsLabel(hObj,colX,yPos,hnames[hdrIdx],cPri,8,"Segoe UI Bold");
         hdrIdx++;
      }
      if(!isHdr&&sigIdx<DOTS_GRID_COLS) {
         int ri=grid[sigIdx];
         string dotObj=P+tag+"D_"+IntegerToString(sigIdx);
         string nmObj=P+tag+"N_"+IntegerToString(sigIdx);
         string drObj=P+tag+"R_"+IntegerToString(sigIdx);
         color dirAccent=(dots_rules[ri].direction==1)?cLong:cShort;
         color dotClr=cBD;
         if(dots_state[ri].ticket>0) dotClr=dirAccent;
         else if(dots_state[ri].condA&&dots_state[ri].condB&&dots_state[ri].condC) dotClr=cBtn;
         if(ObjectFind(0,dotObj)<0) {
            ObjectCreate(0,dotObj,OBJ_LABEL,0,0,0);
            ObjectSetInteger(0,dotObj,OBJPROP_CORNER,CORNER_RIGHT_UPPER);
            ObjectSetString(0,dotObj,OBJPROP_FONT,"Wingdings");
            ObjectSetInteger(0,dotObj,OBJPROP_FONTSIZE,6);
            ObjectSetInteger(0,dotObj,OBJPROP_ANCHOR,ANCHOR_LEFT_UPPER);
            ObjectSetInteger(0,dotObj,OBJPROP_BACK,false);
            ObjectSetInteger(0,dotObj,OBJPROP_ZORDER,40007);
         }
         ObjectSetInteger(0,dotObj,OBJPROP_XDISTANCE,colX);
         ObjectSetInteger(0,dotObj,OBJPROP_YDISTANCE,yPos+1);
         ObjectSetString(0,dotObj,OBJPROP_TEXT,CharToString(110));
         ObjectSetInteger(0,dotObj,OBJPROP_COLOR,dotClr);
         string dispName=dots_ruleName[ri];
         int uPos=StringFind(dispName,"_");
         if(uPos>=0) dispName=StringSubstr(dispName,uPos+1);
         DotsLabel(nmObj,colX-12,yPos,dispName,cPri,8,"Consolas");
         string dirCh=(dots_rules[ri].direction==1)?"L":"S";
         DotsLabelR(drObj,colX-120,yPos,dirCh,cMut,8,"Consolas");
         sigIdx++;
      }
   }
}
void DrawDotsPanel() {
   color cBG  =C'19,29,42';
   color cBD  =C'65,71,83';
   color cPri =C'146,134,124';
   color cSec =C'96,95,113';
   color cMut =C'95,107,119';
   color cBtn =C'89,116,124';
   int margin=15;
   int c1W=200;
   int c2W=260;
   int c3W=200;
   int gap=3;
   int hdrH=28;
   int pad=8;
   string P=ea_prefix+"Dots_";
   int chartH=(int)ChartGetInteger(0,CHART_HEIGHT_IN_PIXELS);
   int topY=45;
   int panelH=chartH-topY-15;
   if(panelH<200) panelH=200;
   int c3X=margin;
   int c2X=margin+c3W+gap;
   int c1X=margin+c3W+gap+c2W+gap;
   DotsRect(P+"C1_bg",c1X,topY,c1W,panelH,cBG,cBD);
   DotsRect(P+"C1_hdr",c1X,topY,c1W,hdrH,cBD,cBD);
   bool showPrice=GetHeaderToggleState();
   int c1cx=c1X+c1W/2;
   if(showPrice)
      DotsLabelC(P+"C1_title",c1cx,topY+(hdrH/2),DoubleToString(Bid,Digits),cPri,18,"Segoe UI Bold");
   else
      DotsLabelC(P+"C1_title",c1cx,topY+(hdrH/2),"DOTS",cPri,18,"Segoe UI Bold");
   DotsRenderLog();
   DotsRect(P+"C2_bg",c2X,topY,c2W,panelH,cBG,cBD);
   DotsRect(P+"C2_hdr",c2X,topY,c2W,hdrH,cBD,cBD);
   int c2cx=c2X+c2W/2;
   int d2dDir=(1<ArraySize(Direction))?Direction[1]:0;
   string d2dStr=(d2dDir==1)?"LONG":(d2dDir==-1)?"SHORT":"FLAT";
   color d2dClr=(d2dDir==1)?cPri:(d2dDir==-1)?cBtn:cSec;
   DotsLabelC(P+"C2_title",c2cx,topY+(hdrH/2),"76 SIGNALS",cPri,9,"Segoe UI Bold");
   int c2lx=c2X+c2W-pad;
   int c2rx=c2X+pad;
   DotsLabel(P+"C2_d2d",c2lx,topY+hdrH+3,d2dStr,d2dClr,8,"Consolas");
   double atrNow=(1<ArraySize(ATR_1M_Array))?ATR_1M_Array[1]:0.0;
   double adxNow=(1<ArraySize(hist_ADXValue))?hist_ADXValue[1]:0.0;
   double volNow=(1<ArraySize(hist_VolumeValue))?hist_VolumeValue[1]:0.0;
   DotsLabelR(P+"C2_stats",c2rx,topY+hdrH+3,
      DoubleToString(atrNow,1)+"  "+DoubleToString(adxNow,0)+"  "+DoubleToString(volNow,0)+"  "+IntegerToString(dots_active_count)+"/"+IntegerToString(Dots_MaxPositions),
      cMut,8,"Consolas");
   int gridContentY=topY+hdrH+18;
   int gridContentH=panelH-hdrH-18-pad;
   int maxRows=DOTS_GRID_ROWS;
   int leftColX=c2lx;
   int rightColX=c2lx-(c2W/2)+pad;
   DotsRenderGridCol("L",dots_grid_left,dots_grid_lhdr,dots_grid_lnames,leftColX,gridContentY,gridContentH,maxRows);
   DotsRenderGridCol("R",dots_grid_right,dots_grid_rhdr,dots_grid_rnames,rightColX,gridContentY,gridContentH,maxRows);
   DotsRect(P+"C3_bg",c3X,topY,c3W,panelH,cBG,cBD);
   DotsRect(P+"C3_hdr",c3X,topY,c3W,hdrH,cBD,cBD);
   int c3cx=c3X+c3W/2;
   int c3lx=c3X+c3W-pad;
   int c3rx=c3X+pad;
   double livePnl=0.0;
   for(int i=0;i<DOTS_NUM_RULES;i++) {
      if(dots_state[i].ticket<=0) continue;
      if(OrderSelect(dots_state[i].ticket,SELECT_BY_TICKET))
         livePnl+=OrderProfit()+OrderSwap()+OrderCommission();
   }
   color pnlClr=(livePnl>=0.0)?cPri:cSec;
   DotsLabelC(P+"C3_title",c3cx,topY+(hdrH/2),"LIVE",cPri,9,"Segoe UI Bold");
   string statusStr=isDotsTradeActive?"ACTIVE":"STANDBY";
   color statusClr=isDotsTradeActive?cPri:cSec;
   DotsLabel(P+"C3_status",c3lx,topY+hdrH+3,statusStr,statusClr,8,"Consolas");
   DotsLabelR(P+"C3_pnl",c3rx,topY+hdrH+3,"$"+DoubleToString(livePnl,2),pnlClr,8,"Consolas");
   int c3dataY=topY+hdrH+20;
   int c3lineH=16;
   DotsLabel(P+"C3_pos",c3lx,c3dataY,"Positions",cMut,8,"Consolas");
   DotsLabelR(P+"C3_posV",c3rx,c3dataY,IntegerToString(dots_active_count)+" / "+IntegerToString(Dots_MaxPositions),cPri,8,"Consolas");
   c3dataY+=c3lineH;
   DotsLabel(P+"C3_atr",c3lx,c3dataY,"ATR",cMut,8,"Consolas");
   DotsLabelR(P+"C3_atrV",c3rx,c3dataY,DoubleToString(atrNow,1),cPri,8,"Consolas");
   c3dataY+=c3lineH;
   DotsLabel(P+"C3_adx",c3lx,c3dataY,"ADX",cMut,8,"Consolas");
   DotsLabelR(P+"C3_adxV",c3rx,c3dataY,DoubleToString(adxNow,1),cPri,8,"Consolas");
   c3dataY+=c3lineH;
   DotsLabel(P+"C3_vol",c3lx,c3dataY,"Volume",cMut,8,"Consolas");
   DotsLabelR(P+"C3_volV",c3rx,c3dataY,DoubleToString(volNow,0),cPri,8,"Consolas");
   c3dataY+=c3lineH+4;
   DotsRect(P+"C3_div1",c3X+pad,c3dataY,c3W-2*pad,1,cBD,cBD);
   c3dataY+=6;
   DotsLabel(P+"C3_wins",c3lx,c3dataY,"Wins",cMut,8,"Consolas");
   DotsLabelR(P+"C3_winsV",c3rx,c3dataY,IntegerToString(dots_today_wins),cPri,8,"Consolas");
   c3dataY+=c3lineH;
   DotsLabel(P+"C3_loss",c3lx,c3dataY,"Losses",cMut,8,"Consolas");
   DotsLabelR(P+"C3_lossV",c3rx,c3dataY,IntegerToString(dots_today_losses),cSec,8,"Consolas");
   c3dataY+=c3lineH;
   DotsLabel(P+"C3_pnlD",c3lx,c3dataY,"Day P&L",cMut,8,"Consolas");
   color dayClr=(dots_today_pnl>=0.0)?cPri:cSec;
   DotsLabelR(P+"C3_pnlDV",c3rx,c3dataY,"$"+DoubleToString(dots_today_pnl,2),dayClr,8,"Consolas");
   c3dataY+=c3lineH;
   string pfStr=(dots_gross_loss>0.0)?DoubleToString(dots_gross_win/dots_gross_loss,2):"INF";
   color pfClr=(dots_gross_loss>0.0&&dots_gross_win/dots_gross_loss>=4.0)?cPri:cSec;
   if(dots_gross_loss==0.0&&dots_gross_win>0.0) pfClr=cPri;
   DotsLabel(P+"C3_pf",c3lx,c3dataY,"PF",cMut,8,"Consolas");
   DotsLabelR(P+"C3_pfV",c3rx,c3dataY,pfStr,pfClr,8,"Consolas");
   c3dataY+=c3lineH;
   DotsLabel(P+"C3_total",c3lx,c3dataY,"Total",cMut,8,"Consolas");
   DotsLabelR(P+"C3_totalV",c3rx,c3dataY,IntegerToString(dots_total_trades),cPri,8,"Consolas");
   c3dataY+=c3lineH+4;
   DotsRect(P+"C3_div2",c3X+pad,c3dataY,c3W-2*pad,1,cBD,cBD);
   c3dataY+=6;
   color cLong=C'30,144,255';
   color cShort=C'255,141,30';
   int cardH=34;
   int maxCards=6;
   int cardIdx=0;
   for(int t=0;t<DOTS_NUM_RULES&&cardIdx<maxCards;t++) {
      if(dots_state[t].ticket<=0) continue;
      string cObj=P+"C3_card"+IntegerToString(cardIdx);
      string cBdr=P+"C3_cbdr"+IntegerToString(cardIdx);
      string cNm=P+"C3_cnm"+IntegerToString(cardIdx);
      string cPnlObj=P+"C3_cpnl"+IntegerToString(cardIdx);
      string cR2L=P+"C3_cr2l"+IntegerToString(cardIdx);
      string cR2R=P+"C3_cr2r"+IntegerToString(cardIdx);
      int dir=dots_state[t].direction;
      color borderClr=(dir==1)?cLong:cShort;
      DotsRect(cObj,c3X+pad,c3dataY,c3W-2*pad,cardH-2,cBG,cBD);
      DotsRect(cBdr,c3X+c3W-pad,c3dataY,3,cardH-2,borderClr,borderClr);
      string dispName=dots_ruleName[t];
      int uPos=StringFind(dispName,"_");
      if(uPos>=0) dispName=StringSubstr(dispName,uPos+1);
      string dirCh=(dir==1)?"L":"S";
      DotsLabel(cNm,c3lx-4,c3dataY+2,dirCh+" "+dispName,cPri,8,"Consolas");
      double tPnl=0.0;
      if(OrderSelect(dots_state[t].ticket,SELECT_BY_TICKET))
         tPnl=OrderProfit()+OrderSwap()+OrderCommission();
      color tPnlClr=(tPnl>=0.0)?cPri:cSec;
      DotsLabelR(cPnlObj,c3rx,c3dataY+2,"$"+DoubleToString(tPnl,2),tPnlClr,8,"Consolas");
      string slStr=DoubleToString(dots_state[t].entryPrice,Digits)+ShortToString(8594)+DoubleToString(dots_state[t].currentSL,Digits);
      DotsLabel(cR2L,c3lx-4,c3dataY+14,slStr,cMut,8,"Consolas");
      string stStr="T"+IntegerToString(dots_state[t].tiersReached)+" BE:"+(dots_state[t].beNudged?"Y":"N")+" LF:"+(dots_state[t].tiersReached>=Dots_LF_Activation?"Y":"N");
      DotsLabelR(cR2R,c3rx,c3dataY+14,stStr,cMut,8,"Consolas");
      c3dataY+=cardH;
      cardIdx++;
   }
   for(int cl=cardIdx;cl<maxCards;cl++) {
      string cObj=P+"C3_card"+IntegerToString(cl);
      string cBdr=P+"C3_cbdr"+IntegerToString(cl);
      string cNm=P+"C3_cnm"+IntegerToString(cl);
      string cPnlObj=P+"C3_cpnl"+IntegerToString(cl);
      string cR2L=P+"C3_cr2l"+IntegerToString(cl);
      string cR2R=P+"C3_cr2r"+IntegerToString(cl);
      if(ObjectFind(0,cObj)>=0) ObjectSetInteger(0,cObj,OBJPROP_YSIZE,0);
      if(ObjectFind(0,cBdr)>=0) ObjectSetInteger(0,cBdr,OBJPROP_YSIZE,0);
      if(ObjectFind(0,cNm)>=0) ObjectSetString(0,cNm,OBJPROP_TEXT,"");
      if(ObjectFind(0,cPnlObj)>=0) ObjectSetString(0,cPnlObj,OBJPROP_TEXT,"");
      if(ObjectFind(0,cR2L)>=0) ObjectSetString(0,cR2L,OBJPROP_TEXT,"");
      if(ObjectFind(0,cR2R)>=0) ObjectSetString(0,cR2R,OBJPROP_TEXT,"");
   }
   c3dataY+=4;
   DotsRect(P+"C3_div3",c3X+pad,c3dataY,c3W-2*pad,1,cBD,cBD);
   c3dataY+=6;
   string btnBgN=ea_prefix+"btnDotsActivate_bg";
   string btnTxN=ea_prefix+"btnDotsActivate_text";
   int btnW=c3W-2*pad;
   if(ObjectFind(0,btnBgN)<0) {
      ObjectCreate(0,btnBgN,OBJ_RECTANGLE_LABEL,0,0,0);
      ObjectSetInteger(0,btnBgN,OBJPROP_CORNER,CORNER_RIGHT_UPPER);
      ObjectSetInteger(0,btnBgN,OBJPROP_BORDER_TYPE,BORDER_FLAT);
      ObjectSetInteger(0,btnBgN,OBJPROP_BACK,false);
      ObjectSetInteger(0,btnBgN,OBJPROP_ZORDER,40008);
   }
   ObjectSetInteger(0,btnBgN,OBJPROP_XDISTANCE,c3X+pad);
   ObjectSetInteger(0,btnBgN,OBJPROP_YDISTANCE,c3dataY);
   ObjectSetInteger(0,btnBgN,OBJPROP_XSIZE,btnW);
   ObjectSetInteger(0,btnBgN,OBJPROP_YSIZE,20);
   ObjectSetInteger(0,btnBgN,OBJPROP_BGCOLOR,cBD);
   ObjectSetInteger(0,btnBgN,OBJPROP_BORDER_COLOR,cMut);
   if(ObjectFind(0,btnTxN)<0) {
      ObjectCreate(0,btnTxN,OBJ_LABEL,0,0,0);
      ObjectSetInteger(0,btnTxN,OBJPROP_CORNER,CORNER_RIGHT_UPPER);
      ObjectSetString(0,btnTxN,OBJPROP_FONT,"Segoe UI Bold");
      ObjectSetInteger(0,btnTxN,OBJPROP_FONTSIZE,9);
      ObjectSetInteger(0,btnTxN,OBJPROP_ANCHOR,ANCHOR_CENTER);
      ObjectSetInteger(0,btnTxN,OBJPROP_ZORDER,40009);
   }
   ObjectSetInteger(0,btnTxN,OBJPROP_XDISTANCE,c3X+c3W/2);
   ObjectSetInteger(0,btnTxN,OBJPROP_YDISTANCE,c3dataY+10);
   ObjectSetString(0,btnTxN,OBJPROP_TEXT,isDotsTradeActive?"DOTS ACTIVE":"ACTIVATE DOTS");
   ObjectSetInteger(0,btnTxN,OBJPROP_COLOR,isDotsTradeActive?cPri:cBtn);
   c3dataY+=26;
   datetime estNow=GetEstTime();
   int estH=TimeHour(estNow);
   int estM=TimeMinute(estNow);
   int estDay=GetEstDayOfWeek(TimeGMT());
   int minsToClose=0;
   if(estDay>=1&&estDay<=4) minsToClose=(5-estDay)*24*60+(16-estH)*60+(45-estM);
   else if(estDay==5) minsToClose=(16-estH)*60+(45-estM);
   if(minsToClose<0) minsToClose=0;
   string closeStr=(estDay==5&&minsToClose==0)?"CLOSED":IntegerToString(minsToClose/60)+"h "+IntegerToString(minsToClose%60)+"m";
   color closeClr=(estDay==5&&minsToClose<60)?cSec:cMut;
   DotsLabel(P+"C3_fri",c3lx,c3dataY,"Fri Close",cMut,8,"Consolas");
   DotsLabelR(P+"C3_friV",c3rx,c3dataY,closeStr,closeClr,8,"Consolas");
   c3dataY+=c3lineH;
   DotsLabel(P+"C3_est",c3lx,c3dataY,"EST",cMut,8,"Consolas");
   DotsLabelR(P+"C3_estV",c3rx,c3dataY,StringFormat("%02d:%02d",estH,estM),cPri,8,"Consolas");
   c3dataY+=c3lineH+4;
   DotsRect(P+"C3_div4",c3X+pad,c3dataY,c3W-2*pad,1,cBD,cBD);
   c3dataY+=6;
   DotsLabel(P+"C3_rhdr",c3lx,c3dataY,"ROLLING THRESHOLDS",cSec,8,"Segoe UI Bold");
   c3dataY+=c3lineH;
   DotsLabel(P+"C3_rSLT",c3lx,c3dataY,"AT_Slope_LT",cMut,8,"Consolas");
   DotsLabelR(P+"C3_rSLTV",c3rx,c3dataY,DoubleToString(dots_threshold[FEAT_AT_Slope_LT][THR_HI],8),cPri,8,"Consolas");
   c3dataY+=c3lineH;
   DotsLabel(P+"C3_rSEL",c3lx,c3dataY,"Slope_EMA_LT",cMut,8,"Consolas");
   DotsLabelR(P+"C3_rSELV",c3rx,c3dataY,DoubleToString(dots_threshold[FEAT_Slope_EMA_LT][THR_HI],8),cPri,8,"Consolas");
   c3dataY+=c3lineH;
   DotsLabel(P+"C3_rOMH",c3lx,c3dataY,"OBV_Macd hi",cMut,8,"Consolas");
   DotsLabelR(P+"C3_rOMHV",c3rx,c3dataY,DoubleToString(dots_threshold[FEAT_OBV_Macd][THR_HI],2),cPri,8,"Consolas");
   c3dataY+=c3lineH;
   DotsLabel(P+"C3_rOML",c3lx,c3dataY,"OBV_Macd lo",cMut,8,"Consolas");
   DotsLabelR(P+"C3_rOMLV",c3rx,c3dataY,DoubleToString(dots_threshold[FEAT_OBV_Macd][THR_LO],2),cPri,8,"Consolas");
   c3dataY+=c3lineH;
   DotsLabel(P+"C3_rHL",c3lx,c3dataY,"Harmonic",cMut,8,"Consolas");
   DotsLabelR(P+"C3_rHLV",c3rx,c3dataY,DoubleToString(dots_threshold[FEAT_Harmonic_LLEMA][THR_HI],6),cPri,8,"Consolas");
   c3dataY+=c3lineH;
   DotsLabel(P+"C3_rRO",c3lx,c3dataY,"RangeOsc",cMut,8,"Consolas");
   DotsLabelR(P+"C3_rROV",c3rx,c3dataY,DoubleToString(dots_threshold[FEAT_RangeOsc_Val][THR_LO],2),cPri,8,"Consolas");
   c3dataY+=c3lineH;
   DotsLabel(P+"C3_rRef",c3lx,c3dataY,"Refresh",cMut,8,"Consolas");
   DotsLabelR(P+"C3_rRefV",c3rx,c3dataY,"Day "+IntegerToString(dots_roll_lastRefreshDay),cMut,8,"Consolas");
}
void RemoveDotsPanel() {
   string match=ea_prefix+"Dots_";
   for(int i=ObjectsTotal(0,-1,-1)-1;i>=0;i--) {
      string name=ObjectName(0,i,-1,-1);
      if(StringFind(name,match)==0) ObjectDelete(0,name);
   }
   ObjectDelete(0,ea_prefix+"btnDotsActivate_bg");
   ObjectDelete(0,ea_prefix+"btnDotsActivate_text");
}
//+------------------------------------------------------------------+
//| SECTION 8.0 - TRADE EXECUTION & MANAGEMENT                       |
//+------------------------------------------------------------------+
bool IsTradeRiskFree(int ticket) {
   if(OrderSelect(ticket, SELECT_BY_TICKET)) {
      if(OrderSymbol() != Symbol()) return false;
      double sl = OrderStopLoss();
      double open = OrderOpenPrice();
      int type = OrderType();
      RefreshRates();
      double currentProfit = (type == OP_BUY) ? (Bid - open) : (open - Ask);
      if(sl == 0.0) return false;
      if(currentProfit > 0) {
         if(type == OP_BUY && sl >= open - (Point / 2.0)) return true;
         if(type == OP_SELL && sl <= open + (Point / 2.0)) return true;
      }
   }
   return false;
}
double GetDynamicLotSize(int cmd) {
   double calculatedLot=BaseLotSize;
   double minLot=MarketInfo(Symbol(),MODE_MINLOT);
   double maxLot=MathMin(MarketInfo(Symbol(),MODE_MAXLOT),MaxLotSize);
   double lotStep=MarketInfo(Symbol(),MODE_LOTSTEP);
   if(!UseVolumeMatrixMultiplier) {
      if(lotStep>0) calculatedLot=MathRound(calculatedLot/lotStep)*lotStep;
      if(calculatedLot<minLot) calculatedLot=minLot;
      if(calculatedLot>maxLot) calculatedLot=maxLot;
      double marginCheck=AccountFreeMarginCheck(Symbol(),cmd,calculatedLot);
      if(marginCheck<=0.0 || GetLastError()==134) {
         double oneLotMargin=MarketInfo(Symbol(),MODE_MARGINREQUIRED);
         if(oneLotMargin>0.0) {
            double affordable=(AccountFreeMargin()*0.95)/oneLotMargin;
            if(lotStep>0) calculatedLot=MathFloor(affordable/lotStep)*lotStep;
            if(calculatedLot<minLot) return 0.0;
            if(calculatedLot>maxLot) calculatedLot=maxLot;
            ResetLastError();
         } else {
            return 0.0;
         }
      }
      return calculatedLot;
   }
   int peakBarIndex=-1;
   double peakVol=GetRollingMaxVolume(3,1,peakBarIndex);
   double currentMultiplier=0.0;
   if(Tier12_Vol>0.0 && Tier12_Mult>0.0 && peakVol>=Tier12_Vol) currentMultiplier=Tier12_Mult;
   else if(Tier11_Vol>0.0 && Tier11_Mult>0.0 && peakVol>=Tier11_Vol) currentMultiplier=Tier11_Mult;
   else if(Tier10_Vol>0.0 && Tier10_Mult>0.0 && peakVol>=Tier10_Vol) currentMultiplier=Tier10_Mult;
   else if(Tier9_Vol>0.0 && Tier9_Mult>0.0 && peakVol>=Tier9_Vol) currentMultiplier=Tier9_Mult;
   else if(Tier8_Vol>0.0 && Tier8_Mult>0.0 && peakVol>=Tier8_Vol) currentMultiplier=Tier8_Mult;
   else if(Tier7_Vol>0.0 && Tier7_Mult>0.0 && peakVol>=Tier7_Vol) currentMultiplier=Tier7_Mult;
   else if(Tier6_Vol>0.0 && Tier6_Mult>0.0 && peakVol>=Tier6_Vol) currentMultiplier=Tier6_Mult;
   else if(Tier5_Vol>0.0 && Tier5_Mult>0.0 && peakVol>=Tier5_Vol) currentMultiplier=Tier5_Mult;
   else if(Tier4_Vol>0.0 && Tier4_Mult>0.0 && peakVol>=Tier4_Vol) currentMultiplier=Tier4_Mult;
   else if(Tier3_Vol>0.0 && Tier3_Mult>0.0 && peakVol>=Tier3_Vol) currentMultiplier=Tier3_Mult;
   else if(Tier2_Vol>0.0 && Tier2_Mult>0.0 && peakVol>=Tier2_Vol) currentMultiplier=Tier2_Mult;
   else if(Tier1_Vol>0.0 && Tier1_Mult>0.0 && peakVol>=Tier1_Vol) currentMultiplier=Tier1_Mult;
   if(currentMultiplier==0.0) return 0.0;
   calculatedLot=BaseLotSize*currentMultiplier;
   if(lotStep>0) calculatedLot=MathRound(calculatedLot/lotStep)*lotStep;
   if(calculatedLot<minLot) calculatedLot=minLot;
   if(calculatedLot>maxLot) calculatedLot=maxLot;
   double marginCheck=AccountFreeMarginCheck(Symbol(),cmd,calculatedLot);
   if(marginCheck<=0.0 || GetLastError()==134) {
      double oneLotMargin=MarketInfo(Symbol(),MODE_MARGINREQUIRED);
      if(oneLotMargin>0.0) {
         double affordable=(AccountFreeMargin()*0.95)/oneLotMargin;
         if(lotStep>0) calculatedLot=MathFloor(affordable/lotStep)*lotStep;
         if(calculatedLot<minLot) return 0.0;
         if(calculatedLot>maxLot) calculatedLot=maxLot;
         ResetLastError();
      } else {
         return 0.0;
      }
   }
   return calculatedLot;
}
void HandleLoomsTradeSignal() {
   static int d2d_exec_state=0;
   if(d2d_exec_state==0 && lastCommittedSignal!=0) d2d_exec_state=lastCommittedSignal;
   if(UseOpenHours) {
      datetime estTime=GetEstTime();
      int currentHourEST=TimeHour(estTime);
      int currentMinuteEST=TimeMinute(estTime);
      int openTimeTotalMinutes=OpenHourEST*60+OpenMinuteEST;
      int closeTimeTotalMinutes=CloseHourEST*60+CloseMinuteEST;
      int currentTimeTotalMinutes=currentHourEST*60+currentMinuteEST;
      bool inSession;
      if(openTimeTotalMinutes>closeTimeTotalMinutes) inSession=(currentTimeTotalMinutes>=openTimeTotalMinutes||currentTimeTotalMinutes<closeTimeTotalMinutes);
      else inSession=(currentTimeTotalMinutes>=openTimeTotalMinutes&&currentTimeTotalMinutes<closeTimeTotalMinutes);
      if(!inSession) return;
   }
   if(UseBlockTime) {
      datetime estTime=GetEstTime();
      int currentHourEST=TimeHour(estTime);
      int currentMinuteEST=TimeMinute(estTime);
      int blockStartTimeTotalMinutes=BlockStartHourEST*60+BlockStartMinuteEST;
      int blockEndTimeTotalMinutes=BlockEndHourEST*60+BlockEndMinuteEST;
      int currentTimeTotalMinutes=currentHourEST*60+currentMinuteEST;
      if(currentTimeTotalMinutes>=blockStartTimeTotalMinutes&&currentTimeTotalMinutes<blockEndTimeTotalMinutes) return;
   }
   int estDay=GetEstDayOfWeek(TimeGMT());
   bool isTradingDayAllowed=false;
   switch(estDay) {
      case 0: if(TradeOnSunday) isTradingDayAllowed=true; break;
      case 1: if(TradeOnMonday) isTradingDayAllowed=true; break;
      case 2: if(TradeOnTuesday) isTradingDayAllowed=true; break;
      case 3: if(TradeOnWednesday) isTradingDayAllowed=true; break;
      case 4: if(TradeOnThursday) isTradingDayAllowed=true; break;
      case 5: if(TradeOnFriday) isTradingDayAllowed=true; break;
      case 6: isTradingDayAllowed=false; break;
   }
   if(!isTradingDayAllowed) return;
   if(Day()!=currentDay) {
      currentDailyLoss=0.0;
      currentDay=Day();
   }
   if(combinedCurrentDailyLoss>=MaxDailyLoss&&MaxDailyLoss>0) return;
   int signal=LockBuffer[1];
   if(signal==0) return;
   if(signal==d2d_exec_state) return;
   if(Time[1]<=d2d_lastTradedSignalTime) return;
   if(UseAdaptiveTrendFilter) {
      if(signal==1 && detectedSlope_ST<=0.0) return;
      if(signal==-1 && detectedSlope_ST>=0.0) return;
   }
   int cmd=(signal==1)?OP_BUY:OP_SELL;
   double finalLotSize=GetDynamicLotSize(cmd);
   if(finalLotSize<=0.0) return;
   if(MaxSpreadUSD>0) {
      double spreadCost=GetCurrentMarketSpreadUSD(finalLotSize);
      if(spreadCost>=MaxSpreadUSD) return;
   }
   if(isLoomsActive) {
      if(activationTime>0&&Time[1]<activationTime) return;
      for(int i=OrdersTotal()-1; i>=0; i--) {
         if(OrderSelect(i,SELECT_BY_POS,MODE_TRADES)&&OrderMagicNumber()==MagicNumber&&OrderSymbol()==Symbol()) {
            if(OrderType()==OP_BUYLIMIT||OrderType()==OP_SELLLIMIT) {
               if(!OrderDelete(OrderTicket())) Print("Error deleting pending order");
            }
         }
      }
      bool hasBuyOrder=HasOpenOrder(OP_BUY);
      bool hasSellOrder=HasOpenOrder(OP_SELL);
      string orderComment="Looms_"+Symbol()+"_ID"+IntegerToString(EA_ID)+"_M"+IntegerToString(MagicNumber);
      double sl=0,tp=0;
      if(D2D_UseDynamicSL) {
         double superTrendLevel=SuperTrend[1];
         if(superTrendLevel>0) {
            double openP=(signal==1)?Ask:Bid;
            double base_dist=MathAbs(openP-superTrendLevel);
            double buffer_dist=base_dist*MathAbs(D2D_SL_Buffer_Mult);
            double contract_dist=base_dist*MathAbs(D2D_SL_Contraction_Mult);
            if(signal==1) sl=NormalizeDouble(superTrendLevel-buffer_dist+contract_dist,Digits);
            else sl=NormalizeDouble(superTrendLevel+buffer_dist-contract_dist,Digits);
         }
         if(sl!=0) sl=ValidateStopLoss(signal==1?OP_BUY:OP_SELL,sl);
      }
      if(signal==1) {
         if(hasSellOrder) {
            CloseOrders(OP_SELL);
            ResetPartialTP();
         }
         if(!hasBuyOrder) {
            if(OrderExecutionType=="Market") {
               int ticket=OrderSend(Symbol(),OP_BUY,finalLotSize,Ask,Slippage,sl,tp,orderComment,MagicNumber,0,C'146,134,124');
               if(ticket>0) {
                  d2d_lastTradedSignalTime=Time[1];
                  d2d_exec_state=1;
                  PlaySound("ok.wav");
               }
            }
         }
      } else if(signal==-1) {
         if(hasBuyOrder) {
            CloseOrders(OP_BUY);
            ResetPartialTP();
         }
         if(!hasSellOrder) {
            if(OrderExecutionType=="Market") {
               int ticket=OrderSend(Symbol(),OP_SELL,finalLotSize,Bid,Slippage,sl,tp,orderComment,MagicNumber,0,C'89,116,124');
               if(ticket>0) {
                  d2d_lastTradedSignalTime=Time[1];
                  d2d_exec_state=-1;
                  PlaySound("ok.wav");
               }
            }
         }
      }
   }
}
void HandleOBVfriendTradeSignal() {
   if(!UseOBVfriend) return;
   if(combinedCurrentDailyLoss>=MaxDailyLoss&&MaxDailyLoss>0) return;
   if(UseOpenHours) {
      datetime estTime=GetEstTime();
      int currentHourEST=TimeHour(estTime);
      int currentMinuteEST=TimeMinute(estTime);
      int openTimeTotalMinutes=OpenHourEST*60+OpenMinuteEST;
      int closeTimeTotalMinutes=CloseHourEST*60+CloseMinuteEST;
      int currentTimeTotalMinutes=currentHourEST*60+currentMinuteEST;
      bool inSession;
      if(openTimeTotalMinutes>closeTimeTotalMinutes) inSession=(currentTimeTotalMinutes>=openTimeTotalMinutes||currentTimeTotalMinutes<closeTimeTotalMinutes);
      else inSession=(currentTimeTotalMinutes>=openTimeTotalMinutes&&currentTimeTotalMinutes<closeTimeTotalMinutes);
      if(!inSession) return;
   }
   if(UseBlockTime) {
      datetime estTime=GetEstTime();
      int currentHourEST=TimeHour(estTime);
      int currentMinuteEST=TimeMinute(estTime);
      int blockStartTimeTotalMinutes=BlockStartHourEST*60+BlockStartMinuteEST;
      int blockEndTimeTotalMinutes=BlockEndHourEST*60+BlockEndMinuteEST;
      int currentTimeTotalMinutes=currentHourEST*60+currentMinuteEST;
      if(currentTimeTotalMinutes>=blockStartTimeTotalMinutes&&currentTimeTotalMinutes<blockEndTimeTotalMinutes) return;
   }
   int estDay=GetEstDayOfWeek(TimeGMT());
   bool isTradingDayAllowed=false;
   switch(estDay) {
      case 0: if(TradeOnSunday) isTradingDayAllowed=true; break;
      case 1: if(TradeOnMonday) isTradingDayAllowed=true; break;
      case 2: if(TradeOnTuesday) isTradingDayAllowed=true; break;
      case 3: if(TradeOnWednesday) isTradingDayAllowed=true; break;
      case 4: if(TradeOnThursday) isTradingDayAllowed=true; break;
      case 5: if(TradeOnFriday) isTradingDayAllowed=true; break;
      case 6: isTradingDayAllowed=false; break;
   }
   if(!isTradingDayAllowed) return;
   double currentOBVDir = state_TChan_OC[1];
   double prevOBVDir = state_TChan_OC[2];
   if(currentOBVDir==0) return;
   if(currentOBVDir==prevOBVDir) return; 
   if(UseAdaptiveTrendFilter) {
      if((int)currentOBVDir==1 && detectedSlope_ST<=0.0) return;
      if((int)currentOBVDir==-1 && detectedSlope_ST>=0.0) return;
   }
   int cmd=(currentOBVDir==1)?OP_BUY:OP_SELL;
   double finalLotSize=GetDynamicLotSize(cmd);
   if(finalLotSize<=0.0) return;
   if(MaxSpreadUSD>0) {
      double spreadCost=GetCurrentMarketSpreadUSD(finalLotSize);
      if(spreadCost>=MaxSpreadUSD) return;
   }
   int existingTradeType=FindOpenOBVfriendTrade();
   string obvOrderComment="Looms_OBVf_"+Symbol()+"_ID"+IntegerToString(EA_ID)+"_M"+IntegerToString(OBVfriendMagicNumber);
   double sl=0,tp=0;
   if(currentOBVDir==1) {
      if(existingTradeType==OP_SELL) {
         CloseAllOBVfriendOrders();
         ResetOBVfriendPartialTP();
      }
      if(existingTradeType!=OP_BUY) {
         if(OBV_UseDynamicSL) {
            double superTrendLevel = OBVfriend_SuperTrend[1];
            if(superTrendLevel>0) {
               double openP=Ask;
               double base_dist=MathAbs(openP-superTrendLevel);
               double buffer_dist=base_dist*MathAbs(OBV_SL_Buffer_Mult);
               double contract_dist=base_dist*MathAbs(OBV_SL_Contraction_Mult);
               sl=NormalizeDouble(superTrendLevel-buffer_dist+contract_dist,Digits);
            }
            if(sl!=0) sl=ValidateStopLoss(OP_BUY,sl);
         }
         int ticket=OrderSend(Symbol(),OP_BUY,finalLotSize,Ask,Slippage,sl,tp,obvOrderComment,OBVfriendMagicNumber,0,C'146,134,124');
         if(ticket>0) obvfriend_lastTradedDirection=1;
      } else {
         obvfriend_lastTradedDirection=1;
      }
   } else if(currentOBVDir==-1) {
      if(existingTradeType==OP_BUY) {
         CloseAllOBVfriendOrders();
         ResetOBVfriendPartialTP();
      }
      if(existingTradeType!=OP_SELL) {
         if(OBV_UseDynamicSL) {
            double superTrendLevel = OBVfriend_SuperTrend[1];
            if(superTrendLevel>0) {
               double openP=Bid;
               double base_dist=MathAbs(openP-superTrendLevel);
               double buffer_dist=base_dist*MathAbs(OBV_SL_Buffer_Mult);
               double contract_dist=base_dist*MathAbs(OBV_SL_Contraction_Mult);
               sl=NormalizeDouble(superTrendLevel+buffer_dist-contract_dist,Digits);
            }
            if(sl!=0) sl=ValidateStopLoss(OP_SELL,sl);
         }
         int ticket=OrderSend(Symbol(),OP_SELL,finalLotSize,Bid,Slippage,sl,tp,obvOrderComment,OBVfriendMagicNumber,0,C'89,116,124');
         if(ticket>0) obvfriend_lastTradedDirection=-1;
      } else {
         obvfriend_lastTradedDirection=-1;
      }
   }
}
void ForceTradeExecution() {
   if(!isLoomsActive) return;
   int signal=lastCommittedSignal;
   if(signal==0) return;
   if(signal==1) { CloseOrders(OP_SELL); ResetPartialTP(); ResetLeapFrog(); }
   if(signal==-1) { CloseOrders(OP_BUY); ResetPartialTP(); ResetLeapFrog(); }
   double sl=0;
   int lastSigIdx=1;
   for(int k=1; k<Bars; k++) {
      if(LockBuffer[k]!=0) { lastSigIdx=k; break; }
   }
   double histDist=0;
   if(SuperTrend[lastSigIdx]>0 && SuperTrend[lastSigIdx]!=EMPTY_VALUE) {
      histDist=MathAbs(Close[lastSigIdx]-SuperTrend[lastSigIdx]);
   }
   if(histDist==0) histDist=50*Point;
   if(SuperTrend[1]>0 && SuperTrend[1]!=EMPTY_VALUE) {
      double currentDist = MathAbs((signal==1?Ask:Bid) - SuperTrend[1]);
      histDist = MathMax(histDist, currentDist);
   }
   if(signal==1&&!HasOpenOrder(OP_BUY)) {
      sl=NormalizeDouble(Ask-histDist,Digits);
      sl=ValidateStopLoss(OP_BUY,sl);
      int ticket=OrderSend(Symbol(),OP_BUY,BaseLotSize,Ask,Slippage,sl,0,"ForceBuy",MagicNumber,0,C'146,134,124');
      if(ticket>0) d2d_lastTradedSignalTime=Time[1];
   }
   if(signal==-1&&!HasOpenOrder(OP_SELL)) {
      sl=NormalizeDouble(Bid+histDist,Digits);
      sl=ValidateStopLoss(OP_SELL,sl);
      int ticket=OrderSend(Symbol(),OP_SELL,BaseLotSize,Bid,Slippage,sl,0,"ForceSell",MagicNumber,0,C'89,116,124');
      if(ticket>0) d2d_lastTradedSignalTime=Time[1];
   }
}
void ForceOBVfriendTradeExecution() {
   if(!UseOBVfriend) return;
   int dir=(int)(state_TChan_OC[1]);
   if(dir==0) return;
   if(dir==1) { CloseAllOBVfriendOrders(); ResetOBVfriendPartialTP(); }
   if(dir==-1) { CloseAllOBVfriendOrders(); ResetOBVfriendPartialTP(); }
   int existing=FindOpenOBVfriendTrade();
   double sl=0;
   int lastSigIdx = 1;
   for(int k = lastSigIdx; k < Bars - 1; k++) {
      if(state_TChan_OC[k]!=0 && state_TChan_OC[k]!=state_TChan_OC[k+1]) { lastSigIdx=k; break; }
   }
   double histDist=0;
   if(OBVfriend_SuperTrend[lastSigIdx]>0 && OBVfriend_SuperTrend[lastSigIdx]!=EMPTY_VALUE) {
      histDist=MathAbs(Close[lastSigIdx]-OBVfriend_SuperTrend[lastSigIdx]);
   }
   if(histDist==0) histDist=50*Point;
   double currentST = OBVfriend_SuperTrend[1];
   if(currentST>0 && currentST!=EMPTY_VALUE) {
      double currentDist = MathAbs((dir==1?Ask:Bid) - currentST);
      histDist = MathMax(histDist, currentDist);
   }
   if(dir==1&&existing!=OP_BUY) {
      sl=NormalizeDouble(Ask-histDist,Digits);
      sl=ValidateStopLoss(OP_BUY,sl);
      int ticket=OrderSend(Symbol(),OP_BUY,BaseLotSize,Ask,Slippage,sl,0,"ForceOBVf",OBVfriendMagicNumber,0,C'146,134,124');
      if(ticket>0) obvfriend_lastTradedSignalTime = Time[1];
   }
   if(dir==-1&&existing!=OP_SELL) {
      sl=NormalizeDouble(Bid+histDist,Digits);
      sl=ValidateStopLoss(OP_SELL,sl);
      int ticket=OrderSend(Symbol(),OP_SELL,BaseLotSize,Bid,Slippage,sl,0,"ForceOBVf",OBVfriendMagicNumber,0,C'89,116,124');
      if(ticket>0) obvfriend_lastTradedSignalTime = Time[1];
   }
}
void ManageForcedSessionActions() {
   if(!UseOpenHours||!ForceOpenHoursTrade) return;
   static bool openForcedToday=false;
   static bool closeForcedToday=false;
   datetime estTime=GetEstTime();
   int currentHourEST=TimeHour(estTime);
   int currentMinuteEST=TimeMinute(estTime);
   if(currentHourEST==CloseHourEST&&currentMinuteEST==CloseMinuteEST) {
      if(!closeForcedToday) {
         bool actionNeeded = false;
         for(int i=OrdersTotal()-1; i>=0; i--) {
            if(OrderSelect(i,SELECT_BY_POS,MODE_TRADES)&&OrderSymbol()==Symbol()) {
               int currentMagic=OrderMagicNumber();
               if(currentMagic==MagicNumber || currentMagic==OBVfriendMagicNumber) {
                  if(!IsTradeRiskFree(OrderTicket())) {
                     actionNeeded = true;
                     if(!OrderClose(OrderTicket(),OrderLots(),(OrderType()==OP_BUY?Bid:Ask),Slippage,clrRed)) {
                        Print("Close Error");
                     } else {
                        if(currentMagic==MagicNumber) {
                           ResetPartialTP();
                           ResetLeapFrog();
                        } else if(currentMagic==OBVfriendMagicNumber) {
                           ResetOBVfriendPartialTP();
                           ResetOBVfriendLeapFrog();
                        }
                     }
                  }
               }
            }
         }
         if(actionNeeded) Print("FORCED SESSION ACTION: Session Close Time Reached. Closing unprotected trades.");
         closeForcedToday=true;
      }
   } else {
      closeForcedToday=false;
   }
   if(currentHourEST==OpenHourEST&&currentMinuteEST==OpenMinuteEST) {
      if(!openForcedToday) {
         Print("FORCED SESSION ACTION: Open Time Reached.");
         if(isLoomsActive) ForceTradeExecution();
         if(UseOBVfriend) ForceOBVfriendTradeExecution();
         openForcedToday=true;
      }
   } else {
      openForcedToday=false;
   }
}
//+------------------------------------------------------------------+
//| SECTION 8.1 - ADVANCED TRADE MANAGEMENT                          |
//+------------------------------------------------------------------+
void ManageCommissionNudge() {
   if(!Move_To_BE) return;
   int ticket=OrderTicket();
   if(ticket==nudged_TradeTicket) return;
   double initialSL=OrderStopLoss();
   double openPrice=OrderOpenPrice();
   int orderType=OrderType();
   if(initialSL==0) return;
   double firstStepDist=0.0;
   if(D2D_UsePartialTP&&ptp_ProfitStep>0) firstStepDist=ptp_ProfitStep;
   else if(D2D_Leap_Frog_SL&&leap_ProfitStep>0) firstStepDist=leap_ProfitStep;
   else firstStepDist=100*Point;
   double triggerThreshold=firstStepDist*Trigger_At_Risk_Ratio;
   double currentProfit=(orderType==OP_BUY)?(Bid-openPrice):(openPrice-Ask);
   if(currentProfit<triggerThreshold) return;
   double risk=MathAbs(openPrice-initialSL);
   double allowance=risk*((double)Commission_Allowance_Percent/10000.0);
   double newSL=(orderType==OP_BUY)?(openPrice+allowance):(openPrice-allowance);
   newSL=ValidateStopLoss(orderType,newSL);
   bool modify=false;
   if(orderType==OP_BUY&&newSL>initialSL&&newSL<Bid) modify=true;
   if(orderType==OP_SELL&&newSL<initialSL&&newSL>Ask) modify=true;
   if(modify) {
      ResetLastError();
      if(OrderModify(ticket,openPrice,newSL,OrderTakeProfit(),0,clrGreen)) {
         nudged_TradeTicket=ticket;
         Print("Nudged trade #",ticket," to BE + Commission Allowance");
      } else {
         Print("Error nudging trade #",ticket,": ",GetLastError());
      }
   }
}
void ManageOBVfriendCommissionNudge() {
   if(!Move_To_BE) return;
   int ticket=OrderTicket();
   if(ticket==obvfriend_nudged_TradeTicket) return;
   double initialSL=OrderStopLoss();
   double openPrice=OrderOpenPrice();
   int orderType=OrderType();
   if(initialSL==0) return;
   double firstStepDist=0.0;
   if(OBV_UsePartialTP&&obvfriend_ptp_ProfitStep>0) firstStepDist=obvfriend_ptp_ProfitStep;
   else if(OBV_Leap_Frog_SL&&obvfriend_leap_ProfitStep>0) firstStepDist=obvfriend_leap_ProfitStep;
   else firstStepDist=100*Point;
   double triggerThreshold=firstStepDist*Trigger_At_Risk_Ratio;
   double currentProfit=(orderType==OP_BUY)?(Bid-openPrice):(openPrice-Ask);
   if(currentProfit<triggerThreshold) return;
   double risk=MathAbs(openPrice-initialSL);
   double allowance=risk*((double)Commission_Allowance_Percent/10000.0);
   double newSL=(orderType==OP_BUY)?(openPrice+allowance):(openPrice-allowance);
   newSL=ValidateStopLoss(orderType,newSL);
   bool modify=false;
   if(orderType==OP_BUY&&newSL>initialSL&&newSL<Bid) modify=true;
   if(orderType==OP_SELL&&newSL<initialSL&&newSL>Ask) modify=true;
   if(modify) {
      ResetLastError();
      if(OrderModify(ticket,openPrice,newSL,OrderTakeProfit(),0,clrGreen)) {
         obvfriend_nudged_TradeTicket=ticket;
         Print("Nudged OBVfriend trade #",ticket," to BE + Commission Allowance");
      } else {
         Print("Error nudging OBVfriend trade #",ticket,": ",GetLastError());
      }
   }
}
void ManageTrailWithSuperTrend() {
   for(int i=OrdersTotal()-1; i>=0; i--) {
      if(OrderSelect(i,SELECT_BY_POS,MODE_TRADES)&&OrderSymbol()==Symbol()&&OrderMagicNumber()==MagicNumber) {
         double superTrendLevel=SuperTrend[1];
         double finalSL=0;
         double tierSL=0;
         double leapSL=0;
         int oType=OrderType();
         double openP=OrderOpenPrice();
         double currentSL=OrderStopLoss();
         if(D2D_SL_Follows_Tiers && ptp_TradeTicket==OrderTicket() && ptp_TiersTaken >= (D2D_Leap_Frog_Lag + 1)) {
            int nudgeTier=ptp_TiersTaken-D2D_Leap_Frog_Lag;
            tierSL=(oType==OP_BUY)?(openP+(nudgeTier*ptp_ProfitStep)):(openP-(nudgeTier*ptp_ProfitStep));
         }
         if(D2D_Leap_Frog_SL && leap_TradeTicket==OrderTicket() && leap_TiersReached >= (D2D_Leap_Frog_Lag + 1)) {
            int leapNudge=leap_TiersReached-D2D_Leap_Frog_Lag;
            leapSL=(oType==OP_BUY)?(openP+(leapNudge*leap_ProfitStep)):(openP-(leapNudge*leap_ProfitStep));
         }
         if(superTrendLevel>0 && superTrendLevel!=EMPTY_VALUE) {
            string comment=OrderComment();
            bool isForced=(StringFind(comment,"Force")==0 || StringFind(comment,"Manual")==0);
            if(isForced) {
               int lastSigIdx=1;
               for(int k=1; k<Bars; k++) {
                  if(LockBuffer[k]!=0) { lastSigIdx=k; break; }
               }
               double histDist=0;
               if(SuperTrend[lastSigIdx]>0 && SuperTrend[lastSigIdx]!=EMPTY_VALUE) {
                  histDist=MathAbs(Close[lastSigIdx]-SuperTrend[lastSigIdx]);
               }
               if(histDist==0) histDist=50*Point;
               finalSL=(oType==OP_BUY)?NormalizeDouble(openP-histDist,Digits):NormalizeDouble(openP+histDist,Digits);
            } else {
               double base_dist=MathAbs(openP-superTrendLevel);
               double buffer_dist=base_dist*MathAbs(D2D_SL_Buffer_Mult);
               double contract_dist=base_dist*MathAbs(D2D_SL_Contraction_Mult);
               finalSL=(oType==OP_BUY)?NormalizeDouble(superTrendLevel-buffer_dist+contract_dist,Digits):NormalizeDouble(superTrendLevel+buffer_dist-contract_dist,Digits);
            }
         }
         if(oType==OP_BUY) {
            if(tierSL>0) finalSL=MathMax(finalSL,tierSL);
            if(leapSL>0) finalSL=MathMax(finalSL,leapSL);
         } else {
            if(tierSL>0) finalSL=(finalSL==0)?tierSL:MathMin(finalSL,tierSL);
            if(leapSL>0) finalSL=(finalSL==0)?leapSL:MathMin(finalSL,leapSL);
         }
         if(finalSL>0) {
            finalSL=ValidateStopLoss(oType,finalSL);
            double minDiff=3.0*Point;
            bool shouldModify=false;
            if(oType==OP_BUY && finalSL<Bid && (currentSL==0 || (finalSL-currentSL)>=minDiff)) shouldModify=true;
            if(oType==OP_SELL && finalSL>Ask && (currentSL==0 || (currentSL-finalSL)>=minDiff)) shouldModify=true;
            if(shouldModify) {
               ResetLastError();
               if(!OrderModify(OrderTicket(),openP,finalSL,OrderTakeProfit(),0,(oType==OP_BUY?clrGreen:clrRed))) Print("Modify Error: ",GetLastError());
            }
         }
      }
   }
}
void ManageOBVfriendTrailWithSuperTrend() {
   for(int i=OrdersTotal()-1; i>=0; i--) {
      if(OrderSelect(i,SELECT_BY_POS,MODE_TRADES)&&OrderSymbol()==Symbol()&&OrderMagicNumber()==OBVfriendMagicNumber) {
         double superTrendLevel=OBVfriend_SuperTrend[1];
         double finalSL=0;
         double tierSL=0;
         double leapSL=0;
         int oType=OrderType();
         double openP=OrderOpenPrice();
         double currentSL=OrderStopLoss();
         if(OBV_SL_Follows_Tiers && obvfriend_ptp_TradeTicket==OrderTicket() && obvfriend_ptp_TiersTaken >= (OBV_PTP_Lag + 1)) {
            int nudgeTier=obvfriend_ptp_TiersTaken-OBV_PTP_Lag;
            tierSL=(oType==OP_BUY)?(openP+(nudgeTier*obvfriend_ptp_ProfitStep)):(openP-(nudgeTier*obvfriend_ptp_ProfitStep));
         }
         if(OBV_Leap_Frog_SL && obvfriend_leap_TradeTicket==OrderTicket() && obvfriend_leap_TiersReached >= (OBV_Leap_Frog_Lag + 1)) {
            int leapNudge=obvfriend_leap_TiersReached-OBV_Leap_Frog_Lag;
            leapSL=(oType==OP_BUY)?(openP+(leapNudge*obvfriend_leap_ProfitStep)):(openP-(leapNudge*obvfriend_leap_ProfitStep));
         }
         if(superTrendLevel>0 && superTrendLevel!=EMPTY_VALUE) {
            string comment=OrderComment();
            bool isForced=(StringFind(comment,"Force")==0 || StringFind(comment,"Manual")==0);
            if(isForced) {
               int lastSigIdx=1;
               for(int k=1; k<Bars; k++) {
                  if(state_TChan_OC[k]!=0 && state_TChan_OC[k]!=state_TChan_OC[k+1]) { lastSigIdx=k; break; }
               }
               double histDist=0;
               if(OBVfriend_SuperTrend[lastSigIdx]>0 && OBVfriend_SuperTrend[lastSigIdx]!=EMPTY_VALUE) {
                  histDist=MathAbs(Close[lastSigIdx]-OBVfriend_SuperTrend[lastSigIdx]);
               }
               if(histDist==0) histDist=50*Point;
               finalSL=(oType==OP_BUY)?NormalizeDouble(openP-histDist,Digits):NormalizeDouble(openP+histDist,Digits);
            } else {
               double base_dist=MathAbs(openP-superTrendLevel);
               double buffer_dist=base_dist*MathAbs(OBV_SL_Buffer_Mult);
               double contract_dist=base_dist*MathAbs(OBV_SL_Contraction_Mult);
               finalSL=(oType==OP_BUY)?NormalizeDouble(superTrendLevel-buffer_dist+contract_dist,Digits):NormalizeDouble(superTrendLevel+buffer_dist-contract_dist,Digits);
            }
         }
         if(oType==OP_BUY) {
            if(tierSL>0) finalSL=MathMax(finalSL,tierSL);
            if(leapSL>0) finalSL=MathMax(finalSL,leapSL);
         } else {
            if(tierSL>0) finalSL=(finalSL==0)?tierSL:MathMin(finalSL,tierSL);
            if(leapSL>0) finalSL=(finalSL==0)?leapSL:MathMin(finalSL,leapSL);
         }
         if(finalSL>0) {
            finalSL=ValidateStopLoss(oType,finalSL);
            double minDiff=3.0*Point;
            bool shouldModify=false;
            if(oType==OP_BUY && finalSL<Bid && (currentSL==0 || (finalSL-currentSL)>=minDiff)) shouldModify=true;
            if(oType==OP_SELL && finalSL>Ask && (currentSL==0 || (currentSL-finalSL)>=minDiff)) shouldModify=true;
            if(shouldModify) {
               ResetLastError();
               if(!OrderModify(OrderTicket(),openP,finalSL,OrderTakeProfit(),0,(oType==OP_BUY?clrGreen:clrRed))) Print("Modify Error OBVf: ",GetLastError());
            }
         }
      }
   }
}
//+------------------------------------------------------------------+
//| SECTION 8.2 - PARTIAL TP VISUALS & LOGIC                         |
//+------------------------------------------------------------------+
string IntegerToOrdinalString(int number) {
   if(number<=0) return IntegerToString(number);
   int lastDigit=number%10;
   int lastTwoDigits=number%100;
   if(lastTwoDigits>=11&&lastTwoDigits<=13) return IntegerToString(number)+"th";
   switch(lastDigit) {
      case 1: return IntegerToString(number)+"st";
      case 2: return IntegerToString(number)+"nd";
      case 3: return IntegerToString(number)+"rd";
      default: return IntegerToString(number)+"th";
   }
}
void DrawPartialTPLines(double openPrice,int orderType) {
   if(ptp_TradeTicket<=0) return;
   int totalTiers=(int)MathFloor(100.0/MathMax(1,D2D_PartialTP_Percent));
   for(int i=1; i<=totalTiers; i++) {
      double priceLevel=(orderType==OP_BUY)?NormalizeDouble(openPrice+(i*ptp_ProfitStep),Digits):NormalizeDouble(openPrice-(i*ptp_ProfitStep),Digits);
      string lineName=ea_prefix+"ptp_line_"+IntegerToString(i);
      string labelName=ea_prefix+"ptp_label_"+IntegerToString(i);
      if(ObjectFind(0,lineName)<0) ObjectCreate(0,lineName,OBJ_HLINE,0,0,priceLevel);
      ObjectSetInteger(0,lineName,OBJPROP_COLOR,clrWhite);
      ObjectSetInteger(0,lineName,OBJPROP_STYLE,STYLE_DOT);
      ObjectSetInteger(0,lineName,OBJPROP_BACK,true);
      if(ObjectFind(0,labelName)<0) ObjectCreate(0,labelName,OBJ_TEXT,0,Time[0],priceLevel);
      ObjectSetString(0,labelName,OBJPROP_TEXT,IntegerToOrdinalString(i)+" TP");
      ObjectSetInteger(0,labelName,OBJPROP_FONTSIZE,8);
      ObjectSetInteger(0,labelName,OBJPROP_COLOR,clrWhite);
      ObjectSetInteger(0,labelName,OBJPROP_ANCHOR,ANCHOR_RIGHT);
   }
}
void DrawLeapFrogLines(double openPrice,int orderType) {
   if(leap_TradeTicket<=0) return;
   color leapColor=C'96,95,113';
   int tiersToDraw = (leap_TiersReached < 5) ? 10 : leap_TiersReached + 5;
   for(int i=1; i<=tiersToDraw; i++) {
      double priceLevel=(orderType==OP_BUY)?NormalizeDouble(openPrice+(i*leap_ProfitStep),Digits):NormalizeDouble(openPrice-(i*leap_ProfitStep),Digits);
      string lineName=ea_prefix+"leap_line_"+IntegerToString(i);
      string labelName=ea_prefix+"leap_label_"+IntegerToString(i);
      if(ObjectFind(0,lineName)<0) ObjectCreate(0,lineName,OBJ_HLINE,0,0,priceLevel);
      ObjectSetInteger(0,lineName,OBJPROP_COLOR,leapColor);
      ObjectSetInteger(0,lineName,OBJPROP_STYLE,STYLE_DOT);
      ObjectSetInteger(0,lineName,OBJPROP_BACK,true);
      if(ObjectFind(0,labelName)<0) ObjectCreate(0,labelName,OBJ_TEXT,0,Time[0],priceLevel);
      ObjectSetString(0,labelName,OBJPROP_TEXT,IntegerToOrdinalString(i)+" Leap Frog SL");
      ObjectSetInteger(0,labelName,OBJPROP_FONTSIZE,8);
      ObjectSetInteger(0,labelName,OBJPROP_COLOR,leapColor);
      ObjectSetInteger(0,labelName,OBJPROP_ANCHOR,ANCHOR_LEFT);
   }
}
void DrawOBVfriendPartialTPLines(double openPrice,int orderType) {
   if(obvfriend_ptp_TradeTicket<=0) return;
   int totalTiers=(int)MathFloor(100.0/MathMax(1,OBV_PartialTP_Percent));
   for(int i=1; i<=totalTiers; i++) {
      double priceLevel=(orderType==OP_BUY)?NormalizeDouble(openPrice+(i*obvfriend_ptp_ProfitStep),Digits):NormalizeDouble(openPrice-(i*obvfriend_ptp_ProfitStep),Digits);
      string lineName=ea_prefix+"obvfriend_ptp_line_"+IntegerToString(i);
      string labelName=ea_prefix+"obvfriend_ptp_label_"+IntegerToString(i);
      if(ObjectFind(0,lineName)<0) ObjectCreate(0,lineName,OBJ_HLINE,0,0,priceLevel);
      ObjectSetInteger(0,lineName,OBJPROP_COLOR,clrAqua);
      ObjectSetInteger(0,lineName,OBJPROP_STYLE,STYLE_DOT);
      ObjectSetInteger(0,lineName,OBJPROP_BACK,true);
      if(ObjectFind(0,labelName)<0) ObjectCreate(0,labelName,OBJ_TEXT,0,Time[0],priceLevel);
      ObjectSetString(0,labelName,OBJPROP_TEXT,"OBVf "+IntegerToOrdinalString(i)+" TP");
      ObjectSetInteger(0,labelName,OBJPROP_FONTSIZE,8);
      ObjectSetInteger(0,labelName,OBJPROP_COLOR,clrAqua);
      ObjectSetInteger(0,labelName,OBJPROP_ANCHOR,ANCHOR_RIGHT);
   }
}
void DrawOBVfriendLeapFrogLines(double openPrice,int orderType) {
   if(obvfriend_leap_TradeTicket<=0) return;
   color leapColor=C'96,95,113';
   int tiersToDraw = (obvfriend_leap_TiersReached < 5) ? 10 : obvfriend_leap_TiersReached + 5;
   for(int i=1; i<=tiersToDraw; i++) {
      double priceLevel=(orderType==OP_BUY)?NormalizeDouble(openPrice+(i*obvfriend_leap_ProfitStep),Digits):NormalizeDouble(openPrice-(i*obvfriend_leap_ProfitStep),Digits);
      string lineName=ea_prefix+"obvfriend_leap_line_"+IntegerToString(i);
      string labelName=ea_prefix+"obvfriend_leap_label_"+IntegerToString(i);
      if(ObjectFind(0,lineName)<0) ObjectCreate(0,lineName,OBJ_HLINE,0,0,priceLevel);
      ObjectSetInteger(0,lineName,OBJPROP_COLOR,leapColor);
      ObjectSetInteger(0,lineName,OBJPROP_STYLE,STYLE_DOT);
      ObjectSetInteger(0,lineName,OBJPROP_BACK,true);
      if(ObjectFind(0,labelName)<0) ObjectCreate(0,labelName,OBJ_TEXT,0,Time[0],priceLevel);
      ObjectSetString(0,labelName,OBJPROP_TEXT,"OBVf "+IntegerToOrdinalString(i)+" Leap Frog SL");
      ObjectSetInteger(0,labelName,OBJPROP_FONTSIZE,8);
      ObjectSetInteger(0,labelName,OBJPROP_COLOR,leapColor);
      ObjectSetInteger(0,labelName,OBJPROP_ANCHOR,ANCHOR_LEFT);
   }
}
void DeletePartialTPLines() {
   for(int i=ObjectsTotal(0,-1,-1)-1; i>=0; i--) {
      string n=ObjectName(0,i,-1,-1);
      if(StringFind(n,ea_prefix+"ptp_line_")==0||StringFind(n,ea_prefix+"ptp_label_")==0) ObjectDelete(0,n);
   }
}
void DeleteLeapFrogLines() {
   for(int i=ObjectsTotal(0,-1,-1)-1; i>=0; i--) {
      string n=ObjectName(0,i,-1,-1);
      if(StringFind(n,ea_prefix+"leap_line_")==0||StringFind(n,ea_prefix+"leap_label_")==0) ObjectDelete(0,n);
   }
}
void DeleteOBVfriendPartialTPLines() {
   for(int i=ObjectsTotal(0,-1,-1)-1; i>=0; i--) {
      string n=ObjectName(0,i,-1,-1);
      if(StringFind(n,ea_prefix+"obvfriend_ptp_line_")==0||StringFind(n,ea_prefix+"obvfriend_ptp_label_")==0) ObjectDelete(0,n);
   }
}
void DeleteOBVfriendLeapFrogLines() {
   for(int i=ObjectsTotal(0,-1,-1)-1; i>=0; i--) {
      string n=ObjectName(0,i,-1,-1);
      if(StringFind(n,ea_prefix+"obvfriend_leap_line_")==0||StringFind(n,ea_prefix+"obvfriend_leap_label_")==0) ObjectDelete(0,n);
   }
}
void ManagePartialTP() {
   int ticket=0; double openPrice=0; int orderType=-1; datetime openTime=0;
   for(int i=OrdersTotal()-1; i>=0; i--) {
      if(OrderSelect(i,SELECT_BY_POS,MODE_TRADES)&&OrderSymbol()==Symbol()&&OrderMagicNumber()==MagicNumber) {
         ticket=OrderTicket(); openPrice=OrderOpenPrice(); orderType=OrderType(); openTime=OrderOpenTime();
         break;
      }
   }
   if(!D2D_UsePartialTP || ticket==0) {
      if(ptp_TradeTicket>0) ResetPartialTP();
      return;
   }
   if(ptp_TradeTicket>0&&ptp_TradeTicket!=ticket) {
      if(ptp_OrderOpenTime!=0&&ptp_OrderOpenTime==openTime) ptp_TradeTicket=ticket;
      else ResetPartialTP();
   }
   if(ptp_TradeTicket==0) {
      double stVal=SuperTrend[1];
      double risk=MathAbs(openPrice-stVal);
      if(stVal==0||stVal==EMPTY_VALUE||risk==0) risk=100*Point;
      double step=(risk*D2D_PartialTP_Step_Risk_Mult)+(CalcATR1M(1,atrPeriod)*D2D_PartialTP_Step_ATR_Mult);
      if(step>Point) {
         ptp_ProfitStep=step;
         ptp_InitialLotSize=OrderLots();
         ptp_TradeTicket=ticket;
         ptp_OrderOpenTime=openTime;
         ptp_TiersTaken=0;
         ptp_NextTargetPrice=(orderType==OP_BUY)?NormalizeDouble(openPrice+step,Digits):NormalizeDouble(openPrice-step,Digits);
         DrawPartialTPLines(openPrice,orderType);
      }
   }
   if(ptp_TradeTicket==ticket&&ptp_ProfitStep>0) {
      double cp=(orderType==OP_BUY)?Bid:Ask;
      bool targetHit=(orderType==OP_BUY)?(cp>=ptp_NextTargetPrice):(cp<=ptp_NextTargetPrice);
      if(targetHit) {
         double lotStep=MarketInfo(Symbol(),MODE_LOTSTEP);
         double rawCloseLot=ptp_InitialLotSize*((double)D2D_PartialTP_Percent/100.0);
         double cl=NormalizeDouble(MathRound(rawCloseLot/lotStep)*lotStep,2);
         double minL=MarketInfo(Symbol(),MODE_MINLOT);
         if(cl<minL) cl=minL;
         if(OrderLots()>minL&&cl<=NormalizeDouble(OrderLots()-0.001,2)) {
            if(OrderClose(ticket,cl,(orderType==OP_BUY?Bid:Ask),Slippage,clrGreen)) {
               ptp_TiersTaken++;
               ptp_NextTargetPrice=(orderType==OP_BUY)?NormalizeDouble(openPrice+(ptp_ProfitStep*(ptp_TiersTaken+1)),Digits):NormalizeDouble(openPrice-(ptp_ProfitStep*(ptp_TiersTaken+1)),Digits);
            }
         }
         else if(OrderLots()<=cl+0.001) {
            if(OrderClose(ticket,OrderLots(),(orderType==OP_BUY?Bid:Ask),Slippage,clrGreen)) ResetPartialTP();
         }
      }
   }
}
void ManageLeapFrogSL() {
   int ticket=0; double openPrice=0; int orderType=-1; datetime openTime=0;
   for(int i=OrdersTotal()-1; i>=0; i--) {
      if(OrderSelect(i,SELECT_BY_POS,MODE_TRADES)&&OrderSymbol()==Symbol()&&OrderMagicNumber()==MagicNumber) {
         ticket=OrderTicket(); openPrice=OrderOpenPrice(); orderType=OrderType(); openTime=OrderOpenTime();
         break;
      }
   }
   if(!D2D_Leap_Frog_SL || ticket==0) {
      if(leap_TradeTicket>0) ResetLeapFrog();
      return;
   }
   if(leap_TradeTicket>0&&leap_TradeTicket!=ticket) {
      if(leap_OrderOpenTime!=0&&leap_OrderOpenTime==openTime) leap_TradeTicket=ticket;
      else ResetLeapFrog();
   }
   if(leap_TradeTicket==0) {
      double stVal=SuperTrend[1];
      double risk=MathAbs(openPrice-stVal);
      if(stVal==0||stVal==EMPTY_VALUE||risk==0) risk=100*Point;
      double step=(risk*D2D_Leap_Frog_Risk_Mult)+(CalcATR1M(1,atrPeriod)*D2D_Leap_Frog_ATR_Mult);
      if(step>Point) {
         leap_ProfitStep=step;
         leap_TradeTicket=ticket;
         leap_OrderOpenTime=openTime;
         leap_TiersReached=0;
         leap_NextTargetPrice=(orderType==OP_BUY)?NormalizeDouble(openPrice+step,Digits):NormalizeDouble(openPrice-step,Digits);
         if(!D2D_UsePartialTP) DrawLeapFrogLines(openPrice,orderType);
      }
   }
   if(leap_TradeTicket==ticket&&leap_ProfitStep>0) {
      double cp=(orderType==OP_BUY)?Bid:Ask;
      bool targetHit=(orderType==OP_BUY)?(cp>=leap_NextTargetPrice):(cp<=leap_NextTargetPrice);
      if(targetHit) {
         leap_TiersReached++;
         leap_NextTargetPrice=(orderType==OP_BUY)?NormalizeDouble(openPrice+(leap_ProfitStep*(leap_TiersReached+1)),Digits):NormalizeDouble(openPrice-(leap_ProfitStep*(leap_TiersReached+1)),Digits);
         if(!D2D_UsePartialTP) DrawLeapFrogLines(openPrice,orderType);
      }
   }
}
void ManageOBVfriendPartialTP() {
   int ticket=0; double openPrice=0; int orderType=-1; datetime openTime=0;
   for(int i=OrdersTotal()-1; i>=0; i--) {
      if(OrderSelect(i,SELECT_BY_POS,MODE_TRADES)&&OrderSymbol()==Symbol()&&OrderMagicNumber()==OBVfriendMagicNumber) {
         ticket=OrderTicket(); openPrice=OrderOpenPrice(); orderType=OrderType(); openTime=OrderOpenTime();
         break;
      }
   }
   if(ticket==0) {
      if(obvfriend_ptp_TradeTicket>0) ResetOBVfriendPartialTP();
      return;
   }
   if(obvfriend_ptp_TradeTicket>0&&obvfriend_ptp_TradeTicket!=ticket) {
      if(obvfriend_ptp_OrderOpenTime!=0&&obvfriend_ptp_OrderOpenTime==openTime) obvfriend_ptp_TradeTicket=ticket;
      else ResetOBVfriendPartialTP();
   }
   if(obvfriend_ptp_TradeTicket==0) {
      double stVal=OBVfriend_SuperTrend[1];
      double risk=MathAbs(openPrice-stVal);
      if(stVal==0||stVal==EMPTY_VALUE||risk==0) risk=100*Point;
      double step=(risk*OBV_PartialTP_Step_Risk_Mult)+(CalcATR1M(1,atrPeriod)*OBV_PartialTP_Step_ATR_Mult);
      if(step>Point) {
         obvfriend_ptp_ProfitStep=step;
         obvfriend_ptp_InitialLotSize=OrderLots();
         obvfriend_ptp_TradeTicket=ticket;
         obvfriend_ptp_OrderOpenTime=openTime;
         obvfriend_ptp_TiersTaken=0;
         obvfriend_ptp_NextTargetPrice=(orderType==OP_BUY)?NormalizeDouble(openPrice+step,Digits):NormalizeDouble(openPrice-step,Digits);
         DrawOBVfriendPartialTPLines(openPrice,orderType);
      }
   }
   if(obvfriend_ptp_TradeTicket==ticket&&obvfriend_ptp_ProfitStep>0) {
      double cp=(orderType==OP_BUY)?Bid:Ask;
      bool targetHit=(orderType==OP_BUY)?(cp>=obvfriend_ptp_NextTargetPrice):(cp<=obvfriend_ptp_NextTargetPrice);
      if(targetHit) {
         if(OBV_UsePartialTP) {
            double lotStep=MarketInfo(Symbol(),MODE_LOTSTEP);
            double rawCloseLot=obvfriend_ptp_InitialLotSize*((double)OBV_PartialTP_Percent/100.0);
            double cl=NormalizeDouble(MathRound(rawCloseLot/lotStep)*lotStep,2);
            double minL=MarketInfo(Symbol(),MODE_MINLOT);
            if(cl<minL) cl=minL;
            if(OrderLots()>minL&&cl<=NormalizeDouble(OrderLots()-0.001,2)) {
               if(OrderClose(ticket,cl,(orderType==OP_BUY?Bid:Ask),Slippage,clrGreen)) {
                  obvfriend_ptp_TiersTaken++;
                  obvfriend_ptp_NextTargetPrice=(orderType==OP_BUY)?NormalizeDouble(openPrice+(obvfriend_ptp_ProfitStep*(obvfriend_ptp_TiersTaken+1)),Digits):NormalizeDouble(openPrice-(obvfriend_ptp_ProfitStep*(obvfriend_ptp_TiersTaken+1)),Digits);
               }
            }
            else if(OrderLots()<=cl+0.001) {
               if(OrderClose(ticket,OrderLots(),(orderType==OP_BUY?Bid:Ask),Slippage,clrGreen)) ResetOBVfriendPartialTP();
            }
         }
         else {
            obvfriend_ptp_TiersTaken++;
            obvfriend_ptp_NextTargetPrice=(orderType==OP_BUY)?NormalizeDouble(openPrice+(obvfriend_ptp_ProfitStep*(obvfriend_ptp_TiersTaken+1)),Digits):NormalizeDouble(openPrice-(obvfriend_ptp_ProfitStep*(obvfriend_ptp_TiersTaken+1)),Digits);
         }
      }
   }
}
void ManageOBVfriendLeapFrogSL() {
   int ticket=0; double openPrice=0; int orderType=-1; datetime openTime=0;
   for(int i=OrdersTotal()-1; i>=0; i--) {
      if(OrderSelect(i,SELECT_BY_POS,MODE_TRADES)&&OrderSymbol()==Symbol()&&OrderMagicNumber()==OBVfriendMagicNumber) {
         ticket=OrderTicket(); openPrice=OrderOpenPrice(); orderType=OrderType(); openTime=OrderOpenTime();
         break;
      }
   }
   if(!OBV_Leap_Frog_SL || ticket==0) {
      if(obvfriend_leap_TradeTicket>0) ResetOBVfriendLeapFrog();
      return;
   }
   if(obvfriend_leap_TradeTicket>0&&obvfriend_leap_TradeTicket!=ticket) {
      if(obvfriend_leap_OrderOpenTime!=0&&obvfriend_leap_OrderOpenTime==openTime) obvfriend_leap_TradeTicket=ticket;
      else ResetOBVfriendLeapFrog();
   }
   if(obvfriend_leap_TradeTicket==0) {
      double stVal=OBVfriend_SuperTrend[1];
      double risk=MathAbs(openPrice-stVal);
      if(stVal==0||stVal==EMPTY_VALUE||risk==0) risk=100*Point;
      double step=(risk*OBV_Leap_Frog_Risk_Mult)+(CalcATR1M(1,atrPeriod)*OBV_Leap_Frog_ATR_Mult);
      if(step>Point) {
         obvfriend_leap_ProfitStep=step;
         obvfriend_leap_TradeTicket=ticket;
         obvfriend_leap_OrderOpenTime=openTime;
         obvfriend_leap_TiersReached=0;
         obvfriend_leap_NextTargetPrice=(orderType==OP_BUY)?NormalizeDouble(openPrice+step,Digits):NormalizeDouble(openPrice-step,Digits);
         if(!OBV_UsePartialTP) DrawOBVfriendLeapFrogLines(openPrice,orderType);
      }
   }
   if(obvfriend_leap_TradeTicket==ticket&&obvfriend_leap_ProfitStep>0) {
      double cp=(orderType==OP_BUY)?Bid:Ask;
      bool targetHit=(orderType==OP_BUY)?(cp>=obvfriend_leap_NextTargetPrice):(cp<=obvfriend_leap_NextTargetPrice);
      if(targetHit) {
         obvfriend_leap_TiersReached++;
         obvfriend_leap_NextTargetPrice=(orderType==OP_BUY)?NormalizeDouble(openPrice+(obvfriend_leap_ProfitStep*(obvfriend_leap_TiersReached+1)),Digits):NormalizeDouble(openPrice-(obvfriend_leap_ProfitStep*(obvfriend_leap_TiersReached+1)),Digits);
         if(!OBV_UsePartialTP) DrawOBVfriendLeapFrogLines(openPrice,orderType);
      }
   }
}
void ResetPartialTP() {
   DeletePartialTPLines();
   ptp_ProfitStep=0.0;
   ptp_NextTargetPrice=0.0;
   ptp_InitialLotSize=0.0;
   ptp_TradeTicket=0;
   ptp_OrderOpenTime=0;
   ptp_TiersTaken=0;
}
void ResetLeapFrog() {
   DeleteLeapFrogLines();
   leap_ProfitStep=0.0;
   leap_NextTargetPrice=0.0;
   leap_TradeTicket=0;
   leap_OrderOpenTime=0;
   leap_TiersReached=0;
}
void ResetOBVfriendPartialTP() {
   DeleteOBVfriendPartialTPLines();
   obvfriend_ptp_ProfitStep=0.0;
   obvfriend_ptp_NextTargetPrice=0.0;
   obvfriend_ptp_InitialLotSize=0.0;
   obvfriend_ptp_TradeTicket=0;
   obvfriend_ptp_OrderOpenTime=0;
   obvfriend_ptp_TiersTaken=0;
}
void ResetOBVfriendLeapFrog() {
   DeleteOBVfriendLeapFrogLines();
   obvfriend_leap_ProfitStep=0.0;
   obvfriend_leap_NextTargetPrice=0.0;
   obvfriend_leap_TradeTicket=0;
   obvfriend_leap_OrderOpenTime=0;
   obvfriend_leap_TiersReached=0;
}
//+------------------------------------------------------------------+
//| SECTION 8.3 - ORDER MANAGEMENT HELPERS                           |
//+------------------------------------------------------------------+
double ValidateStopLoss(int orderType,double proposedSL) {
   RefreshRates();
   double stopLevel=MarketInfo(Symbol(),MODE_STOPLEVEL)*Point;
   double minDist=stopLevel+(MarketInfo(Symbol(),MODE_SPREAD)*Point);
   if(orderType==OP_BUY&&proposedSL>Bid-minDist) return NormalizeDouble(Bid-minDist,Digits);
   if(orderType==OP_SELL&&proposedSL<Ask+minDist) return NormalizeDouble(Ask+minDist,Digits);
   return NormalizeDouble(proposedSL,Digits);
}
bool HasOpenOrder(int orderType) {
   for(int i=OrdersTotal()-1; i>=0; i--) {
      if(OrderSelect(i,SELECT_BY_POS,MODE_TRADES)) {
         if(OrderSymbol()==Symbol()&&OrderType()==orderType&&OrderMagicNumber()==MagicNumber) return true;
      }
   }
   return false;
}
int FindOpenOBVfriendTrade() {
   for(int i=OrdersTotal()-1; i>=0; i--) {
      if(OrderSelect(i,SELECT_BY_POS,MODE_TRADES)) {
         if(OrderSymbol()==Symbol()&&OrderMagicNumber()==OBVfriendMagicNumber) return OrderType();
      }
   }
   return -1;
}
void CloseOrders(int orderType) {
   for(int i=OrdersTotal()-1; i>=0; i--) {
      if(OrderSelect(i,SELECT_BY_POS,MODE_TRADES)) {
         if(OrderSymbol()==Symbol()&&OrderType()==orderType&&OrderMagicNumber()==MagicNumber) {
            if(!OrderClose(OrderTicket(),OrderLots(),(orderType==OP_BUY?Bid:Ask),Slippage,clrRed)) Print("Close Error");
         }
      }
   }
}
void CloseAllOBVfriendOrders() {
   ResetOBVfriendPartialTP();
   for(int i=OrdersTotal()-1; i>=0; i--) {
      if(OrderSelect(i,SELECT_BY_POS,MODE_TRADES)) {
         if(OrderSymbol()==Symbol()&&OrderMagicNumber()==OBVfriendMagicNumber) {
            if(!OrderClose(OrderTicket(),OrderLots(),(OrderType()==OP_BUY?Bid:Ask),Slippage,clrRed)) Print("Close Error");
         }
      }
   }
}
void CloseManualTrades() {
   for(int i=OrdersTotal()-1; i>=0; i--) {
      if(OrderSelect(i,SELECT_BY_POS,MODE_TRADES)) {
         if(OrderSymbol()==Symbol()&&OrderMagicNumber()==ManualMagicNumber) {
            if(!OrderClose(OrderTicket(),OrderLots(),(OrderType()==OP_BUY?Bid:Ask),Slippage,clrRed)) Print("Close Error");
         }
      }
   }
}
int GetPeriodSeconds() {
   return(Period()*60);
}
//+------------------------------------------------------------------+
//| SECTION 8.4 - SESSION ENFORCEMENT LOGIC                          |
//+------------------------------------------------------------------+
void EnforceTradingSession() {
   if(!UseOpenHours) return;
   static bool sessionCloseActionTaken=false;
   datetime estTime=GetEstTime();
   int currentHourEST=TimeHour(estTime);
   int currentMinuteEST=TimeMinute(estTime);
   int openTimeTotalMinutes=OpenHourEST*60+OpenMinuteEST;
   int closeTimeTotalMinutes=CloseHourEST*60+CloseMinuteEST;
   int currentTimeTotalMinutes=currentHourEST*60+currentMinuteEST;
   bool inSession;
   if(openTimeTotalMinutes>closeTimeTotalMinutes) inSession=(currentTimeTotalMinutes>=openTimeTotalMinutes||currentTimeTotalMinutes<closeTimeTotalMinutes);
   else inSession=(currentTimeTotalMinutes>=openTimeTotalMinutes&&currentTimeTotalMinutes<closeTimeTotalMinutes);
   if(inSession) {
      if(sessionCloseActionTaken) sessionCloseActionTaken=false;
      return;
   }
   if(!sessionCloseActionTaken&&!ForceOpenHoursTrade) {
      bool actionNeeded = false;
      for(int i=OrdersTotal()-1; i>=0; i--) {
         if(OrderSelect(i,SELECT_BY_POS,MODE_TRADES) && OrderSymbol()==Symbol() && OrderMagicNumber()==MagicNumber) {
            if(!IsTradeRiskFree(OrderTicket())) {
               actionNeeded = true;
               if(!OrderClose(OrderTicket(),OrderLots(),(OrderType()==OP_BUY?Bid:Ask),Slippage,clrRed)) Print("Close Error");
            }
         }
      }
      if(actionNeeded || HasOpenOrder(OP_BUY) || HasOpenOrder(OP_SELL)) {
         if(actionNeeded) Print("Enforcing trading session. Closing unprotected positions outside the allowed window.");
         sessionCloseActionTaken=true;
      }
   }
}
void EnforceBlockTimeSession() {
   if(!UseBlockTime) return;
   static bool blockCloseActionTaken=false;
   datetime estTime=GetEstTime();
   int currentHourEST=TimeHour(estTime);
   int currentMinuteEST=TimeMinute(estTime);
   int blockStartTimeTotalMinutes=BlockStartHourEST*60+BlockStartMinuteEST;
   int blockEndTimeTotalMinutes=BlockEndHourEST*60+BlockEndMinuteEST;
   int currentTimeTotalMinutes=currentHourEST*60+currentMinuteEST;
   bool inBlockSession=(currentTimeTotalMinutes>=blockStartTimeTotalMinutes&&currentTimeTotalMinutes<blockEndTimeTotalMinutes);
   if(!inBlockSession) {
      if(blockCloseActionTaken) blockCloseActionTaken=false;
      return;
   }
   if(!blockCloseActionTaken) {
      bool actionNeeded=false;
      for(int i=OrdersTotal()-1; i>=0; i--) {
         if(OrderSelect(i,SELECT_BY_POS,MODE_TRADES)&&OrderSymbol()==Symbol()&&(OrderMagicNumber()==MagicNumber||OrderMagicNumber()==OBVfriendMagicNumber)) {
            if(!IsTradeRiskFree(OrderTicket())) {
               actionNeeded=true;
               if(!OrderClose(OrderTicket(),OrderLots(),(OrderType()==OP_BUY?Bid:Ask),Slippage,clrRed)) Print("Close Error");
            }
         }
      }
      bool d2d_trade_found=false;
      for(int i=OrdersTotal()-1; i>=0; i--) {
         if(OrderSelect(i,SELECT_BY_POS,MODE_TRADES)&&OrderSymbol()==Symbol()&&OrderMagicNumber()==MagicNumber) { d2d_trade_found=true; break; }
      }
      bool obvfriend_trade_found=(FindOpenOBVfriendTrade()!=-1);
      if(actionNeeded || d2d_trade_found || obvfriend_trade_found) {
         if(actionNeeded) Print("Enforcing block time. Closing unprotected positions inside the blocked window.");
         blockCloseActionTaken=true;
      }
   }
}
//+------------------------------------------------------------------+
//| SECTION 8.5 - DATA CACHING HELPER & RE-PAINT                     |
//+------------------------------------------------------------------+
void RefreshCachedData() {
   cachedTickValue=MarketInfo(Symbol(),MODE_TICKVALUE);
   cachedTickSize=MarketInfo(Symbol(),MODE_TICKSIZE);
   cachedStopLevel=MarketInfo(Symbol(),MODE_STOPLEVEL);
   cachedSpreadInPoints=MarketInfo(Symbol(),MODE_SPREAD);
}
void RedrawD2DSignals() {
   for(int i=Bars-1; i>=1; i--) {
      if(LockBuffer[i]!=0) {
         color signalColor=(LockBuffer[i]==1)?C'146,134,124':C'89,116,124';
         DrawSignalOnChart(Time[i],signalColor);
      }
   }
   ChartRedraw();
}
void ForceRePaintSignals() {
   Print("System: Executing final visual alignment...");
   DeleteSignalMarkers();
   DeleteTrendFlipVLines();
   DeleteOBV_Visuals();
   ResizeAllArrays(Bars);
   int startBar=Bars-1;
   if(startBar<1) startBar=1;
   for(int i=startBar; i>=1; i--) {
      hist_VolumeValue[i]=(double)Volume[i];
      Calc_PoC_State_OnBar(i);
      Calc_Momentum_OnBar(i);
      Calc_ADX_OnBar(i);
      Calc_OBV_OnBar(i);
      CalcATR1M(i,atrPeriod);
      Calc_D2D_ST_OnBar(i);
      Calc_AdaptiveTrend_OnBar(i);
      Calc_OBVfriend_ST_OnBar(i);
      Calc_HarmVol_LLEMA_OnBar(i);
      Calc_Sqz_Momentum_OnBar(i);
      Calc_RangeOsc_OnBar(i);
      Calc_Microstructure_OnBar(i);
      Calc_Dots_Derived_OnBar(i);
      Calc_VWAP_OnBar(i);
      Calc_RefLevels_OnBar(i);
      Calc_MultiDay_OnBar(i);
      hist_ADXValue[i]=ADXBuffer[i];
      if(LockBuffer[i]!=0) {
         if(LockTime[i]==0) LockTime[i]=Time[i];
         if(isSignalDotsVisible) {
            color signalColor=(LockBuffer[i]==1)?C'146,134,124':C'89,116,124';
            DrawSignalOnChart(Time[i],signalColor);
         }
      }
   }
   Calc_PoC_State_OnBar(0);
   Calc_Momentum_OnBar(0);
   Calc_ADX_OnBar(0);
   Calc_OBV_OnBar(0);
   CalcATR1M(0,atrPeriod);
   Calc_D2D_ST_OnBar(0);
   Calc_AdaptiveTrend_OnBar(0);
   Calc_OBVfriend_ST_OnBar(0);
   Calc_HarmVol_LLEMA_OnBar(0);
   Calc_Sqz_Momentum_OnBar(0);
   Calc_RangeOsc_OnBar(0);
   Calc_Microstructure_OnBar(0);
   Calc_Dots_Derived_OnBar(0);
   Calc_VWAP_OnBar(0);
   Calc_RefLevels_OnBar(0);
   Calc_MultiDay_OnBar(0);
   DrawHistoricalIndicators_FromBuffers();
   ColorSignalBars();
   if(isSuperTrendVisible) DrawSuperTrendLine();
   if(isOBVfriendSuperTrendVisible) DrawOBVfriendSuperTrendLine();
   DrawLiveSignalBarSegment();
   if(isSuperTrendVisible) DrawLiveSuperTrendSegment();
   if(isOBVfriendSuperTrendVisible) DrawLiveOBVfriendSuperTrendSegment();
   if(isOBVVisualsVisible) DrawLiveOBVSegment();
   DrawLiveST_TrendDirectionIndicator();
   DrawLiveLT_TrendDirectionIndicator();
   SquashChartToMiddleThird(ChartID());
   g_loadingProgress=1.0;
   DrawLoadingBar(g_loadingProgress,"Loaded");
   g_loadingBarHideTime=TimeCurrent()+30;
   LogBootMessage("System Live. Monitoring...");
   ChartRedraw();
   Print("Visual alignment complete.");
}
//+------------------------------------------------------------------+
//| SECTION 8.6 - DOTS TRADE MANAGEMENT                              |
//+------------------------------------------------------------------+
void HandleDotsTradeSignal() {
   if(!UseDots) return;
   if(!isDotsTradeActive) return;
   if(MaxDailyLoss>0&&combinedCurrentDailyLoss>=MaxDailyLoss) return;
   datetime estNow=GetEstTime();
   int estDay=GetEstDayOfWeek(TimeGMT());
   int estHour=TimeHour(estNow);
   int estMinute=TimeMinute(estNow);
   if(estDay==5&&(estHour>16||(estHour==16&&estMinute>=45))) return;
   dots_active_count=0;
   for(int c=0;c<DOTS_NUM_RULES;c++)
      if(dots_state[c].ticket>0) dots_active_count++;
   for(int i=0;i<DOTS_NUM_RULES;i++) {
      if(!dots_cleared[i]) continue;
      if(dots_state[i].ticket>0) continue;
      if(dots_active_count>=Dots_MaxPositions) {
         Print("DOTS| BLOCKED R",i,": Max positions (",dots_active_count,"/",Dots_MaxPositions,")");
         DotsLog("CAP: blocked "+dots_ruleName[i]);
         break;
      }
      double atr=ATR_1M_Array[1];
      double risk=MathMin(atr*Dots_SL_Mult,Dots_SL_Cap);
      if(risk<=0.0) {
         Print("DOTS| BLOCKED R",i,": ATR zero, cannot compute SL");
         continue;
      }
      int dir=dots_rules[i].direction;
      int cmd=(dir==1)?OP_BUY:OP_SELL;
      RefreshRates();
      double entryPrice=(dir==1)?Ask:Bid;
      double rawSL;
      if(dir==1) rawSL=entryPrice-risk*Point;
      else rawSL=entryPrice+risk*Point;
      double sl=ValidateStopLoss(cmd,rawSL);
      double lots=Dots_LotSize;
      double minLot=MarketInfo(Symbol(),MODE_MINLOT);
      double maxLot=MarketInfo(Symbol(),MODE_MAXLOT);
      double lotStep=MarketInfo(Symbol(),MODE_LOTSTEP);
      if(lotStep>0) lots=MathRound(lots/lotStep)*lotStep;
      if(lots<minLot) lots=minLot;
      if(lots>maxLot) lots=maxLot;
      double marginCheck=AccountFreeMarginCheck(Symbol(),cmd,lots);
      if(marginCheck<=0.0||GetLastError()==134) {
         Print("DOTS| BLOCKED R",i,": Insufficient margin");
         ResetLastError();
         continue;
      }
      string comment="DOTS_R"+IntegerToString(i)+"_"+Symbol()+"_"+IntegerToString(DotsMagicNumber);
      color arrowClr=(dir==1)?C'0,255,127':C'255,82,82';
      RefreshRates();
      entryPrice=(dir==1)?Ask:Bid;
      int ticket=OrderSend(Symbol(),cmd,lots,entryPrice,Slippage,sl,0,comment,DotsMagicNumber,0,arrowClr);
      if(ticket>0) {
         double step=Dots_StepFrac*risk;
         double beTrig=Dots_BE_Trigger*step;
         double beLock=Dots_BE_LockFrac*beTrig;
         dots_state[i].ruleIndex=i;
         dots_state[i].ticket=ticket;
         dots_state[i].direction=dir;
         dots_state[i].entryPrice=entryPrice;
         dots_state[i].initialRisk=risk;
         dots_state[i].stepSize=step;
         dots_state[i].currentSL=sl;
         dots_state[i].be_trigger=beTrig;
         dots_state[i].be_lock_dist=beLock;
         dots_state[i].tiersReached=0;
         dots_state[i].beNudged=false;
         dots_active_count++;
         dots_total_trades++;
         DotsLogEntry(i,entryPrice,sl,risk,ticket);
         PlayDotsAlert(i,dir);
         DrawDotsEntryMarker(i,dir,Time[1],entryPrice);
         DotsLog("OPEN: "+dots_ruleName[i]+" @ "+DoubleToString(entryPrice,Digits));
      } else {
         Print("DOTS| ORDER FAILED R",i,": Error=",GetLastError());
         ResetLastError();
      }
   }
}
//+------------------------------------------------------------------+
//| SECTION 8.7 - DOTS HELPERS                                       |
//+------------------------------------------------------------------+
void DotsResetRule(int idx) {
   dots_state[idx].ticket=0;
   dots_state[idx].direction=0;
   dots_state[idx].entryPrice=0.0;
   dots_state[idx].initialRisk=0.0;
   dots_state[idx].stepSize=0.0;
   dots_state[idx].currentSL=0.0;
   dots_state[idx].be_trigger=0.0;
   dots_state[idx].be_lock_dist=0.0;
   dots_state[idx].tiersReached=0;
   dots_state[idx].beNudged=false;
   dots_state[idx].condA=false;
   dots_state[idx].condB=false;
   dots_state[idx].condC=false;
}
void DotsLogEntry(int idx,double price,double sl,double risk,int ticket) {
   int dir=dots_rules[idx].direction;
   string dirStr=(dir==1)?"LONG":"SHORT";
   double v1=DotsGetFeatureValue(dots_rules[idx].feat1,1);
   double v2=DotsGetFeatureValue(dots_rules[idx].feat2,1);
   double v3=DotsGetFeatureValue(dots_rules[idx].feat3,1);
   double t1=dots_threshold[dots_rules[idx].feat1][dots_rules[idx].dir1];
   double t2=dots_threshold[dots_rules[idx].feat2][dots_rules[idx].dir2];
   double t3=dots_threshold[dots_rules[idx].feat3][dots_rules[idx].dir3];
   double step=Dots_StepFrac*risk;
   double beTrig=Dots_BE_Trigger*step;
   Print("DOTS| >>> ENTRY R",idx," [",dirStr,"] @ ",DoubleToString(price,Digits),
         " SL=",DoubleToString(sl,Digits),
         " Risk=",DoubleToString(risk,1),
         " Step=",DoubleToString(step,1),
         " BETrig=",DoubleToString(beTrig,1),
         " Ticket=",ticket);
   Print("DOTS|     F1=",DoubleToString(v1,6)," thr=",DoubleToString(t1,6),
         "  F2=",DoubleToString(v2,6)," thr=",DoubleToString(t2,6),
         "  F3=",DoubleToString(v3,6)," thr=",DoubleToString(t3,6));
}
void DotsLogExit(int idx,string reason,double exitPrice,double pnl) {
   int dir=dots_state[idx].direction;
   string dirStr=(dir==1)?"LONG":"SHORT";
   dots_rule_pnl[idx]+=pnl;
   dots_today_pnl+=pnl;
   if(pnl>=0.0) { dots_rule_wins[idx]++; dots_today_wins++; dots_gross_win+=pnl; }
   else { dots_rule_losses[idx]++; dots_today_losses++; dots_gross_loss+=MathAbs(pnl); }
   Print("DOTS| <<< EXIT R",idx," ",reason,
         " [",dirStr,"] @ ",DoubleToString(exitPrice,Digits),
         " P&L=",DoubleToString(pnl,2),
         " Tiers=",dots_state[idx].tiersReached,
         " BE=",((dots_state[idx].beNudged)?"Y":"N"));
   DrawDotsExitMarker(idx,dir,Time[1],exitPrice,(pnl>=0.0));
   if(pnl>=0.0) DotsLog("EXIT: +"+DoubleToString(pnl,2)+" "+dots_ruleName[idx]+" "+reason);
   else DotsLog("EXIT: "+DoubleToString(pnl,2)+" "+dots_ruleName[idx]+" "+reason);
   if(pnl<0.0&&MaxDailyLoss>0) {
      double ftmoUsed=combinedCurrentDailyLoss/MaxDailyLoss*100.0;
      if(ftmoUsed>=75.0)
         Print("DOTS| FTMO DAILY LOSS CRITICAL: ",DoubleToString(ftmoUsed,1),"%");
      else if(ftmoUsed>=50.0)
         Print("DOTS| FTMO DAILY LOSS AT ",DoubleToString(ftmoUsed,1),
               "% ($",DoubleToString(combinedCurrentDailyLoss,2),
               " / $",DoubleToString(MaxDailyLoss,2),")");
   }
}
void DotsHandleBrokerClose(int idx) {
   double pnl=0.0;
   if(OrderSelect(dots_state[idx].ticket,SELECT_BY_TICKET))
      pnl=OrderProfit()+OrderSwap()+OrderCommission();
   double exitPrice=dots_state[idx].currentSL;
   if(OrderSelect(dots_state[idx].ticket,SELECT_BY_TICKET,MODE_HISTORY))
      exitPrice=OrderClosePrice();
   DotsLogExit(idx,"SL(Broker)",exitPrice,pnl);
   DotsResetRule(idx);
}
//+------------------------------------------------------------------+
//| SECTION 8.8 - DOTS POSITIONS & ALERTS                            |
//+------------------------------------------------------------------+
void ManageDotsPositions() {
   if(!UseDots) return;
   datetime estNow=GetEstTime();
   int estHour=TimeHour(estNow);
   int estMinute=TimeMinute(estNow);
   int estDay=GetEstDayOfWeek(TimeGMT());
   bool fridayClose=(estDay==5&&(estHour>16||(estHour==16&&estMinute>=45)));
   for(int i=0;i<DOTS_NUM_RULES;i++) {
      if(dots_state[i].ticket<=0) continue;
      if(!OrderSelect(dots_state[i].ticket,SELECT_BY_TICKET)||OrderCloseTime()>0) {
         DotsHandleBrokerClose(i);
         continue;
      }
      if(fridayClose) {
         int dir=dots_state[i].direction;
         RefreshRates();
         double clP=(dir==1)?Bid:Ask;
         if(OrderClose(dots_state[i].ticket,OrderLots(),clP,Slippage,clrRed)) {
            double pnl=OrderProfit()+OrderSwap()+OrderCommission();
            DotsLogExit(i,"FridayClose",clP,pnl);
            DotsLog("CLOSE: "+dots_ruleName[i]+" FriClose");
            DotsResetRule(i);
         } else {
            Print("DOTS| FRIDAY CLOSE FAILED R",i,": Error=",GetLastError());
            ResetLastError();
         }
         continue;
      }
      int dir=dots_state[i].direction;
      double entry=dots_state[i].entryPrice;
      double step=dots_state[i].stepSize;
      if(step<=0.0) continue;
      RefreshRates();
      double favourable;
      if(dir==1) favourable=(Bid-entry)/Point;
      else favourable=(entry-Ask)/Point;
      int tiers=(int)MathFloor(favourable/step);
      if(tiers<0) tiers=0;
      if(tiers>dots_state[i].tiersReached) {
         dots_state[i].tiersReached=tiers;
         DotsLog("T+"+IntegerToString(tiers)+" "+dots_ruleName[i]);
      }
      if(favourable>=dots_state[i].be_trigger&&!dots_state[i].beNudged) {
         double newSL;
         if(dir==1) newSL=entry+dots_state[i].be_lock_dist*Point;
         else newSL=entry-dots_state[i].be_lock_dist*Point;
         newSL=NormalizeDouble(newSL,Digits);
         if(OrderModify(dots_state[i].ticket,OrderOpenPrice(),newSL,OrderTakeProfit(),0,clrNONE)) {
            dots_state[i].currentSL=newSL;
            dots_state[i].beNudged=true;
            Print("DOTS| BE NUDGE R",i,
                  " SL→",DoubleToString(newSL,Digits),
                  " Lock=",DoubleToString(dots_state[i].be_lock_dist,1));
            DotsLog("BE: "+dots_ruleName[i]+" nudged");
         } else {
            Print("DOTS| BE NUDGE FAILED R",i,": Error=",GetLastError());
            ResetLastError();
         }
      }
      if(dots_state[i].tiersReached>=Dots_LF_Activation) {
         int trailTiers=dots_state[i].tiersReached-Dots_LF_Lag;
         if(trailTiers>0) {
            double newSL;
            if(dir==1) newSL=entry+trailTiers*step*Point;
            else newSL=entry-trailTiers*step*Point;
            newSL=NormalizeDouble(newSL,Digits);
            bool better=(dir==1)?(newSL>dots_state[i].currentSL):(newSL<dots_state[i].currentSL);
            if(better) {
               if(OrderModify(dots_state[i].ticket,OrderOpenPrice(),newSL,OrderTakeProfit(),0,clrNONE)) {
                  dots_state[i].currentSL=newSL;
                  Print("DOTS| LEAPFROG R",i,
                        " Tier=",dots_state[i].tiersReached,
                        " Trail=",trailTiers,
                        " SL→",DoubleToString(newSL,Digits));
                  DotsLog("LF: "+dots_ruleName[i]+" T"+IntegerToString(dots_state[i].tiersReached));
               } else {
                  Print("DOTS| LEAPFROG FAILED R",i,": Error=",GetLastError());
                  ResetLastError();
               }
            }
         }
      }
   }
   dots_active_count=0;
   for(int c=0;c<DOTS_NUM_RULES;c++)
      if(dots_state[c].ticket>0) dots_active_count++;
}
void PlayDotsAlert(int ruleIndex,int direction) {
   if(Time[0]<=dots_lastAlertTime[ruleIndex]) return;
   dots_lastAlertTime[ruleIndex]=Time[0];
   string dirStr=(direction==1)?"LONG":"SHORT";
   Alert("DOTS R",ruleIndex," ",dirStr," signal fired");
   PlaySound("alert2.wav");
}
void DrawDotsEntryMarker(int ruleIdx,int dir,datetime barTime,double price) {
   int arrowCode=(dir==1)?233:234;
   color clr=(dir==1)?C'0,255,127':C'255,82,82';
   string objName="dots_entry_"+dots_ruleName[ruleIdx]+"_"+TimeToString(barTime);
   DrawArrow(objName,barTime,price,clr,1,arrowCode);
   double offset=15.0*Point;
   double lblPrice=(dir==1)?(price-offset):(price+offset);
   string lblName=ea_prefix+objName+"_lbl";
   if(ObjectFind(0,lblName)<0) {
      ObjectCreate(0,lblName,OBJ_TEXT,0,barTime,lblPrice);
      ObjectSetString(0,lblName,OBJPROP_FONT,"Consolas");
      ObjectSetInteger(0,lblName,OBJPROP_FONTSIZE,7);
      ObjectSetInteger(0,lblName,OBJPROP_ANCHOR,ANCHOR_CENTER);
      ObjectSetInteger(0,lblName,OBJPROP_SELECTABLE,false);
      ObjectSetInteger(0,lblName,OBJPROP_BACK,true);
   }
   ObjectSetString(0,lblName,OBJPROP_TEXT,dots_ruleName[ruleIdx]);
   ObjectSetInteger(0,lblName,OBJPROP_COLOR,clr);
}
void DrawDotsExitMarker(int ruleIdx,int dir,datetime barTime,double price,bool profitable) {
   int arrowCode=profitable?159:251;
   color clr=profitable?C'0,255,127':C'255,82,82';
   string objName="dots_exit_"+dots_ruleName[ruleIdx]+"_"+TimeToString(barTime);
   DrawArrow(objName,barTime,price,clr,1,arrowCode);
}
void RemoveDotsChartMarkers() {
   string matchEntry=ea_prefix+"dots_entry_";
   string matchExit=ea_prefix+"dots_exit_";
   for(int i=ObjectsTotal(0,-1,-1)-1; i>=0; i--) {
      string name=ObjectName(0,i,-1,-1);
      if(StringFind(name,matchEntry)==0||StringFind(name,matchExit)==0)
         ObjectDelete(0,name);
   }
}
//+------------------------------------------------------------------+
//| SECTION 9.0 - STATISTICS PANEL                                   |
//+------------------------------------------------------------------+
void DrawLoadingBar(double progress, string text) {
   const int Z_PANEL_BACKGROUND = 40020;
   const int Z_PANEL_FOREGROUND = 40021;
   int chartWidth = (int)ChartGetInteger(0, CHART_WIDTH_IN_PIXELS);
   int panelW = 200;
   int panelH = 20;
   int panelX = chartWidth - panelW - 15;
   int panelY = 50;
   int barW = panelW - 6;
   int barH = panelH - 6;
   color bg_color = C'19,29,42';
   color border_color = C'95,107,119';
   color bar_bg_color = C'19,29,42';
   color bar_fill_color = C'146,134,124';
   color text_color = C'89,116,124';
   string panelName = ea_prefix + "LoadingPanel";
   string textName = ea_prefix + "LoadingText";
   string barBgName = ea_prefix + "LoadingBarBg";
   string barFillName = ea_prefix + "LoadingBarFill";
   if(ObjectFind(0, panelName) < 0) {
      ObjectCreate(0, panelName, OBJ_RECTANGLE_LABEL, 0, 0, 0);
      ObjectSetInteger(0, panelName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
   }
   ObjectSetInteger(0, panelName, OBJPROP_BACK, false);
   ObjectSetInteger(0, panelName, OBJPROP_ZORDER, Z_PANEL_BACKGROUND);
   ObjectSetInteger(0, panelName, OBJPROP_XDISTANCE, panelX);
   ObjectSetInteger(0, panelName, OBJPROP_YDISTANCE, panelY);
   ObjectSetInteger(0, panelName, OBJPROP_XSIZE, panelW);
   ObjectSetInteger(0, panelName, OBJPROP_YSIZE, panelH);
   ObjectSetInteger(0, panelName, OBJPROP_BGCOLOR, bg_color);
   ObjectSetInteger(0, panelName, OBJPROP_BORDER_COLOR, border_color);
   ObjectSetInteger(0, panelName, OBJPROP_BORDER_TYPE, BORDER_FLAT);
   if(ObjectFind(0, textName) < 0) {
      ObjectCreate(0, textName, OBJ_LABEL, 0, 0, 0);
      ObjectSetInteger(0, textName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
      ObjectSetString(0, textName, OBJPROP_FONT, "Segoe UI Bold");
      ObjectSetInteger(0, textName, OBJPROP_FONTSIZE, 10);
      ObjectSetInteger(0, textName, OBJPROP_ANCHOR, ANCHOR_LEFT_UPPER);
   }
   ObjectSetInteger(0, textName, OBJPROP_BACK, false);
   ObjectSetInteger(0, textName, OBJPROP_ZORDER, Z_PANEL_FOREGROUND + 2);
   ObjectSetInteger(0, textName, OBJPROP_XDISTANCE, panelX);
   ObjectSetInteger(0, textName, OBJPROP_YDISTANCE, panelY - 20);
   ObjectSetString(0, textName, OBJPROP_TEXT, text);
   ObjectSetInteger(0, textName, OBJPROP_COLOR, text_color);
   if(ObjectFind(0, barBgName) < 0) {
      ObjectCreate(0, barBgName, OBJ_RECTANGLE_LABEL, 0, 0, 0);
      ObjectSetInteger(0, barBgName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
   }
   ObjectSetInteger(0, barBgName, OBJPROP_BACK, false);
   ObjectSetInteger(0, barBgName, OBJPROP_ZORDER, Z_PANEL_FOREGROUND);
   ObjectSetInteger(0, barBgName, OBJPROP_XDISTANCE, panelX + 3);
   ObjectSetInteger(0, barBgName, OBJPROP_YDISTANCE, panelY + 3);
   ObjectSetInteger(0, barBgName, OBJPROP_XSIZE, barW);
   ObjectSetInteger(0, barBgName, OBJPROP_YSIZE, barH);
   ObjectSetInteger(0, barBgName, OBJPROP_BGCOLOR, bar_bg_color);
   ObjectSetInteger(0, barBgName, OBJPROP_BORDER_COLOR, border_color);
   ObjectSetInteger(0, barBgName, OBJPROP_BORDER_TYPE, BORDER_FLAT);
   if(ObjectFind(0, barFillName) < 0) {
      ObjectCreate(0, barFillName, OBJ_RECTANGLE_LABEL, 0, 0, 0);
      ObjectSetInteger(0, barFillName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
   }
   int fillW = (int)NormalizeDouble(barW * progress, 0);
   if(fillW < 0) fillW = 0;
   if(fillW > barW) fillW = barW;
   ObjectSetInteger(0, barFillName, OBJPROP_BACK, false);
   ObjectSetInteger(0, barFillName, OBJPROP_ZORDER, Z_PANEL_FOREGROUND + 1);
   ObjectSetInteger(0, barFillName, OBJPROP_XDISTANCE, panelX + 3);
   ObjectSetInteger(0, barFillName, OBJPROP_YDISTANCE, panelY + 3);
   ObjectSetInteger(0, barFillName, OBJPROP_XSIZE, fillW);
   ObjectSetInteger(0, barFillName, OBJPROP_YSIZE, barH);
   ObjectSetInteger(0, barFillName, OBJPROP_BGCOLOR, bar_fill_color);
   ObjectSetInteger(0, barFillName, OBJPROP_BORDER_COLOR, bar_fill_color);
   ObjectSetInteger(0, barFillName, OBJPROP_BORDER_TYPE, BORDER_FLAT);
}
void RemoveLoadingBar() {
   ObjectDelete(0, ea_prefix + "LoadingPanel");
   ObjectDelete(0, ea_prefix + "LoadingText");
   ObjectDelete(0, ea_prefix + "LoadingBarBg");
   ObjectDelete(0, ea_prefix + "LoadingBarFill");
   ObjectDelete(0, "LoadingPanel");
   ObjectDelete(0, "LoadingText");
   ObjectDelete(0, "LoadingBarBg");
   ObjectDelete(0, "LoadingBarFill");
}
void UpdateLiveTrackers() {
   liveLockedInProfit = 0.0;
   for(int i = OrdersTotal() - 1; i >= 0; i--) {
      if(OrderSelect(i, SELECT_BY_POS, MODE_TRADES)) {
         if(OrderSymbol() == Symbol() && OrderMagicNumber() == MagicNumber) {
            double openPrice = OrderOpenPrice();
            double stopLoss = OrderStopLoss();
            double tickValue = cachedTickValue;
            double tickSize = cachedTickSize;
            if(stopLoss > 0) {
               if(OrderType() == OP_BUY && stopLoss > openPrice) {
                  liveLockedInProfit = (stopLoss - openPrice) / tickSize * tickValue * OrderLots();
               } else if(OrderType() == OP_SELL && stopLoss < openPrice) {
                  liveLockedInProfit = (openPrice - stopLoss) / tickSize * tickValue * OrderLots();
               }
            }
            break;
         }
      }
   }
}
int DotsParseRuleFromComment(string comment) {
   int rPos=StringFind(comment,"DOTS_R");
   if(rPos<0) return -1;
   string after=StringSubstr(comment,rPos+6);
   int usPos=StringFind(after,"_");
   if(usPos<=0) return -1;
   string idxStr=StringSubstr(after,0,usPos);
   int idx=(int)StringToInteger(idxStr);
   if(idx<0||idx>=DOTS_NUM_RULES) return -1;
   return idx;
}
void UpdateStatsFromHistory() {
   if(g_isLoading) LogBootMessage("Stats: Calculating History Metrics...");
   wins = 0; losses = 0; profitTotal = 0.0; lossTotal = 0.0; totalCommissions = 0.0; currentDailyLoss = 0.0; historicalProfitSecured = 0.0;
   lastClosedTradeProfit = 0.0; lastClosedTradeClosePrice = 0.0;
   manual_wins = 0; manual_losses = 0; manual_profitTotal = 0.0; manual_lossTotal = 0.0; manual_totalCommissions = 0.0; manual_currentDailyLoss = 0.0;
   manual_lastClosedTradeProfit = 0.0; manual_lastClosedTradeClosePrice = 0.0;
   obvfriend_wins = 0; obvfriend_losses = 0; obvfriend_profitTotal = 0.0; obvfriend_lossTotal = 0.0; obvfriend_totalCommissions = 0.0; obvfriend_currentDailyLoss = 0.0;
   obvfriend_lastClosedTradeProfit = 0.0; obvfriend_lastClosedTradeClosePrice = 0.0;
   dots_hist_wins=0; dots_hist_losses=0; dots_hist_profitTotal=0.0; dots_hist_lossTotal=0.0;
   dots_hist_totalCommissions=0.0; dots_hist_currentDailyLoss=0.0;
   dots_hist_lastClosedProfit=0.0; dots_hist_lastClosedPrice=0.0;
   for(int r=0; r<DOTS_NUM_RULES; r++) { dots_rule_wins[r]=0; dots_rule_losses[r]=0; dots_rule_pnl[r]=0.0; }
   aus_session_pnl = 0.0; asia_session_pnl = 0.0; london_session_pnl = 0.0; ny_session_pnl = 0.0;
   combinedCurrentDailyLoss = 0.0;
   dailyBestWin = 0.0;
   dailyBestLoss = 0.0;
   int dots_sync_wins=0;
   int dots_sync_losses=0;
   double dots_sync_pnl=0.0;
   if(Day() != currentDay) {
      currentDay = Day();
      Print("New trading day. Daily loss counter reset for EA #", EA_ID);
   }
   int estDayNow=TimeDay(GetEstTime());
   if(estDayNow!=dots_dailySummaryDay) {
      dots_dailySummaryDay=estDayNow;
      dots_dailySummaryPrinted=false;
      dots_today_sl=0;
      dots_today_feat=0;
      dots_today_time=0;
      dots_gross_win=0.0;
      dots_gross_loss=0.0;
   }
   for(int i = 0; i < OrdersHistoryTotal(); i++) {
      if(!OrderSelect(i, SELECT_BY_POS, MODE_HISTORY)) continue;
      if(OrderSymbol() != Symbol()) continue;
      int magic = OrderMagicNumber();
      datetime closeTime = OrderCloseTime();
      double pnl = OrderProfit() + OrderCommission() + OrderSwap();
      if(magic == MagicNumber) {
         if(closeTime < statsResetTime && statsResetTime != 0) continue;
         if(TimeDay(closeTime) == TimeDay(TimeCurrent())) {
            if(pnl > dailyBestWin) dailyBestWin = pnl;
            if(pnl < dailyBestLoss) dailyBestLoss = pnl;
         }
         if(pnl >= 0) { wins++; profitTotal += pnl; }
         else { losses++; lossTotal += MathAbs(pnl); }
         totalCommissions += OrderCommission();
         if(TimeDay(closeTime) == TimeDay(TimeCurrent())) {
            if(pnl < 0) currentDailyLoss += MathAbs(pnl);
         }
         if(pnl > 0 && OrderClosePrice() == OrderStopLoss()) {
            historicalProfitSecured += pnl;
         }
         lastClosedTradeProfit = pnl;
         lastClosedTradeClosePrice = OrderClosePrice();
         datetime gmtCloseTime = closeTime - (TimeCurrent() - TimeGMT());
         int gmtHour = TimeHour(gmtCloseTime);
         if(gmtHour >= 22 || gmtHour < 7) aus_session_pnl += pnl;
         if(gmtHour >= 0 && gmtHour < 9) asia_session_pnl += pnl;
         if(gmtHour >= 8 && gmtHour < 17) london_session_pnl += pnl;
         if(gmtHour >= 13 || gmtHour < 22) ny_session_pnl += pnl;
      }
      else if(magic == ManualMagicNumber) {
         if(closeTime < manual_statsResetTime && manual_statsResetTime != 0) continue;
         if(TimeDay(closeTime) == TimeDay(TimeCurrent())) {
            if(pnl > dailyBestWin) dailyBestWin = pnl;
            if(pnl < dailyBestLoss) dailyBestLoss = pnl;
         }
         if(pnl >= 0) { manual_wins++; manual_profitTotal += pnl; }
         else { manual_losses++; manual_lossTotal += MathAbs(pnl); }
         manual_totalCommissions += OrderCommission();
         if(TimeDay(closeTime) == TimeDay(TimeCurrent())) {
            if(pnl < 0) manual_currentDailyLoss += MathAbs(pnl);
         }
         manual_lastClosedTradeProfit = pnl;
         manual_lastClosedTradeClosePrice = OrderClosePrice();
      }
      else if(magic == OBVfriendMagicNumber) {
         if(closeTime < obvfriend_statsResetTime && obvfriend_statsResetTime != 0) continue;
         if(TimeDay(closeTime) == TimeDay(TimeCurrent())) {
            if(pnl > dailyBestWin) dailyBestWin = pnl;
            if(pnl < dailyBestLoss) dailyBestLoss = pnl;
         }
         if(pnl >= 0) { obvfriend_wins++; obvfriend_profitTotal += pnl; }
         else { obvfriend_losses++; obvfriend_lossTotal += MathAbs(pnl); }
         obvfriend_totalCommissions += OrderCommission();
         if(TimeDay(closeTime) == TimeDay(TimeCurrent())) {
            if(pnl < 0) obvfriend_currentDailyLoss += MathAbs(pnl);
         }
         obvfriend_lastClosedTradeProfit = pnl;
         obvfriend_lastClosedTradeClosePrice = OrderClosePrice();
      }
      else if(magic == DotsMagicNumber) {
         if(closeTime < dots_hist_statsResetTime && dots_hist_statsResetTime != 0) continue;
         if(TimeDay(closeTime) == TimeDay(TimeCurrent())) {
            if(pnl > dailyBestWin) dailyBestWin = pnl;
            if(pnl < dailyBestLoss) dailyBestLoss = pnl;
         }
         if(pnl >= 0) { dots_hist_wins++; dots_hist_profitTotal += pnl; }
         else { dots_hist_losses++; dots_hist_lossTotal += MathAbs(pnl); }
         dots_hist_totalCommissions += OrderCommission();
         if(TimeDay(closeTime) == TimeDay(TimeCurrent())) {
            if(pnl < 0) dots_hist_currentDailyLoss += MathAbs(pnl);
            if(pnl >= 0) dots_sync_wins++;
            else dots_sync_losses++;
            dots_sync_pnl += pnl;
         }
         dots_hist_lastClosedProfit = pnl;
         dots_hist_lastClosedPrice = OrderClosePrice();
         int ruleIdx=DotsParseRuleFromComment(OrderComment());
         if(ruleIdx>=0&&ruleIdx<DOTS_NUM_RULES) {
            if(pnl >= 0) dots_rule_wins[ruleIdx]++;
            else dots_rule_losses[ruleIdx]++;
            dots_rule_pnl[ruleIdx] += pnl;
         }
      }
   }
   combinedCurrentDailyLoss = currentDailyLoss + manual_currentDailyLoss + obvfriend_currentDailyLoss + dots_hist_currentDailyLoss;
   dots_today_wins=dots_sync_wins;
   dots_today_losses=dots_sync_losses;
   dots_today_pnl=dots_sync_pnl;
   if(g_isLoading) LogBootMessage("Stats: History Processed.");
}
void DrawControlPanel() {
   if(g_isLoading) LogBootMessage("UI: Building Panels...");
   const int Z_PANEL_BACKGROUND = 30000;
   const int Z_PANEL_FOREGROUND = 30001;
   const int Z_BTNS_BG = 30004;
   const int Z_BTNS_TXT = 30005;
   const int Z_TOP_BUTTON_BG = 30006;
   const int Z_TOP_BUTTON_TEXT = 30007;
   int panelX = 15, panelY = 15, panelW = 300, panelH = 910;
   int padding = 10, lineHeight = 20, col1_width = 140, header_y_adjust = 25;
   int manualPanelW = 205;
   int manualPanelX = panelX + panelW + padding;
   int topButtonGap = 5;
   color bg_color = C'19,29,42';
   color button_bg_color = C'65,71,83';
   color border_color = C'95,107,119';
   color text_color = C'96,95,113';
   color line_color = C'95,107,119';
   color bullish_color = C'146,134,124'; 
   color bearish_color = C'89,116,124'; 
   color header_color = C'146,134,124';
   color activate_looms_color = C'89,116,124';
   color looms_active_color = C'146,134,124';
   color lock_color = C'89,116,124';
   color locked_color = C'146,134,124';
   color reset_stats_color = C'96,95,113';
   color force_trade_color = C'146,134,124';
   int topButtonH = 20;
   string leftBtnNames[] = { "btnTogglePanel", "btnSessions", "btnTrends", "btnDump" };
   string leftBtnTexts[] = { "PANEL", "SESSIONS", "TRENDS", "DUMP" };
   bool leftBtnStates[4];
   leftBtnStates[0] = isPanelVisible;
   leftBtnStates[1] = isSessionVisualsVisible;
   leftBtnStates[2] = isTrendVisualsVisible;
   leftBtnStates[3] = false;
   int leftBtnW = (panelW - (topButtonGap * 3)) / 4;
   int currentX = panelX;
   for(int i = 0; i < 4; i++) {
      string bgName = ea_prefix + leftBtnNames[i] + "_bg";
      string textName = ea_prefix + leftBtnNames[i] + "_text";
      if(ObjectFind(0, bgName) < 0) {
         ObjectCreate(0, bgName, OBJ_RECTANGLE_LABEL, 0, 0, 0);
         ObjectSetInteger(0, bgName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
         ObjectSetInteger(0, bgName, OBJPROP_SELECTABLE, true);
      }
      int currentBtnW = leftBtnW;
      if(i == 3) currentBtnW = (panelX + panelW) - currentX;
      ObjectSetInteger(0, bgName, OBJPROP_BACK, false);
      ObjectSetInteger(0, bgName, OBJPROP_ZORDER, Z_TOP_BUTTON_BG);
      ObjectSetInteger(0, bgName, OBJPROP_XDISTANCE, currentX);
      ObjectSetInteger(0, bgName, OBJPROP_YDISTANCE, panelY);
      ObjectSetInteger(0, bgName, OBJPROP_XSIZE, currentBtnW);
      ObjectSetInteger(0, bgName, OBJPROP_YSIZE, topButtonH);
      ObjectSetInteger(0, bgName, OBJPROP_BGCOLOR, button_bg_color);
      ObjectSetInteger(0, bgName, OBJPROP_BORDER_COLOR, border_color);
      ObjectSetInteger(0, bgName, OBJPROP_BORDER_TYPE, 0);
      if(ObjectFind(0, textName) < 0) {
         ObjectCreate(0, textName, OBJ_LABEL, 0, 0, 0);
         ObjectSetInteger(0, textName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
         ObjectSetString(0, textName, OBJPROP_FONT, "Segoe UI Bold");
         ObjectSetInteger(0, textName, OBJPROP_FONTSIZE, 9);
         ObjectSetInteger(0, textName, OBJPROP_ANCHOR, ANCHOR_CENTER);
      }
      ObjectSetInteger(0, textName, OBJPROP_BACK, false);
      ObjectSetInteger(0, textName, OBJPROP_ZORDER, Z_TOP_BUTTON_TEXT);
      ObjectSetInteger(0, textName, OBJPROP_XDISTANCE, currentX + currentBtnW / 2);
      ObjectSetInteger(0, textName, OBJPROP_YDISTANCE, panelY + topButtonH / 2);
      ObjectSetString(0, textName, OBJPROP_TEXT, leftBtnTexts[i]);
      ObjectSetInteger(0, textName, OBJPROP_COLOR, leftBtnStates[i] ? looms_active_color : activate_looms_color);
      currentX += currentBtnW + topButtonGap;
   }
   string rightBtnNames[] = { "btnDots", "btnPoc", "btnOBV" };
   string rightBtnTexts[] = { "DOTS", "POC", "OBV" };
   bool rightBtnStates[3];
   rightBtnStates[0] = isSignalDotsVisible;
   rightBtnStates[1] = isPocVisualsVisible;
   rightBtnStates[2] = isOBVVisualsVisible;
   int rightBtnW = (manualPanelW - (topButtonGap * 2)) / 3;
   currentX = manualPanelX;
   for(int i = 0; i < 3; i++) {
      string bgName = ea_prefix + rightBtnNames[i] + "_bg";
      string textName = ea_prefix + rightBtnNames[i] + "_text";
      if(ObjectFind(0, bgName) < 0) {
         ObjectCreate(0, bgName, OBJ_RECTANGLE_LABEL, 0, 0, 0);
         ObjectSetInteger(0, bgName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
         ObjectSetInteger(0, bgName, OBJPROP_SELECTABLE, true);
      }
      ObjectSetInteger(0, bgName, OBJPROP_BACK, false);
      ObjectSetInteger(0, bgName, OBJPROP_ZORDER, Z_TOP_BUTTON_BG);
      ObjectSetInteger(0, bgName, OBJPROP_XDISTANCE, currentX);
      ObjectSetInteger(0, bgName, OBJPROP_YDISTANCE, panelY);
      ObjectSetInteger(0, bgName, OBJPROP_XSIZE, rightBtnW);
      ObjectSetInteger(0, bgName, OBJPROP_YSIZE, topButtonH);
      ObjectSetInteger(0, bgName, OBJPROP_BGCOLOR, button_bg_color);
      ObjectSetInteger(0, bgName, OBJPROP_BORDER_COLOR, border_color);
      ObjectSetInteger(0, bgName, OBJPROP_BORDER_TYPE, 0);
      if(ObjectFind(0, textName) < 0) {
         ObjectCreate(0, textName, OBJ_LABEL, 0, 0, 0);
         ObjectSetInteger(0, textName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
         ObjectSetString(0, textName, OBJPROP_FONT, "Segoe UI Bold");
         ObjectSetInteger(0, textName, OBJPROP_FONTSIZE, 9);
         ObjectSetInteger(0, textName, OBJPROP_ANCHOR, ANCHOR_CENTER);
      }
      ObjectSetInteger(0, textName, OBJPROP_BACK, false);
      ObjectSetInteger(0, textName, OBJPROP_ZORDER, Z_TOP_BUTTON_TEXT);
      ObjectSetInteger(0, textName, OBJPROP_XDISTANCE, currentX + rightBtnW / 2);
      ObjectSetInteger(0, textName, OBJPROP_YDISTANCE, panelY + topButtonH / 2);
      ObjectSetString(0, textName, OBJPROP_TEXT, rightBtnTexts[i]);
      ObjectSetInteger(0, textName, OBJPROP_COLOR, rightBtnStates[i] ? looms_active_color : activate_looms_color);
      currentX += rightBtnW + topButtonGap;
   }
   int quantPanelX = manualPanelX + manualPanelW + padding;
   int quantPanelW = 205;
   int qBtnW = (quantPanelW - topButtonGap) / 2;
   string btn1Name_bg = ea_prefix + "btnQuantToggle1_bg";
   string btn1Name_text = ea_prefix + "btnQuantToggle1_text";
   if(ObjectFind(0, btn1Name_bg) < 0) {
      ObjectCreate(0, btn1Name_bg, OBJ_RECTANGLE_LABEL, 0, 0, 0);
      ObjectSetInteger(0, btn1Name_bg, OBJPROP_CORNER, CORNER_LEFT_UPPER);
      ObjectSetInteger(0, btn1Name_bg, OBJPROP_SELECTABLE, true);
   }
   ObjectSetInteger(0, btn1Name_bg, OBJPROP_BACK, false);
   ObjectSetInteger(0, btn1Name_bg, OBJPROP_ZORDER, Z_TOP_BUTTON_BG);
   ObjectSetInteger(0, btn1Name_bg, OBJPROP_XDISTANCE, quantPanelX);
   ObjectSetInteger(0, btn1Name_bg, OBJPROP_YDISTANCE, panelY);
   ObjectSetInteger(0, btn1Name_bg, OBJPROP_XSIZE, qBtnW);
   ObjectSetInteger(0, btn1Name_bg, OBJPROP_YSIZE, topButtonH);
   ObjectSetInteger(0, btn1Name_bg, OBJPROP_BGCOLOR, button_bg_color);
   ObjectSetInteger(0, btn1Name_bg, OBJPROP_BORDER_COLOR, border_color);
   ObjectSetInteger(0, btn1Name_bg, OBJPROP_BORDER_TYPE, 0);
   if(ObjectFind(0, btn1Name_text) < 0) {
      ObjectCreate(0, btn1Name_text, OBJ_LABEL, 0, 0, 0);
      ObjectSetInteger(0, btn1Name_text, OBJPROP_CORNER, CORNER_LEFT_UPPER);
      ObjectSetString(0, btn1Name_text, OBJPROP_FONT, "Segoe UI Bold");
      ObjectSetInteger(0, btn1Name_text, OBJPROP_FONTSIZE, 9);
      ObjectSetInteger(0, btn1Name_text, OBJPROP_ANCHOR, ANCHOR_CENTER);
   }
   ObjectSetInteger(0, btn1Name_text, OBJPROP_BACK, false);
   ObjectSetInteger(0, btn1Name_text, OBJPROP_ZORDER, Z_TOP_BUTTON_TEXT);
   ObjectSetInteger(0, btn1Name_text, OBJPROP_XDISTANCE, quantPanelX + (qBtnW / 2));
   ObjectSetInteger(0, btn1Name_text, OBJPROP_YDISTANCE, panelY + (topButtonH / 2));
   ObjectSetString(0, btn1Name_text, OBJPROP_TEXT, showOBVfCandles ? "OBVf CANDLES" : "D2D CANDLES");
   ObjectSetInteger(0, btn1Name_text, OBJPROP_COLOR, showOBVfCandles ? looms_active_color : activate_looms_color);
   string btn2Name_bg = ea_prefix + "btnQuantToggle2_bg";
   string btn2Name_text = ea_prefix + "btnQuantToggle2_text";
   if(ObjectFind(0, btn2Name_bg) < 0) {
      ObjectCreate(0, btn2Name_bg, OBJ_RECTANGLE_LABEL, 0, 0, 0);
      ObjectSetInteger(0, btn2Name_bg, OBJPROP_CORNER, CORNER_LEFT_UPPER);
      ObjectSetInteger(0, btn2Name_bg, OBJPROP_SELECTABLE, true);
   }
   ObjectSetInteger(0, btn2Name_bg, OBJPROP_BACK, false);
   ObjectSetInteger(0, btn2Name_bg, OBJPROP_ZORDER, Z_TOP_BUTTON_BG);
   ObjectSetInteger(0, btn2Name_bg, OBJPROP_XDISTANCE, quantPanelX + qBtnW + topButtonGap);
   ObjectSetInteger(0, btn2Name_bg, OBJPROP_YDISTANCE, panelY);
   ObjectSetInteger(0, btn2Name_bg, OBJPROP_XSIZE, qBtnW);
   ObjectSetInteger(0, btn2Name_bg, OBJPROP_YSIZE, topButtonH);
   ObjectSetInteger(0, btn2Name_bg, OBJPROP_BGCOLOR, button_bg_color);
   ObjectSetInteger(0, btn2Name_bg, OBJPROP_BORDER_COLOR, border_color);
   ObjectSetInteger(0, btn2Name_bg, OBJPROP_BORDER_TYPE, 0);
   if(ObjectFind(0, btn2Name_text) < 0) {
      ObjectCreate(0, btn2Name_text, OBJ_LABEL, 0, 0, 0);
      ObjectSetInteger(0, btn2Name_text, OBJPROP_CORNER, CORNER_LEFT_UPPER);
      ObjectSetString(0, btn2Name_text, OBJPROP_FONT, "Segoe UI Bold");
      ObjectSetInteger(0, btn2Name_text, OBJPROP_FONTSIZE, 9);
      ObjectSetInteger(0, btn2Name_text, OBJPROP_ANCHOR, ANCHOR_CENTER);
   }
   ObjectSetInteger(0, btn2Name_text, OBJPROP_BACK, false);
   ObjectSetInteger(0, btn2Name_text, OBJPROP_ZORDER, Z_TOP_BUTTON_TEXT);
   ObjectSetInteger(0, btn2Name_text, OBJPROP_XDISTANCE, quantPanelX + qBtnW + topButtonGap + (qBtnW / 2));
   ObjectSetInteger(0, btn2Name_text, OBJPROP_YDISTANCE, panelY + (topButtonH / 2));
   ObjectSetString(0, btn2Name_text, OBJPROP_TEXT, isHarmonicVolVisible ? "HARM VOL ON" : "HARM VOL OFF");
   ObjectSetInteger(0, btn2Name_text, OBJPROP_COLOR, isHarmonicVolVisible ? looms_active_color : activate_looms_color);
   string btn8Name_bg = ea_prefix + "btnQuantToggle8_bg";
   string btn8Name_text = ea_prefix + "btnQuantToggle8_text";
   int btn8X = quantPanelX + 2 * (qBtnW + topButtonGap);
   int btn8W = 55;
   if(ObjectFind(0, btn8Name_bg) < 0) {
      ObjectCreate(0, btn8Name_bg, OBJ_RECTANGLE_LABEL, 0, 0, 0);
      ObjectSetInteger(0, btn8Name_bg, OBJPROP_CORNER, CORNER_LEFT_UPPER);
      ObjectSetInteger(0, btn8Name_bg, OBJPROP_SELECTABLE, true);
   }
   ObjectSetInteger(0, btn8Name_bg, OBJPROP_BACK, false);
   ObjectSetInteger(0, btn8Name_bg, OBJPROP_ZORDER, Z_TOP_BUTTON_BG);
   ObjectSetInteger(0, btn8Name_bg, OBJPROP_XDISTANCE, btn8X);
   ObjectSetInteger(0, btn8Name_bg, OBJPROP_YDISTANCE, panelY);
   ObjectSetInteger(0, btn8Name_bg, OBJPROP_XSIZE, btn8W);
   ObjectSetInteger(0, btn8Name_bg, OBJPROP_YSIZE, topButtonH);
   ObjectSetInteger(0, btn8Name_bg, OBJPROP_BGCOLOR, isDotsVisualsVisible ? C'44,56,72' : C'19,29,42');
   ObjectSetInteger(0, btn8Name_bg, OBJPROP_BORDER_COLOR, border_color);
   ObjectSetInteger(0, btn8Name_bg, OBJPROP_BORDER_TYPE, 0);
   if(ObjectFind(0, btn8Name_text) < 0) {
      ObjectCreate(0, btn8Name_text, OBJ_LABEL, 0, 0, 0);
      ObjectSetInteger(0, btn8Name_text, OBJPROP_CORNER, CORNER_LEFT_UPPER);
      ObjectSetString(0, btn8Name_text, OBJPROP_FONT, "Segoe UI Bold");
      ObjectSetInteger(0, btn8Name_text, OBJPROP_FONTSIZE, 9);
      ObjectSetInteger(0, btn8Name_text, OBJPROP_ANCHOR, ANCHOR_CENTER);
   }
   ObjectSetInteger(0, btn8Name_text, OBJPROP_BACK, false);
   ObjectSetInteger(0, btn8Name_text, OBJPROP_ZORDER, Z_TOP_BUTTON_TEXT);
   ObjectSetInteger(0, btn8Name_text, OBJPROP_XDISTANCE, btn8X + (btn8W / 2));
   ObjectSetInteger(0, btn8Name_text, OBJPROP_YDISTANCE, panelY + (topButtonH / 2));
   ObjectSetString(0, btn8Name_text, OBJPROP_TEXT, "DOTS");
   ObjectSetInteger(0, btn8Name_text, OBJPROP_COLOR, isDotsVisualsVisible ? looms_active_color : activate_looms_color);
   if(!isPanelVisible) {
      RemoveUIPanels();
      return;
   }
   string panelName = ea_prefix + "LoomsMainPanel";
   int panel_y_start = panelY + topButtonH + 5;
   if(ObjectFind(0, panelName) < 0) {
      ObjectCreate(0, panelName, OBJ_RECTANGLE_LABEL, 0, 0, 0);
      ObjectSetInteger(0, panelName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
   }
   ObjectSetInteger(0, panelName, OBJPROP_BACK, false);
   ObjectSetInteger(0, panelName, OBJPROP_ZORDER, Z_PANEL_BACKGROUND);
   ObjectSetInteger(0, panelName, OBJPROP_XDISTANCE, panelX);
   ObjectSetInteger(0, panelName, OBJPROP_YDISTANCE, panel_y_start);
   ObjectSetInteger(0, panelName, OBJPROP_XSIZE, panelW);
   ObjectSetInteger(0, panelName, OBJPROP_YSIZE, panelH);
   ObjectSetInteger(0, panelName, OBJPROP_BGCOLOR, bg_color);
   ObjectSetInteger(0, panelName, OBJPROP_BORDER_COLOR, border_color);
   ObjectSetInteger(0, panelName, OBJPROP_BORDER_TYPE, 0);
   int buttonH = 25;
   int titleH = 29;
   int title_y_start = panel_y_start + 5;
   string loomsTitleName = ea_prefix + "loomsTitle";
   string wgTitleName = ea_prefix + "wgTitle";
   int titleY = title_y_start + (titleH / 2);
   int centerX = panelX + (panelW / 2);
   bool showPrice = GetHeaderToggleState();
   if(ObjectFind(0, loomsTitleName) < 0) {
      ObjectCreate(0, loomsTitleName, OBJ_LABEL, 0, 0, 0);
      ObjectSetInteger(0, loomsTitleName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
      ObjectSetString(0, loomsTitleName, OBJPROP_FONT, "Segoe UI Bold");
      ObjectSetInteger(0, loomsTitleName, OBJPROP_FONTSIZE, 18);
      ObjectSetInteger(0, loomsTitleName, OBJPROP_ANCHOR, ANCHOR_CENTER);
   }
   ObjectSetInteger(0, loomsTitleName, OBJPROP_BACK, false);
   ObjectSetInteger(0, loomsTitleName, OBJPROP_ZORDER, Z_PANEL_FOREGROUND);
   ObjectSetInteger(0, loomsTitleName, OBJPROP_YDISTANCE, titleY);
   ObjectSetInteger(0, loomsTitleName, OBJPROP_XDISTANCE, centerX);
   if(showPrice) {
      ObjectSetString(0, loomsTitleName, OBJPROP_TEXT, DoubleToString(Bid, Digits));
      ObjectSetInteger(0, loomsTitleName, OBJPROP_COLOR, looms_active_color);
      ObjectDelete(0, wgTitleName);
   } else {
      ObjectSetString(0, loomsTitleName, OBJPROP_TEXT, "LOOMS WG");
      ObjectSetInteger(0, loomsTitleName, OBJPROP_COLOR, looms_active_color);
      ObjectDelete(0, wgTitleName);
   }
   int buttonW = panelW - (padding * 2);
   int button_y_start = title_y_start + titleH + 5;
   string buttonObjectNames[] = { "btnLooms_bg", "btnLock_bg", "btnForceTrade_bg", "btnResetStats_bg", "btnRePaint_bg", "btnST_bg", "btnTradeHistory_bg" };
   for(int i = 0; i < ArraySize(buttonObjectNames); i++) {
      string bgName = ea_prefix + buttonObjectNames[i];
      string textName = ea_prefix + StringSubstr(buttonObjectNames[i], 0, StringLen(buttonObjectNames[i]) - 3) + "_text";
      int current_y = button_y_start + (i * (buttonH + 5));
      if(ObjectFind(0, bgName) < 0) {
         ObjectCreate(0, bgName, OBJ_RECTANGLE_LABEL, 0, 0, 0);
         ObjectSetInteger(0, bgName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
         ObjectSetInteger(0, bgName, OBJPROP_SELECTABLE, true);
      }
      ObjectSetInteger(0, bgName, OBJPROP_BACK, false);
      ObjectSetInteger(0, bgName, OBJPROP_ZORDER, Z_BTNS_BG);
      ObjectSetInteger(0, bgName, OBJPROP_XDISTANCE, panelX + padding);
      ObjectSetInteger(0, bgName, OBJPROP_YDISTANCE, current_y);
      ObjectSetInteger(0, bgName, OBJPROP_XSIZE, buttonW);
      ObjectSetInteger(0, bgName, OBJPROP_YSIZE, buttonH);
      ObjectSetInteger(0, bgName, OBJPROP_BGCOLOR, button_bg_color);
      ObjectSetInteger(0, bgName, OBJPROP_BORDER_COLOR, border_color);
      ObjectSetInteger(0, bgName, OBJPROP_BORDER_TYPE, 0);
      if(ObjectFind(0, textName) < 0) {
         ObjectCreate(0, textName, OBJ_LABEL, 0, 0, 0);
         ObjectSetInteger(0, textName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
         ObjectSetString(0, textName, OBJPROP_FONT, "Segoe UI Bold");
         ObjectSetInteger(0, textName, OBJPROP_FONTSIZE, 10);
         ObjectSetInteger(0, textName, OBJPROP_ANCHOR, ANCHOR_CENTER);
      }
      ObjectSetInteger(0, textName, OBJPROP_BACK, false);
      ObjectSetInteger(0, textName, OBJPROP_ZORDER, Z_BTNS_TXT);
      ObjectSetInteger(0, textName, OBJPROP_XDISTANCE, panelX + (panelW / 2));
      ObjectSetInteger(0, textName, OBJPROP_YDISTANCE, current_y + (buttonH / 2));
      if(buttonObjectNames[i] == "btnLooms_bg") {
         ObjectSetString(0, textName, OBJPROP_TEXT, isLoomsActive ? "LOOMS ACTIVE" : "ACTIVATE LOOMS");
         ObjectSetInteger(0, textName, OBJPROP_COLOR, isLoomsActive ? looms_active_color : activate_looms_color);
      } else if(buttonObjectNames[i] == "btnLock_bg") {
         if(Panel_Lock_Security == LOCKS_DISABLED) {
            ObjectSetString(0, textName, OBJPROP_TEXT, "LOCK DISABLED");
            ObjectSetInteger(0, textName, OBJPROP_COLOR, text_color);
         } else {
            ObjectSetString(0, textName, OBJPROP_TEXT, isLocked ? "LOCKED" : "LOCK");
            ObjectSetInteger(0, textName, OBJPROP_COLOR, isLocked ? locked_color : lock_color);
         }
      } else if(buttonObjectNames[i] == "btnForceTrade_bg") {
         ObjectSetString(0, textName, OBJPROP_TEXT, "FORCE TRADE");
         ObjectSetInteger(0, textName, OBJPROP_COLOR, force_trade_color);
      } else if(buttonObjectNames[i] == "btnResetStats_bg") {
         ObjectSetString(0, textName, OBJPROP_TEXT, "RESET STATS");
         ObjectSetInteger(0, textName, OBJPROP_COLOR, reset_stats_color);
      } else if(buttonObjectNames[i] == "btnRePaint_bg") {
         ObjectSetString(0, textName, OBJPROP_TEXT, "RE-INITIALIZE");
         ObjectSetInteger(0, textName, OBJPROP_COLOR, reset_stats_color);
      } else if(buttonObjectNames[i] == "btnST_bg") {
         ObjectSetString(0, textName, OBJPROP_TEXT, isSuperTrendVisible ? "D2D ST VISIBLE" : "D2D ST INVISIBLE");
         ObjectSetInteger(0, textName, OBJPROP_COLOR, isSuperTrendVisible ? looms_active_color : activate_looms_color);
      } else if(buttonObjectNames[i] == "btnTradeHistory_bg") {
         ObjectSetString(0, textName, OBJPROP_TEXT, isTradeHistoryVisible ? "TRADE HISTORY UI ON" : "TRADE HISTORY UI OFF");
         ObjectSetInteger(0, textName, OBJPROP_COLOR, isTradeHistoryVisible ? looms_active_color : activate_looms_color);
      }
   }
   int stats_y_start = button_y_start + (ArraySize(buttonObjectNames) * (buttonH + 5)) + padding + 15;
   string statLabels[] = {
      "EA Status", "DD Status",
      "Wins", "Losses", "Total Profit", "Total Loss", "Commissions", "Daily Loss", "Last Close P/L",
      "Brain Posture", "OBV Alignment", "Volume State", "POC Status",
      "SL Locked-in-Profit", "SL Profit Secured",
      "Signal Position", "Signal Price", "Last Signal Time", "Bars Since Signal", "D2D Multiplier",
      "Spread", "Candle Ends In", "Open Times", "Current Time EST", "Daily Status"
   };
   string headers[] = { "STATUS", "PERFORMANCE", "DYNAMIC BRAIN", "STOP LOSS", "LIVE SIGNAL", "MARKET" };
   int header_indices[] = { 0, 2, 9, 13, 15, 20 };
   for(int i = 0; i < ArraySize(headers); i++) {
      string headerName = ea_prefix + "LoomsHeader" + IntegerToString(i);
      int y_pos = stats_y_start + (header_indices[i] * lineHeight) + (i * header_y_adjust);
      if(ObjectFind(0, headerName) < 0) {
         ObjectCreate(0, headerName, OBJ_LABEL, 0, 0, 0);
         ObjectSetInteger(0, headerName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
         ObjectSetString(0, headerName, OBJPROP_FONT, "Segoe UI Bold");
         ObjectSetInteger(0, headerName, OBJPROP_FONTSIZE, 9);
      }
      ObjectSetInteger(0, headerName, OBJPROP_BACK, false);
      ObjectSetInteger(0, headerName, OBJPROP_ZORDER, Z_PANEL_FOREGROUND);
      ObjectSetInteger(0, headerName, OBJPROP_XDISTANCE, panelX + padding);
      ObjectSetInteger(0, headerName, OBJPROP_YDISTANCE, y_pos - 15);
      ObjectSetString(0, headerName, OBJPROP_TEXT, headers[i]);
      ObjectSetInteger(0, headerName, OBJPROP_COLOR, header_color);
   }
   int header_offset_count = 0;
   for(int i = 0; i < ArraySize(statLabels); i++) {
      if(header_offset_count < ArraySize(header_indices) - 1 && i >= header_indices[header_offset_count + 1]) header_offset_count++;
      int current_y = stats_y_start + (i * lineHeight) + (header_offset_count * header_y_adjust);
      string labelName = ea_prefix + "LoomsStatLabel" + IntegerToString(i);
      string valueName = ea_prefix + "LoomsStatValue" + IntegerToString(i);
      string lineName = ea_prefix + "LoomsStatLine" + IntegerToString(i);
      if(ObjectFind(0, labelName) < 0) {
         ObjectCreate(0, labelName, OBJ_LABEL, 0, 0, 0);
         ObjectSetInteger(0, labelName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
         ObjectSetString(0, labelName, OBJPROP_FONT, "Segoe UI");
         ObjectSetInteger(0, labelName, OBJPROP_FONTSIZE, 10);
         ObjectSetInteger(0, labelName, OBJPROP_ANCHOR, ANCHOR_LEFT_UPPER);
      }
      ObjectSetInteger(0, labelName, OBJPROP_BACK, false);
      ObjectSetInteger(0, labelName, OBJPROP_ZORDER, Z_PANEL_FOREGROUND);
      ObjectSetInteger(0, labelName, OBJPROP_XDISTANCE, panelX + padding);
      ObjectSetInteger(0, labelName, OBJPROP_YDISTANCE, current_y);
      ObjectSetString(0, labelName, OBJPROP_TEXT, statLabels[i]);
      ObjectSetInteger(0, labelName, OBJPROP_COLOR, text_color);
      if(ObjectFind(0, valueName) < 0) {
         ObjectCreate(0, valueName, OBJ_LABEL, 0, 0, 0);
         ObjectSetInteger(0, valueName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
         ObjectSetString(0, valueName, OBJPROP_FONT, "Segoe UI");
         ObjectSetInteger(0, valueName, OBJPROP_FONTSIZE, 10);
         ObjectSetInteger(0, valueName, OBJPROP_ANCHOR, ANCHOR_LEFT_UPPER);
      }
      ObjectSetInteger(0, valueName, OBJPROP_BACK, false);
      ObjectSetInteger(0, valueName, OBJPROP_ZORDER, Z_PANEL_FOREGROUND);
      ObjectSetInteger(0, valueName, OBJPROP_XDISTANCE, panelX + col1_width);
      ObjectSetInteger(0, valueName, OBJPROP_YDISTANCE, current_y);
      if(ObjectFind(0, lineName) < 0) {
         ObjectCreate(0, lineName, OBJ_RECTANGLE_LABEL, 0, 0, 0);
         ObjectSetInteger(0, lineName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
         ObjectSetInteger(0, lineName, OBJPROP_BGCOLOR, line_color);
         ObjectSetInteger(0, lineName, OBJPROP_BORDER_COLOR, line_color);
      }
      ObjectSetInteger(0, lineName, OBJPROP_BACK, false);
      ObjectSetInteger(0, lineName, OBJPROP_ZORDER, Z_PANEL_BACKGROUND);
      ObjectSetInteger(0, lineName, OBJPROP_XDISTANCE, panelX + padding);
      ObjectSetInteger(0, lineName, OBJPROP_YDISTANCE, current_y + lineHeight - 4);
      ObjectSetInteger(0, lineName, OBJPROP_XSIZE, panelW - (padding * 2));
      ObjectSetInteger(0, lineName, OBJPROP_YSIZE, 1);
      ObjectSetInteger(0, lineName, OBJPROP_BORDER_TYPE, 0);
   }
   int val_idx = 0;
   bool isDDPaused = (combinedCurrentDailyLoss >= MaxDailyLoss && MaxDailyLoss > 0);
   ObjectSetString(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx), OBJPROP_TEXT, "ME " + IntegerToString(MagicNumber) + (isLoomsActive ? " Active" : " Inactive"));
   ObjectSetInteger(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx++), OBJPROP_COLOR, isLoomsActive ? bullish_color : bearish_color);
   ObjectSetString(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx), OBJPROP_TEXT, isDDPaused ? "Paused" : "Good to Go");
   ObjectSetInteger(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx++), OBJPROP_COLOR, isDDPaused ? bearish_color : bullish_color);
   ObjectSetString(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx), OBJPROP_TEXT, IntegerToString(wins));
   ObjectSetInteger(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx++), OBJPROP_COLOR, wins > 0 ? bullish_color : text_color);
   ObjectSetString(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx), OBJPROP_TEXT, IntegerToString(losses));
   ObjectSetInteger(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx++), OBJPROP_COLOR, losses > 0 ? bearish_color : text_color);
   ObjectSetString(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx), OBJPROP_TEXT, "$" + DoubleToString(profitTotal, 2));
   ObjectSetInteger(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx++), OBJPROP_COLOR, profitTotal > 0 ? bullish_color : text_color);
   ObjectSetString(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx), OBJPROP_TEXT, "$" + DoubleToString(lossTotal, 2));
   ObjectSetInteger(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx++), OBJPROP_COLOR, lossTotal > 0 ? bearish_color : text_color);
   ObjectSetString(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx), OBJPROP_TEXT, "$" + DoubleToString(MathAbs(totalCommissions), 2));
   ObjectSetInteger(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx++), OBJPROP_COLOR, totalCommissions != 0 ? bearish_color : text_color);
   ObjectSetString(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx), OBJPROP_TEXT, "$" + DoubleToString(currentDailyLoss, 2));
   ObjectSetInteger(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx++), OBJPROP_COLOR, currentDailyLoss > 0 ? bearish_color : text_color);
   string pnlString = (lastClosedTradeProfit >= 0 ? "+$" : "-$") + DoubleToString(MathAbs(lastClosedTradeProfit), 2) + " @ " + DoubleToString(lastClosedTradeClosePrice, Digits);
   if(lastClosedTradeClosePrice == 0) pnlString = "-";
   ObjectSetString(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx), OBJPROP_TEXT, pnlString);
   ObjectSetInteger(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx++), OBJPROP_COLOR, lastClosedTradeProfit >= 0 ? bullish_color : bearish_color);
   color postureColor = text_color;
   if(dy_Posture == "Relaxed") postureColor = bullish_color;
   else if(dy_Posture == "Tension" || dy_Posture == "Hyper-Sensitive") postureColor = bearish_color;
   else if(dy_Posture == "Defensive") postureColor = text_color;
   ObjectSetString(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx), OBJPROP_TEXT, dy_Posture);
   ObjectSetInteger(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx++), OBJPROP_COLOR, postureColor);
   color obvColor = text_color;
   if(dy_OBV_State == "Agree") obvColor = bullish_color;
   else if(dy_OBV_State == "Conflict") obvColor = bearish_color;
   ObjectSetString(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx), OBJPROP_TEXT, dy_OBV_State);
   ObjectSetInteger(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx++), OBJPROP_COLOR, obvColor);
   color volColor = text_color;
   if(dy_Vol_State == "High") volColor = bullish_color; 
   ObjectSetString(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx), OBJPROP_TEXT, dy_Vol_State);
   ObjectSetInteger(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx++), OBJPROP_COLOR, volColor);
   color pocColor = text_color;
   if(dy_POC_State == "In Zone") pocColor = bearish_color;
   else if(dy_POC_State == "Clear") pocColor = bullish_color;
   ObjectSetString(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx), OBJPROP_TEXT, dy_POC_State);
   ObjectSetInteger(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx++), OBJPROP_COLOR, pocColor);
   ObjectSetString(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx), OBJPROP_TEXT, "$" + DoubleToString(liveLockedInProfit, 2));
   ObjectSetInteger(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx++), OBJPROP_COLOR, liveLockedInProfit > 0 ? bullish_color : text_color);
   ObjectSetString(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx), OBJPROP_TEXT, "$" + DoubleToString(historicalProfitSecured, 2));
   ObjectSetInteger(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx++), OBJPROP_COLOR, historicalProfitSecured > 0 ? bullish_color : text_color);
   if(lastCommittedSignal != 0 && lastCommittedSignalIndex >= 0 && lastCommittedSignalIndex < Bars) {
      ObjectSetString(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx), OBJPROP_TEXT, (lastCommittedSignal == 1) ? "Long" : "Short");
      ObjectSetInteger(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx++), OBJPROP_COLOR, (lastCommittedSignal == 1) ? bullish_color : bearish_color);
      ObjectSetString(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx), OBJPROP_TEXT, DoubleToString(lastSignalPrice, Digits));
      ObjectSetInteger(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx++), OBJPROP_COLOR, text_color);
      ObjectSetString(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx), OBJPROP_TEXT, TimeToString(lastCommittedSignalTime, TIME_DATE | TIME_MINUTES));
      ObjectSetInteger(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx++), OBJPROP_COLOR, text_color);
      ObjectSetString(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx), OBJPROP_TEXT, IntegerToString(iBarShift(NULL, 0, lastCommittedSignalTime, false)));
      ObjectSetInteger(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx++), OBJPROP_COLOR, text_color);
   } else {
      for(int i = 0; i < 4; i++) {
         ObjectSetString(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx + i), OBJPROP_TEXT, "-");
         ObjectSetInteger(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx + i), OBJPROP_COLOR, text_color);
      }
      val_idx += 4;
   }
   string d2dMultiplierStr = DoubleToString(latest_d2d_dynamic_factor, 2);
   if(latest_d2d_dynamic_factor == 0.0) d2dMultiplierStr = "Calculating...";
   ObjectSetString(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx), OBJPROP_TEXT, d2dMultiplierStr);
   ObjectSetInteger(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx++), OBJPROP_COLOR, text_color);
   ObjectSetString(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx), OBJPROP_TEXT, "$" + DoubleToString(GetCurrentMarketSpreadUSD(BaseLotSize), 2));
   ObjectSetInteger(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx++), OBJPROP_COLOR, text_color);
   int timeframeSeconds = Period() * 60;
   int secondsSinceBarOpen = (int)(TimeCurrent() - Time[0]);
   int secondsRemaining = timeframeSeconds - secondsSinceBarOpen;
   if(secondsRemaining < 0) secondsRemaining = 0;
   int minutes = secondsRemaining / 60;
   int seconds = secondsRemaining % 60;
   string countdownText = StringFormat("%dm %02ds", minutes, seconds);
   ObjectSetString(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx), OBJPROP_TEXT, countdownText);
   ObjectSetInteger(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx++), OBJPROP_COLOR, text_color);
   string openStatus;
   color openColor;
   if(!UseOpenHours) {
      openStatus = "Not Used";
      openColor = text_color;
   } else {
      datetime estTime = GetEstTime();
      int currentHourEST = TimeHour(estTime);
      int currentMinuteEST = TimeMinute(estTime);
      int openTimeTotalMinutes = OpenHourEST * 60 + OpenMinuteEST;
      int closeTimeTotalMinutes = CloseHourEST * 60 + CloseMinuteEST;
      int currentTimeTotalMinutes = currentHourEST * 60 + currentMinuteEST;
      bool inSession;
      if(openTimeTotalMinutes > closeTimeTotalMinutes) inSession = (currentTimeTotalMinutes >= openTimeTotalMinutes || currentTimeTotalMinutes < closeTimeTotalMinutes);
      else inSession = (currentTimeTotalMinutes >= openTimeTotalMinutes && currentTimeTotalMinutes < closeTimeTotalMinutes);
      if(inSession) {
         openStatus = "Open";
         openColor = bullish_color;
      } else {
         openStatus = "Closed";
         openColor = bearish_color;
      }
   }
   ObjectSetString(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx), OBJPROP_TEXT, openStatus);
   ObjectSetInteger(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx++), OBJPROP_COLOR, openColor);
   datetime estTime = GetEstTime();
   string estTimeString = TimeToString(estTime, TIME_SECONDS);
   ObjectSetString(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx), OBJPROP_TEXT, estTimeString);
   ObjectSetInteger(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx++), OBJPROP_COLOR, text_color);
   int estDay = GetEstDayOfWeek(TimeGMT());
   string weekdayStatus = "";
   color weekdayColor = text_color;
   bool dayEnabled = false;
   string dayName = "";
   switch(estDay) {
      case 0: dayName = "Sunday"; break;
      case 1: dayName = "Monday"; dayEnabled = TradeOnMonday; break;
      case 2: dayName = "Tuesday"; dayEnabled = TradeOnTuesday; break;
      case 3: dayName = "Wednesday"; dayEnabled = TradeOnWednesday; break;
      case 4: dayName = "Thursday"; dayEnabled = TradeOnThursday; break;
      case 5: dayName = "Friday"; dayEnabled = TradeOnFriday; break;
      case 6: dayName = "Saturday"; break;
   }
   if(estDay == 0 || estDay == 6) {
      weekdayStatus = "Weekend";
      weekdayColor = text_color;
   }
   else {
      if(dayEnabled) { weekdayStatus = dayName + " Active"; weekdayColor = bullish_color; }
      else { weekdayStatus = dayName + " Inactive"; weekdayColor = bearish_color; }
   }
   ObjectSetString(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx), OBJPROP_TEXT, weekdayStatus);
   ObjectSetInteger(0, ea_prefix + "LoomsStatValue" + IntegerToString(val_idx++), OBJPROP_COLOR, weekdayColor);
   if(g_isLoading) LogBootMessage("UI: Stats Grid Populated.");
   int totalPerfPanelY = panel_y_start + panelH + padding;
   int totalPerfPanelH = 120;
   string totalPerfPanelName = ea_prefix + "TotalPerfPanel";
   if(ObjectFind(0, totalPerfPanelName) < 0) {
      ObjectCreate(0, totalPerfPanelName, OBJ_RECTANGLE_LABEL, 0, 0, 0);
      ObjectSetInteger(0, totalPerfPanelName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
   }
   ObjectSetInteger(0, totalPerfPanelName, OBJPROP_BACK, false);
   ObjectSetInteger(0, totalPerfPanelName, OBJPROP_ZORDER, Z_PANEL_BACKGROUND);
   ObjectSetInteger(0, totalPerfPanelName, OBJPROP_XDISTANCE, panelX);
   ObjectSetInteger(0, totalPerfPanelName, OBJPROP_YDISTANCE, totalPerfPanelY);
   ObjectSetInteger(0, totalPerfPanelName, OBJPROP_XSIZE, panelW);
   ObjectSetInteger(0, totalPerfPanelName, OBJPROP_YSIZE, totalPerfPanelH);
   ObjectSetInteger(0, totalPerfPanelName, OBJPROP_BGCOLOR, bg_color);
   ObjectSetInteger(0, totalPerfPanelName, OBJPROP_BORDER_COLOR, border_color);
   ObjectSetInteger(0, totalPerfPanelName, OBJPROP_BORDER_TYPE, 0);
   string totalHeaderName = ea_prefix + "TotalPerfHeader";
   if(ObjectFind(0, totalHeaderName) < 0) {
      ObjectCreate(0, totalHeaderName, OBJ_LABEL, 0, 0, 0);
      ObjectSetInteger(0, totalHeaderName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
      ObjectSetString(0, totalHeaderName, OBJPROP_FONT, "Segoe UI Bold");
      ObjectSetInteger(0, totalHeaderName, OBJPROP_FONTSIZE, 9);
   }
   ObjectSetInteger(0, totalHeaderName, OBJPROP_BACK, false);
   ObjectSetInteger(0, totalHeaderName, OBJPROP_ZORDER, Z_PANEL_FOREGROUND);
   ObjectSetInteger(0, totalHeaderName, OBJPROP_XDISTANCE, panelX + padding);
   ObjectSetInteger(0, totalHeaderName, OBJPROP_YDISTANCE, totalPerfPanelY + 10);
   ObjectSetString(0, totalHeaderName, OBJPROP_TEXT, "TOTAL PERFORMANCE");
   ObjectSetInteger(0, totalHeaderName, OBJPROP_COLOR, header_color);
   int total_wins = wins + manual_wins + obvfriend_wins;
   int total_losses = losses + manual_losses + obvfriend_losses;
   double total_profit = profitTotal + manual_profitTotal + obvfriend_profitTotal;
   double total_loss = lossTotal + manual_lossTotal + obvfriend_lossTotal;
   string tp_labels[8];
   tp_labels[0] = "Total Wins";
   tp_labels[1] = "Total Losses";
   tp_labels[2] = "Best Win";
   tp_labels[3] = "Win Rate";
   tp_labels[4] = "Total Profit";
   tp_labels[5] = "Total Loss";
   tp_labels[6] = "Best Loss";
   tp_labels[7] = "Profit Factor";
   string tp_values[8];
   tp_values[0] = IntegerToString(total_wins);
   tp_values[1] = IntegerToString(total_losses);
   tp_values[2] = (dailyBestWin > 0) ? "$" + DoubleToString(dailyBestWin, 2) : "$0.00";
   double win_rate = 0.0;
   if(total_wins + total_losses > 0) win_rate = (double)total_wins / (total_wins + total_losses) * 100.0;
   tp_values[3] = DoubleToString(win_rate, 2) + "%";
   tp_values[4] = "$" + DoubleToString(total_profit, 2);
   tp_values[5] = "$" + DoubleToString(total_loss, 2);
   tp_values[6] = (dailyBestLoss < 0) ? "$" + DoubleToString(dailyBestLoss, 2) : "$0.00";
   double profit_factor = 0.0;
   if(total_loss > 0) profit_factor = total_profit / total_loss;
   tp_values[7] = (total_profit > 0 && total_loss == 0) ? "Inf." : DoubleToString(profit_factor, 2);
   color tp_colors[8];
   tp_colors[0] = bullish_color;
   tp_colors[1] = bearish_color;
   tp_colors[2] = bullish_color;
   tp_colors[3] = bullish_color;
   tp_colors[4] = bullish_color;
   tp_colors[5] = bearish_color;
   tp_colors[6] = bearish_color;
   tp_colors[7] = bullish_color;
   int y_row1 = totalPerfPanelY + 30;
   int y_row2 = y_row1 + lineHeight;
   int y_row3 = y_row2 + lineHeight;
   int y_row4 = y_row3 + lineHeight;
   int x_lab1 = panelX + padding;
   int x_val1 = panelX + 85;
   int x_lab2 = panelX + 150;
   int x_val2 = panelX + 225;
   int x_coords_lab[8];
   x_coords_lab[0] = x_lab1; x_coords_lab[1] = x_lab1; x_coords_lab[2] = x_lab1; x_coords_lab[3] = x_lab1;
   x_coords_lab[4] = x_lab2; x_coords_lab[5] = x_lab2; x_coords_lab[6] = x_lab2; x_coords_lab[7] = x_lab2;
   int x_coords_val[8];
   x_coords_val[0] = x_val1; x_coords_val[1] = x_val1; x_coords_val[2] = x_val1; x_coords_val[3] = x_val1;
   x_coords_val[4] = x_val2; x_coords_val[5] = x_val2; x_coords_val[6] = x_val2; x_coords_val[7] = x_val2;
   int y_coords[8];
   y_coords[0] = y_row1; y_coords[1] = y_row2; y_coords[2] = y_row3; y_coords[3] = y_row4;
   y_coords[4] = y_row1; y_coords[5] = y_row2; y_coords[6] = y_row3; y_coords[7] = y_row4;
   for(int i = 0; i < 8; i++) {
      string labelName = ea_prefix + "TotalPerfLabel" + IntegerToString(i);
      string valueName = ea_prefix + "TotalPerfValue" + IntegerToString(i);
      if(ObjectFind(0, labelName) < 0) { ObjectCreate(0, labelName, OBJ_LABEL, 0, 0, 0); ObjectSetInteger(0, labelName, OBJPROP_CORNER, CORNER_LEFT_UPPER); ObjectSetString(0, labelName, OBJPROP_FONT, "Segoe UI"); ObjectSetInteger(0, labelName, OBJPROP_FONTSIZE, 10); }
      ObjectSetInteger(0, labelName, OBJPROP_XDISTANCE, x_coords_lab[i]);
      ObjectSetInteger(0, labelName, OBJPROP_YDISTANCE, y_coords[i]);
      ObjectSetString(0, labelName, OBJPROP_TEXT, tp_labels[i]);
      ObjectSetInteger(0, labelName, OBJPROP_COLOR, text_color);
      ObjectSetInteger(0, labelName, OBJPROP_ZORDER, Z_PANEL_FOREGROUND);
      if(ObjectFind(0, valueName) < 0) { ObjectCreate(0, valueName, OBJ_LABEL, 0, 0, 0); ObjectSetInteger(0, valueName, OBJPROP_CORNER, CORNER_LEFT_UPPER); ObjectSetString(0, valueName, OBJPROP_FONT, "Segoe UI"); ObjectSetInteger(0, valueName, OBJPROP_FONTSIZE, 10); }
      ObjectSetInteger(0, valueName, OBJPROP_XDISTANCE, x_coords_val[i]);
      ObjectSetInteger(0, valueName, OBJPROP_YDISTANCE, y_coords[i]);
      ObjectSetString(0, valueName, OBJPROP_TEXT, tp_values[i]);
      ObjectSetInteger(0, valueName, OBJPROP_COLOR, tp_colors[i]);
      ObjectSetInteger(0, valueName, OBJPROP_ZORDER, Z_PANEL_FOREGROUND);
   }
   string manualPanelName = ea_prefix + "ManualTradePanel";
   if(ObjectFind(0, manualPanelName) < 0) {
      ObjectCreate(0, manualPanelName, OBJ_RECTANGLE_LABEL, 0, 0, 0);
      ObjectSetInteger(0, manualPanelName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
   }
   ObjectSetInteger(0, manualPanelName, OBJPROP_BACK, false);
   ObjectSetInteger(0, manualPanelName, OBJPROP_ZORDER, Z_PANEL_BACKGROUND);
   ObjectSetInteger(0, manualPanelName, OBJPROP_XDISTANCE, manualPanelX);
   ObjectSetInteger(0, manualPanelName, OBJPROP_YDISTANCE, panel_y_start);
   ObjectSetInteger(0, manualPanelName, OBJPROP_XSIZE, manualPanelW);
   int manualPanelH = 390;
   ObjectSetInteger(0, manualPanelName, OBJPROP_YSIZE, manualPanelH);
   ObjectSetInteger(0, manualPanelName, OBJPROP_BGCOLOR, bg_color);
   ObjectSetInteger(0, manualPanelName, OBJPROP_BORDER_COLOR, border_color);
   ObjectSetInteger(0, manualPanelName, OBJPROP_BORDER_TYPE, 0);
   string mButtonNames[] = { "btnMTradeBuy1", "btnMTradeBuy2", "btnMTradeSell1", "btnMTradeSell2", "btnMLock", "btnMClose", "btnMResetStats" };
   string mButtonTexts[7];
   mButtonTexts[0] = "BUY " + DoubleToString(Manual_Buy_Lot_1, 2);
   mButtonTexts[1] = "BUY " + DoubleToString(Manual_Buy_Lot_2, 2);
   mButtonTexts[2] = "SELL " + DoubleToString(Manual_Sell_Lot_1, 2);
   mButtonTexts[3] = "SELL " + DoubleToString(Manual_Sell_Lot_2, 2);
   if(Panel_Lock_Security == LOCKS_DISABLED) mButtonTexts[4] = "LOCK DISABLED";
   else mButtonTexts[4] = isManualPanelLocked ? "LOCKED" : "UNLOCK";
   mButtonTexts[5] = "CLOSE ALL";
   mButtonTexts[6] = "RESET STATS";
   color mButtonColors[7];
   mButtonColors[0] = looms_active_color;
   mButtonColors[1] = looms_active_color;
   mButtonColors[2] = activate_looms_color;
   mButtonColors[3] = activate_looms_color;
   if(Panel_Lock_Security == LOCKS_DISABLED) mButtonColors[4] = text_color;
   else mButtonColors[4] = isManualPanelLocked ? locked_color : lock_color;
   mButtonColors[5] = text_color;
   mButtonColors[6] = text_color;
   int mButtonW = manualPanelW - (padding * 2);
   int mButtonH = 25;
   int mButtonY = panel_y_start + padding;
   int spaceBetween = 5;
   for(int i = 0; i < ArraySize(mButtonNames); i++) {
      string bgName = ea_prefix + mButtonNames[i] + "_bg";
      string textName = ea_prefix + mButtonNames[i] + "_text";
      if(ObjectFind(0, bgName) < 0) {
         ObjectCreate(0, bgName, OBJ_RECTANGLE_LABEL, 0, 0, 0);
         ObjectSetInteger(0, bgName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
         ObjectSetInteger(0, bgName, OBJPROP_SELECTABLE, true);
      }
      ObjectSetInteger(0, bgName, OBJPROP_BACK, false);
      ObjectSetInteger(0, bgName, OBJPROP_ZORDER, Z_BTNS_BG);
      ObjectSetInteger(0, bgName, OBJPROP_XDISTANCE, manualPanelX + padding);
      ObjectSetInteger(0, bgName, OBJPROP_YDISTANCE, mButtonY);
      ObjectSetInteger(0, bgName, OBJPROP_XSIZE, mButtonW);
      ObjectSetInteger(0, bgName, OBJPROP_YSIZE, mButtonH);
      ObjectSetInteger(0, bgName, OBJPROP_BGCOLOR, button_bg_color);
      ObjectSetInteger(0, bgName, OBJPROP_BORDER_COLOR, border_color);
      ObjectSetInteger(0, bgName, OBJPROP_BORDER_TYPE, 0);
      if(ObjectFind(0, textName) < 0) {
         ObjectCreate(0, textName, OBJ_LABEL, 0, 0, 0);
         ObjectSetInteger(0, textName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
         ObjectSetString(0, textName, OBJPROP_FONT, "Segoe UI Bold");
         ObjectSetInteger(0, textName, OBJPROP_FONTSIZE, 9);
         ObjectSetInteger(0, textName, OBJPROP_ANCHOR, ANCHOR_CENTER);
      }
      ObjectSetInteger(0, textName, OBJPROP_BACK, false);
      ObjectSetInteger(0, textName, OBJPROP_ZORDER, Z_BTNS_TXT);
      ObjectSetInteger(0, textName, OBJPROP_XDISTANCE, manualPanelX + manualPanelW / 2);
      ObjectSetInteger(0, textName, OBJPROP_YDISTANCE, mButtonY + mButtonH / 2);
      ObjectSetString(0, textName, OBJPROP_TEXT, mButtonTexts[i]);
      ObjectSetInteger(0, textName, OBJPROP_COLOR, mButtonColors[i]);
      mButtonY += mButtonH + spaceBetween;
   }
   if(g_isLoading) LogBootMessage("UI: Manual Panel Created.");
   mButtonY -= spaceBetween;
   mButtonY += 10;
   string mHeaderName = ea_prefix + "ManualHeader";
   if(ObjectFind(0, mHeaderName) < 0) {
      ObjectCreate(0, mHeaderName, OBJ_LABEL, 0, 0, 0);
      ObjectSetInteger(0, mHeaderName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
      ObjectSetString(0, mHeaderName, OBJPROP_FONT, "Segoe UI Bold");
      ObjectSetInteger(0, mHeaderName, OBJPROP_FONTSIZE, 9);
   }
   ObjectSetInteger(0, mHeaderName, OBJPROP_BACK, false);
   ObjectSetInteger(0, mHeaderName, OBJPROP_ZORDER, Z_PANEL_FOREGROUND);
   ObjectSetInteger(0, mHeaderName, OBJPROP_XDISTANCE, manualPanelX + padding);
   ObjectSetInteger(0, mHeaderName, OBJPROP_YDISTANCE, mButtonY);
   ObjectSetString(0, mHeaderName, OBJPROP_TEXT, "PERFORMANCE");
   ObjectSetInteger(0, mHeaderName, OBJPROP_COLOR, header_color);
   mButtonY += 5;
   string manualStatLabels[] = { "Wins", "Losses", "Total Profit", "Total Loss", "Commissions", "Daily Loss", "Last Close P/L" };
   int manual_col1_width = 100;
   int m_stats_y = mButtonY + 15;
   for(int i = 0; i < ArraySize(manualStatLabels); i++) {
      int current_y = m_stats_y + (i * lineHeight);
      string labelName = ea_prefix + "ManualStatLabel" + IntegerToString(i);
      string valueName = ea_prefix + "ManualStatValue" + IntegerToString(i);
      string lineName = ea_prefix + "ManualStatLine" + IntegerToString(i);
      if(ObjectFind(0, labelName) < 0) {
         ObjectCreate(0, labelName, OBJ_LABEL, 0, 0, 0);
         ObjectSetInteger(0, labelName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
         ObjectSetString(0, labelName, OBJPROP_FONT, "Segoe UI");
         ObjectSetInteger(0, labelName, OBJPROP_FONTSIZE, 10);
         ObjectSetInteger(0, labelName, OBJPROP_ANCHOR, ANCHOR_LEFT_UPPER);
      }
      ObjectSetInteger(0, labelName, OBJPROP_BACK, false);
      ObjectSetInteger(0, labelName, OBJPROP_ZORDER, Z_PANEL_FOREGROUND);
      ObjectSetInteger(0, labelName, OBJPROP_XDISTANCE, manualPanelX + padding);
      ObjectSetInteger(0, labelName, OBJPROP_YDISTANCE, current_y);
      ObjectSetString(0, labelName, OBJPROP_TEXT, manualStatLabels[i]);
      ObjectSetInteger(0, labelName, OBJPROP_COLOR, text_color);
      if(ObjectFind(0, valueName) < 0) {
         ObjectCreate(0, valueName, OBJ_LABEL, 0, 0, 0);
         ObjectSetInteger(0, valueName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
         ObjectSetString(0, valueName, OBJPROP_FONT, "Segoe UI");
         ObjectSetInteger(0, valueName, OBJPROP_FONTSIZE, 10);
         ObjectSetInteger(0, valueName, OBJPROP_ANCHOR, ANCHOR_LEFT_UPPER);
      }
      ObjectSetInteger(0, valueName, OBJPROP_BACK, false);
      ObjectSetInteger(0, valueName, OBJPROP_ZORDER, Z_PANEL_FOREGROUND);
      ObjectSetInteger(0, valueName, OBJPROP_XDISTANCE, manualPanelX + manual_col1_width);
      ObjectSetInteger(0, valueName, OBJPROP_YDISTANCE, current_y);
      if(ObjectFind(0, lineName) < 0) {
         ObjectCreate(0, lineName, OBJ_RECTANGLE_LABEL, 0, 0, 0);
         ObjectSetInteger(0, lineName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
         ObjectSetInteger(0, lineName, OBJPROP_BGCOLOR, line_color);
         ObjectSetInteger(0, lineName, OBJPROP_BORDER_COLOR, line_color);
      }
      ObjectSetInteger(0, lineName, OBJPROP_BACK, false);
      ObjectSetInteger(0, lineName, OBJPROP_ZORDER, Z_PANEL_BACKGROUND);
      ObjectSetInteger(0, lineName, OBJPROP_XDISTANCE, manualPanelX + padding);
      ObjectSetInteger(0, lineName, OBJPROP_YDISTANCE, current_y + lineHeight - 4);
      ObjectSetInteger(0, lineName, OBJPROP_XSIZE, manualPanelW - (padding * 2));
      ObjectSetInteger(0, lineName, OBJPROP_YSIZE, 1);
      ObjectSetInteger(0, lineName, OBJPROP_BORDER_TYPE, 0);
   }
   int m_val_idx = 0;
   ObjectSetString(0, ea_prefix + "ManualStatValue" + IntegerToString(m_val_idx), OBJPROP_TEXT, IntegerToString(manual_wins));
   ObjectSetInteger(0, ea_prefix + "ManualStatValue" + IntegerToString(m_val_idx++), OBJPROP_COLOR, manual_wins > 0 ? bullish_color : text_color);
   ObjectSetString(0, ea_prefix + "ManualStatValue" + IntegerToString(m_val_idx), OBJPROP_TEXT, IntegerToString(manual_losses));
   ObjectSetInteger(0, ea_prefix + "ManualStatValue" + IntegerToString(m_val_idx++), OBJPROP_COLOR, manual_losses > 0 ? bearish_color : text_color);
   ObjectSetString(0, ea_prefix + "ManualStatValue" + IntegerToString(m_val_idx), OBJPROP_TEXT, "$" + DoubleToString(manual_profitTotal, 2));
   ObjectSetInteger(0, ea_prefix + "ManualStatValue" + IntegerToString(m_val_idx++), OBJPROP_COLOR, manual_profitTotal > 0 ? bullish_color : text_color);
   ObjectSetString(0, ea_prefix + "ManualStatValue" + IntegerToString(m_val_idx), OBJPROP_TEXT, "$" + DoubleToString(manual_lossTotal, 2));
   ObjectSetInteger(0, ea_prefix + "ManualStatValue" + IntegerToString(m_val_idx++), OBJPROP_COLOR, manual_lossTotal > 0 ? bearish_color : text_color);
   ObjectSetString(0, ea_prefix + "ManualStatValue" + IntegerToString(m_val_idx), OBJPROP_TEXT, "$" + DoubleToString(MathAbs(manual_totalCommissions), 2));
   ObjectSetInteger(0, ea_prefix + "ManualStatValue" + IntegerToString(m_val_idx++), OBJPROP_COLOR, manual_totalCommissions != 0 ? bearish_color : text_color);
   ObjectSetString(0, ea_prefix + "ManualStatValue" + IntegerToString(m_val_idx), OBJPROP_TEXT, "$" + DoubleToString(manual_currentDailyLoss, 2));
   ObjectSetInteger(0, ea_prefix + "ManualStatValue" + IntegerToString(m_val_idx++), OBJPROP_COLOR, manual_currentDailyLoss > 0 ? bearish_color : text_color);
   string manualPnlString = (manual_lastClosedTradeProfit >= 0 ? "+$" : "-$") + DoubleToString(MathAbs(manual_lastClosedTradeProfit), 2);
   if(manual_lastClosedTradeClosePrice == 0) manualPnlString = "-";
   ObjectSetString(0, ea_prefix + "ManualStatValue" + IntegerToString(m_val_idx), OBJPROP_TEXT, manualPnlString);
   ObjectSetInteger(0, ea_prefix + "ManualStatValue" + IntegerToString(m_val_idx++), OBJPROP_COLOR, manual_lastClosedTradeProfit >= 0 ? bullish_color : bearish_color);
   int quantPanelY = panel_y_start; 
   int quantPanelH = 250;
   string quantPanelName = ea_prefix + "QuantDashboardPanel";
   if(ObjectFind(0, quantPanelName) < 0) {
      ObjectCreate(0, quantPanelName, OBJ_RECTANGLE_LABEL, 0, 0, 0);
      ObjectSetInteger(0, quantPanelName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
   }
   ObjectSetInteger(0, quantPanelName, OBJPROP_BACK, false);
   ObjectSetInteger(0, quantPanelName, OBJPROP_ZORDER, Z_PANEL_BACKGROUND);
   ObjectSetInteger(0, quantPanelName, OBJPROP_XDISTANCE, quantPanelX);
   ObjectSetInteger(0, quantPanelName, OBJPROP_YDISTANCE, quantPanelY);
   ObjectSetInteger(0, quantPanelName, OBJPROP_XSIZE, quantPanelW);
   ObjectSetInteger(0, quantPanelName, OBJPROP_YSIZE, quantPanelH);
   ObjectSetInteger(0, quantPanelName, OBJPROP_BGCOLOR, bg_color);
   ObjectSetInteger(0, quantPanelName, OBJPROP_BORDER_COLOR, border_color);
   ObjectSetInteger(0, quantPanelName, OBJPROP_BORDER_TYPE, 0);
   string qHeaderName = ea_prefix + "QuantHeader";
   if(ObjectFind(0, qHeaderName) < 0) {
      ObjectCreate(0, qHeaderName, OBJ_LABEL, 0, 0, 0);
      ObjectSetInteger(0, qHeaderName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
      ObjectSetString(0, qHeaderName, OBJPROP_FONT, "Segoe UI Bold");
      ObjectSetInteger(0, qHeaderName, OBJPROP_FONTSIZE, 9);
   }
   ObjectSetInteger(0, qHeaderName, OBJPROP_BACK, false);
   ObjectSetInteger(0, qHeaderName, OBJPROP_ZORDER, Z_PANEL_FOREGROUND);
   ObjectSetInteger(0, qHeaderName, OBJPROP_XDISTANCE, quantPanelX + padding);
   ObjectSetInteger(0, qHeaderName, OBJPROP_YDISTANCE, quantPanelY + 10);
   ObjectSetString(0, qHeaderName, OBJPROP_TEXT, "QUANT DASHBOARD");
   ObjectSetInteger(0, qHeaderName, OBJPROP_COLOR, header_color);
   int peakBarIndex=-1;
   double peakVol=GetRollingMaxVolume(3,1,peakBarIndex);
   double currentMultiplier=0.0;
   string sigStatus = "IDLE";
   color sigColor = text_color;
   if(UseVolumeMatrixMultiplier) {
      if(peakVol>=Tier12_Vol) currentMultiplier=Tier12_Mult;
      else if(peakVol>=Tier11_Vol) currentMultiplier=Tier11_Mult;
      else if(peakVol>=Tier10_Vol) currentMultiplier=Tier10_Mult;
      else if(peakVol>=Tier9_Vol) currentMultiplier=Tier9_Mult;
      else if(peakVol>=Tier8_Vol) currentMultiplier=Tier8_Mult;
      else if(peakVol>=Tier7_Vol) currentMultiplier=Tier7_Mult;
      else if(peakVol>=Tier6_Vol) currentMultiplier=Tier6_Mult;
      else if(peakVol>=Tier5_Vol) currentMultiplier=Tier5_Mult;
      else if(peakVol>=Tier4_Vol) currentMultiplier=Tier4_Mult;
      else if(peakVol>=Tier3_Vol) currentMultiplier=Tier3_Mult;
      else if(peakVol>=Tier2_Vol) currentMultiplier=Tier2_Mult;
      else if(peakVol>=Tier1_Vol) currentMultiplier=Tier1_Mult;
      if(LockBuffer[1] != 0) {
         if(peakVol >= Tier1_Vol) {
            sigStatus = "SNIPING";
            sigColor = (LockBuffer[1] == 1) ? bullish_color : bearish_color;
         } else {
            sigStatus = "BLOCKED";
            sigColor = bearish_color;
         }
      }
   } else {
      currentMultiplier=-1.0;
      if(LockBuffer[1] != 0) {
         sigStatus = "Matrix Disabled";
         sigColor = (LockBuffer[1] == 1) ? bullish_color : bearish_color;
      }
   }
   string qLabels[] = {"3-Bar Peak Vol", "Risk Multiplier", "Signal Status"};
   string qValues[3];
   qValues[0] = DoubleToString(peakVol, 0);
   if(currentMultiplier < 0.0) qValues[1] = "Disabled";
   else qValues[1] = DoubleToString(currentMultiplier, 2) + "x";
   qValues[2] = sigStatus;
   color qColors[3];
   qColors[0] = (peakVol >= Tier1_Vol) ? bullish_color : text_color;
   if(currentMultiplier < 0.0) qColors[1] = text_color;
   else qColors[1] = (currentMultiplier >= 1.0) ? bullish_color : ((currentMultiplier > 0) ? text_color : bearish_color);
   qColors[2] = sigColor;
   int q_stats_y = quantPanelY + 30;
   int q_col1_width = 100;
   for(int i = 0; i < ArraySize(qLabels); i++) {
      int current_y = q_stats_y + (i * lineHeight);
      string labelName = ea_prefix + "QuantStatLabel" + IntegerToString(i);
      string valueName = ea_prefix + "QuantStatValue" + IntegerToString(i);
      string lineName = ea_prefix + "QuantStatLine" + IntegerToString(i);
      if(ObjectFind(0, labelName) < 0) {
         ObjectCreate(0, labelName, OBJ_LABEL, 0, 0, 0);
         ObjectSetInteger(0, labelName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
         ObjectSetString(0, labelName, OBJPROP_FONT, "Segoe UI");
         ObjectSetInteger(0, labelName, OBJPROP_FONTSIZE, 10);
         ObjectSetInteger(0, labelName, OBJPROP_ANCHOR, ANCHOR_LEFT_UPPER);
      }
      ObjectSetInteger(0, labelName, OBJPROP_BACK, false);
      ObjectSetInteger(0, labelName, OBJPROP_ZORDER, Z_PANEL_FOREGROUND);
      ObjectSetInteger(0, labelName, OBJPROP_XDISTANCE, quantPanelX + padding);
      ObjectSetInteger(0, labelName, OBJPROP_YDISTANCE, current_y);
      ObjectSetString(0, labelName, OBJPROP_TEXT, qLabels[i]);
      ObjectSetInteger(0, labelName, OBJPROP_COLOR, text_color);
      if(ObjectFind(0, valueName) < 0) {
         ObjectCreate(0, valueName, OBJ_LABEL, 0, 0, 0);
         ObjectSetInteger(0, valueName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
         ObjectSetString(0, valueName, OBJPROP_FONT, "Segoe UI");
         ObjectSetInteger(0, valueName, OBJPROP_FONTSIZE, 10);
         ObjectSetInteger(0, valueName, OBJPROP_ANCHOR, ANCHOR_LEFT_UPPER);
      }
      ObjectSetInteger(0, valueName, OBJPROP_BACK, false);
      ObjectSetInteger(0, valueName, OBJPROP_ZORDER, Z_PANEL_FOREGROUND);
      ObjectSetInteger(0, valueName, OBJPROP_XDISTANCE, quantPanelX + q_col1_width);
      ObjectSetInteger(0, valueName, OBJPROP_YDISTANCE, current_y);
      ObjectSetString(0, valueName, OBJPROP_TEXT, qValues[i]);
      ObjectSetInteger(0, valueName, OBJPROP_COLOR, qColors[i]);
      if(ObjectFind(0, lineName) < 0) {
         ObjectCreate(0, lineName, OBJ_RECTANGLE_LABEL, 0, 0, 0);
         ObjectSetInteger(0, lineName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
         ObjectSetInteger(0, lineName, OBJPROP_BGCOLOR, line_color);
         ObjectSetInteger(0, lineName, OBJPROP_BORDER_COLOR, line_color);
      }
      ObjectSetInteger(0, lineName, OBJPROP_BACK, false);
      ObjectSetInteger(0, lineName, OBJPROP_ZORDER, Z_PANEL_BACKGROUND);
      ObjectSetInteger(0, lineName, OBJPROP_XDISTANCE, quantPanelX + padding);
      ObjectSetInteger(0, lineName, OBJPROP_YDISTANCE, current_y + lineHeight - 4);
      ObjectSetInteger(0, lineName, OBJPROP_XSIZE, quantPanelW - (padding * 2));
      ObjectSetInteger(0, lineName, OBJPROP_YSIZE, 1);
      ObjectSetInteger(0, lineName, OBJPROP_BORDER_TYPE, 0);
   }
   int qToggleBtnY = q_stats_y + (ArraySize(qLabels) * lineHeight) + 5;
   int qToggleBtnW = quantPanelW - (padding * 2);
   int qToggleBtnH = 25;
   string btn3Name_bg = ea_prefix + "btnQuantToggle3_bg";
   string btn3Name_text = ea_prefix + "btnQuantToggle3_text";
   if(ObjectFind(0, btn3Name_bg) < 0) {
      ObjectCreate(0, btn3Name_bg, OBJ_RECTANGLE_LABEL, 0, 0, 0);
      ObjectSetInteger(0, btn3Name_bg, OBJPROP_CORNER, CORNER_LEFT_UPPER);
      ObjectSetInteger(0, btn3Name_bg, OBJPROP_SELECTABLE, true);
   }
   ObjectSetInteger(0, btn3Name_bg, OBJPROP_BACK, false);
   ObjectSetInteger(0, btn3Name_bg, OBJPROP_ZORDER, Z_BTNS_BG);
   ObjectSetInteger(0, btn3Name_bg, OBJPROP_XDISTANCE, quantPanelX + padding);
   ObjectSetInteger(0, btn3Name_bg, OBJPROP_YDISTANCE, qToggleBtnY);
   ObjectSetInteger(0, btn3Name_bg, OBJPROP_XSIZE, qToggleBtnW);
   ObjectSetInteger(0, btn3Name_bg, OBJPROP_YSIZE, qToggleBtnH);
   ObjectSetInteger(0, btn3Name_bg, OBJPROP_BGCOLOR, button_bg_color);
   ObjectSetInteger(0, btn3Name_bg, OBJPROP_BORDER_COLOR, border_color);
   ObjectSetInteger(0, btn3Name_bg, OBJPROP_BORDER_TYPE, 0);
   if(ObjectFind(0, btn3Name_text) < 0) {
      ObjectCreate(0, btn3Name_text, OBJ_LABEL, 0, 0, 0);
      ObjectSetInteger(0, btn3Name_text, OBJPROP_CORNER, CORNER_LEFT_UPPER);
      ObjectSetString(0, btn3Name_text, OBJPROP_FONT, "Segoe UI Bold");
      ObjectSetInteger(0, btn3Name_text, OBJPROP_FONTSIZE, 9);
      ObjectSetInteger(0, btn3Name_text, OBJPROP_ANCHOR, ANCHOR_CENTER);
   }
   ObjectSetInteger(0, btn3Name_text, OBJPROP_BACK, false);
   ObjectSetInteger(0, btn3Name_text, OBJPROP_ZORDER, Z_BTNS_TXT);
   ObjectSetInteger(0, btn3Name_text, OBJPROP_XDISTANCE, quantPanelX + padding + (qToggleBtnW / 2));
   ObjectSetInteger(0, btn3Name_text, OBJPROP_YDISTANCE, qToggleBtnY + (qToggleBtnH / 2));
   ObjectSetString(0, btn3Name_text, OBJPROP_TEXT, isOBVfLineVisible ? "OBVf LINE ON" : "OBVf LINE OFF");
   ObjectSetInteger(0, btn3Name_text, OBJPROP_COLOR, isOBVfLineVisible ? looms_active_color : activate_looms_color);
   string btn4Name_bg = ea_prefix + "btnQuantToggle4_bg";
   string btn4Name_text = ea_prefix + "btnQuantToggle4_text";
   if(ObjectFind(0, btn4Name_bg) < 0) {
      ObjectCreate(0, btn4Name_bg, OBJ_RECTANGLE_LABEL, 0, 0, 0);
      ObjectSetInteger(0, btn4Name_bg, OBJPROP_CORNER, CORNER_LEFT_UPPER);
      ObjectSetInteger(0, btn4Name_bg, OBJPROP_SELECTABLE, true);
   }
   ObjectSetInteger(0, btn4Name_bg, OBJPROP_BACK, false);
   ObjectSetInteger(0, btn4Name_bg, OBJPROP_ZORDER, Z_BTNS_BG);
   ObjectSetInteger(0, btn4Name_bg, OBJPROP_XDISTANCE, quantPanelX + padding);
   ObjectSetInteger(0, btn4Name_bg, OBJPROP_YDISTANCE, qToggleBtnY + qToggleBtnH + topButtonGap);
   ObjectSetInteger(0, btn4Name_bg, OBJPROP_XSIZE, qToggleBtnW);
   ObjectSetInteger(0, btn4Name_bg, OBJPROP_YSIZE, qToggleBtnH);
   ObjectSetInteger(0, btn4Name_bg, OBJPROP_BGCOLOR, button_bg_color);
   ObjectSetInteger(0, btn4Name_bg, OBJPROP_BORDER_COLOR, border_color);
   ObjectSetInteger(0, btn4Name_bg, OBJPROP_BORDER_TYPE, 0);
   if(ObjectFind(0, btn4Name_text) < 0) {
      ObjectCreate(0, btn4Name_text, OBJ_LABEL, 0, 0, 0);
      ObjectSetInteger(0, btn4Name_text, OBJPROP_CORNER, CORNER_LEFT_UPPER);
      ObjectSetString(0, btn4Name_text, OBJPROP_FONT, "Segoe UI Bold");
      ObjectSetInteger(0, btn4Name_text, OBJPROP_FONTSIZE, 9);
      ObjectSetInteger(0, btn4Name_text, OBJPROP_ANCHOR, ANCHOR_CENTER);
   }
   ObjectSetInteger(0, btn4Name_text, OBJPROP_BACK, false);
   ObjectSetInteger(0, btn4Name_text, OBJPROP_ZORDER, Z_BTNS_TXT);
   ObjectSetInteger(0, btn4Name_text, OBJPROP_XDISTANCE, quantPanelX + padding + (qToggleBtnW / 2));
   ObjectSetInteger(0, btn4Name_text, OBJPROP_YDISTANCE, qToggleBtnY + qToggleBtnH + topButtonGap + (qToggleBtnH / 2));
   ObjectSetString(0, btn4Name_text, OBJPROP_TEXT, isPriceTrackerVisible ? "TRACKER ON" : "TRACKER OFF");
   ObjectSetInteger(0, btn4Name_text, OBJPROP_COLOR, isPriceTrackerVisible ? looms_active_color : activate_looms_color);
   string btn5Name_bg = ea_prefix + "btnQuantToggle5_bg";
   string btn5Name_text = ea_prefix + "btnQuantToggle5_text";
   if(ObjectFind(0, btn5Name_bg) < 0) {
      ObjectCreate(0, btn5Name_bg, OBJ_RECTANGLE_LABEL, 0, 0, 0);
      ObjectSetInteger(0, btn5Name_bg, OBJPROP_CORNER, CORNER_LEFT_UPPER);
      ObjectSetInteger(0, btn5Name_bg, OBJPROP_SELECTABLE, true);
   }
   ObjectSetInteger(0, btn5Name_bg, OBJPROP_BACK, false);
   ObjectSetInteger(0, btn5Name_bg, OBJPROP_ZORDER, Z_BTNS_BG);
   ObjectSetInteger(0, btn5Name_bg, OBJPROP_XDISTANCE, quantPanelX + padding);
   ObjectSetInteger(0, btn5Name_bg, OBJPROP_YDISTANCE, qToggleBtnY + (qToggleBtnH * 2) + (topButtonGap * 2));
   ObjectSetInteger(0, btn5Name_bg, OBJPROP_XSIZE, qToggleBtnW);
   ObjectSetInteger(0, btn5Name_bg, OBJPROP_YSIZE, qToggleBtnH);
   ObjectSetInteger(0, btn5Name_bg, OBJPROP_BGCOLOR, button_bg_color);
   ObjectSetInteger(0, btn5Name_bg, OBJPROP_BORDER_COLOR, border_color);
   ObjectSetInteger(0, btn5Name_bg, OBJPROP_BORDER_TYPE, 0);
   if(ObjectFind(0, btn5Name_text) < 0) {
      ObjectCreate(0, btn5Name_text, OBJ_LABEL, 0, 0, 0);
      ObjectSetInteger(0, btn5Name_text, OBJPROP_CORNER, CORNER_LEFT_UPPER);
      ObjectSetString(0, btn5Name_text, OBJPROP_FONT, "Segoe UI Bold");
      ObjectSetInteger(0, btn5Name_text, OBJPROP_FONTSIZE, 9);
      ObjectSetInteger(0, btn5Name_text, OBJPROP_ANCHOR, ANCHOR_CENTER);
   }
   ObjectSetInteger(0, btn5Name_text, OBJPROP_BACK, false);
   ObjectSetInteger(0, btn5Name_text, OBJPROP_ZORDER, Z_BTNS_TXT);
   ObjectSetInteger(0, btn5Name_text, OBJPROP_XDISTANCE, quantPanelX + padding + (qToggleBtnW / 2));
   ObjectSetInteger(0, btn5Name_text, OBJPROP_YDISTANCE, qToggleBtnY + (qToggleBtnH * 2) + (topButtonGap * 2) + (qToggleBtnH / 2));
   ObjectSetString(0, btn5Name_text, OBJPROP_TEXT, isKamaHistoVisible ? "KAMA HISTO ON" : "KAMA HISTO OFF");
   ObjectSetInteger(0, btn5Name_text, OBJPROP_COLOR, isKamaHistoVisible ? looms_active_color : activate_looms_color);
   string btn6Name_bg = ea_prefix + "btnQuantToggle6_bg";
   string btn6Name_text = ea_prefix + "btnQuantToggle6_text";
   if(ObjectFind(0, btn6Name_bg) < 0) {
      ObjectCreate(0, btn6Name_bg, OBJ_RECTANGLE_LABEL, 0, 0, 0);
      ObjectSetInteger(0, btn6Name_bg, OBJPROP_CORNER, CORNER_LEFT_UPPER);
      ObjectSetInteger(0, btn6Name_bg, OBJPROP_SELECTABLE, true);
   }
   ObjectSetInteger(0, btn6Name_bg, OBJPROP_BACK, false);
   ObjectSetInteger(0, btn6Name_bg, OBJPROP_ZORDER, Z_BTNS_BG);
   ObjectSetInteger(0, btn6Name_bg, OBJPROP_XDISTANCE, quantPanelX + padding);
   ObjectSetInteger(0, btn6Name_bg, OBJPROP_YDISTANCE, qToggleBtnY + (qToggleBtnH * 3) + (topButtonGap * 3));
   ObjectSetInteger(0, btn6Name_bg, OBJPROP_XSIZE, qToggleBtnW);
   ObjectSetInteger(0, btn6Name_bg, OBJPROP_YSIZE, qToggleBtnH);
   ObjectSetInteger(0, btn6Name_bg, OBJPROP_BGCOLOR, button_bg_color);
   ObjectSetInteger(0, btn6Name_bg, OBJPROP_BORDER_COLOR, border_color);
   ObjectSetInteger(0, btn6Name_bg, OBJPROP_BORDER_TYPE, 0);
   if(ObjectFind(0, btn6Name_text) < 0) {
      ObjectCreate(0, btn6Name_text, OBJ_LABEL, 0, 0, 0);
      ObjectSetInteger(0, btn6Name_text, OBJPROP_CORNER, CORNER_LEFT_UPPER);
      ObjectSetString(0, btn6Name_text, OBJPROP_FONT, "Segoe UI Bold");
      ObjectSetInteger(0, btn6Name_text, OBJPROP_FONTSIZE, 9);
      ObjectSetInteger(0, btn6Name_text, OBJPROP_ANCHOR, ANCHOR_CENTER);
   }
   ObjectSetInteger(0, btn6Name_text, OBJPROP_BACK, false);
   ObjectSetInteger(0, btn6Name_text, OBJPROP_ZORDER, Z_BTNS_TXT);
   ObjectSetInteger(0, btn6Name_text, OBJPROP_XDISTANCE, quantPanelX + padding + (qToggleBtnW / 2));
   ObjectSetInteger(0, btn6Name_text, OBJPROP_YDISTANCE, qToggleBtnY + (qToggleBtnH * 3) + (topButtonGap * 3) + (qToggleBtnH / 2));
   ObjectSetString(0, btn6Name_text, OBJPROP_TEXT, "SQUASH CHART");
   ObjectSetInteger(0, btn6Name_text, OBJPROP_COLOR, reset_stats_color);
   string btn7Name_bg = ea_prefix + "btnQuantToggle7_bg";
   string btn7Name_text = ea_prefix + "btnQuantToggle7_text";
   if(ObjectFind(0, btn7Name_bg) < 0) {
      ObjectCreate(0, btn7Name_bg, OBJ_RECTANGLE_LABEL, 0, 0, 0);
      ObjectSetInteger(0, btn7Name_bg, OBJPROP_CORNER, CORNER_LEFT_UPPER);
      ObjectSetInteger(0, btn7Name_bg, OBJPROP_SELECTABLE, true);
   }
   ObjectSetInteger(0, btn7Name_bg, OBJPROP_BACK, false);
   ObjectSetInteger(0, btn7Name_bg, OBJPROP_ZORDER, Z_BTNS_BG);
   ObjectSetInteger(0, btn7Name_bg, OBJPROP_XDISTANCE, quantPanelX + padding);
   ObjectSetInteger(0, btn7Name_bg, OBJPROP_YDISTANCE, qToggleBtnY + (qToggleBtnH * 4) + (topButtonGap * 4));
   ObjectSetInteger(0, btn7Name_bg, OBJPROP_XSIZE, qToggleBtnW);
   ObjectSetInteger(0, btn7Name_bg, OBJPROP_YSIZE, qToggleBtnH);
   ObjectSetInteger(0, btn7Name_bg, OBJPROP_BGCOLOR, button_bg_color);
   ObjectSetInteger(0, btn7Name_bg, OBJPROP_BORDER_COLOR, border_color);
   ObjectSetInteger(0, btn7Name_bg, OBJPROP_BORDER_TYPE, 0);
   if(ObjectFind(0, btn7Name_text) < 0) {
      ObjectCreate(0, btn7Name_text, OBJ_LABEL, 0, 0, 0);
      ObjectSetInteger(0, btn7Name_text, OBJPROP_CORNER, CORNER_LEFT_UPPER);
      ObjectSetString(0, btn7Name_text, OBJPROP_FONT, "Segoe UI Bold");
      ObjectSetInteger(0, btn7Name_text, OBJPROP_FONTSIZE, 9);
      ObjectSetInteger(0, btn7Name_text, OBJPROP_ANCHOR, ANCHOR_CENTER);
   }
   ObjectSetInteger(0, btn7Name_text, OBJPROP_BACK, false);
   ObjectSetInteger(0, btn7Name_text, OBJPROP_ZORDER, Z_BTNS_TXT);
   ObjectSetInteger(0, btn7Name_text, OBJPROP_XDISTANCE, quantPanelX + padding + (qToggleBtnW / 2));
   ObjectSetInteger(0, btn7Name_text, OBJPROP_YDISTANCE, qToggleBtnY + (qToggleBtnH * 4) + (topButtonGap * 4) + (qToggleBtnH / 2));
   ObjectSetString(0, btn7Name_text, OBJPROP_TEXT, isRangeOscVisible ? "RANGE OSC ON" : "RANGE OSC OFF");
   ObjectSetInteger(0, btn7Name_text, OBJPROP_COLOR, isRangeOscVisible ? looms_active_color : activate_looms_color);
   int sqzPanelX = quantPanelX;
   int sqzPanelW_loc = quantPanelW;
   int sqzPanelH = 200;
   int sqzPanelY = quantPanelY + quantPanelH + padding;
   string sqzPanelName = ea_prefix + "SqueezePanel_BG";
   if(ObjectFind(0, sqzPanelName) < 0) {
      ObjectCreate(0, sqzPanelName, OBJ_RECTANGLE_LABEL, 0, 0, 0);
      ObjectSetInteger(0, sqzPanelName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
   }
   ObjectSetInteger(0, sqzPanelName, OBJPROP_BACK, false);
   ObjectSetInteger(0, sqzPanelName, OBJPROP_ZORDER, Z_PANEL_BACKGROUND);
   ObjectSetInteger(0, sqzPanelName, OBJPROP_XDISTANCE, sqzPanelX);
   ObjectSetInteger(0, sqzPanelName, OBJPROP_YDISTANCE, sqzPanelY);
   ObjectSetInteger(0, sqzPanelName, OBJPROP_XSIZE, sqzPanelW_loc);
   ObjectSetInteger(0, sqzPanelName, OBJPROP_YSIZE, sqzPanelH);
   ObjectSetInteger(0, sqzPanelName, OBJPROP_BGCOLOR, bg_color);
   ObjectSetInteger(0, sqzPanelName, OBJPROP_BORDER_COLOR, border_color);
   ObjectSetInteger(0, sqzPanelName, OBJPROP_BORDER_TYPE, 0);
   string sqzHeaderName = ea_prefix + "SqzHeader";
   if(ObjectFind(0, sqzHeaderName) < 0) {
      ObjectCreate(0, sqzHeaderName, OBJ_LABEL, 0, 0, 0);
      ObjectSetInteger(0, sqzHeaderName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
      ObjectSetString(0, sqzHeaderName, OBJPROP_FONT, "Segoe UI Bold");
      ObjectSetInteger(0, sqzHeaderName, OBJPROP_FONTSIZE, 9);
   }
   ObjectSetInteger(0, sqzHeaderName, OBJPROP_BACK, false);
   ObjectSetInteger(0, sqzHeaderName, OBJPROP_ZORDER, Z_PANEL_FOREGROUND);
   ObjectSetInteger(0, sqzHeaderName, OBJPROP_XDISTANCE, sqzPanelX + padding);
   ObjectSetInteger(0, sqzHeaderName, OBJPROP_YDISTANCE, sqzPanelY + 10);
   ObjectSetString(0, sqzHeaderName, OBJPROP_TEXT, "VOLATILITY SQUEEZE");
   ObjectSetInteger(0, sqzHeaderName, OBJPROP_COLOR, header_color);
   int sqzStateLimit = ArraySize(state_Sqz_State);
   int sqzValLimit = ArraySize(state_Sqz_Val);
   int sqzStateVal = (1 < sqzStateLimit) ? state_Sqz_State[1] : 0;
   double sqzMomoVal = (1 < sqzValLimit) ? state_Sqz_Val[1] : 0.0;
   string sqzLabels[3];
   sqzLabels[0] = "State";
   sqzLabels[1] = "Momentum";
   sqzLabels[2] = "Action";
   string sqzValues[3];
   color sqzColors[3];
   if(sqzStateVal == 1) { sqzValues[0] = "Spring Load"; sqzColors[0] = clrOrange; }
   else if(sqzStateVal == -1) { sqzValues[0] = "Expansion"; sqzColors[0] = looms_active_color; }
   else { sqzValues[0] = "No Squeeze"; sqzColors[0] = text_color; }
   {
      double temp_max_abs = 0.0;
      int temp_check_end = (int)MathMin((double)Bars, 1000.0);
      for(int k = 1; k < temp_check_end; k++) {
         if(k < sqzValLimit && MathAbs(state_Sqz_Val[k]) > temp_max_abs) temp_max_abs = MathAbs(state_Sqz_Val[k]);
      }
      double temp_ratio = 0.0;
      if(temp_max_abs > 0.0) temp_ratio = sqzMomoVal / temp_max_abs;
      if(temp_ratio > 1.0) temp_ratio = 1.0;
      if(temp_ratio < -1.0) temp_ratio = -1.0;
      double temp_ratio_abs = MathAbs(temp_ratio);
      if(sqzMomoVal == 0.0) {
         sqzValues[1] = "Neutral";
         sqzColors[1] = text_color;
      } else {
         string temp_mag = "Weak";
         if(temp_ratio_abs >= 0.85) temp_mag = "Extreme";
         else if(temp_ratio_abs >= 0.60) temp_mag = "Strong";
         else if(temp_ratio_abs >= 0.25) temp_mag = "Average";
         string temp_dir = (sqzMomoVal > 0.0) ? " BULL" : " BEAR";
         sqzValues[1] = temp_mag + temp_dir;
         if(sqzMomoVal > 0.0) sqzColors[1] = bullish_color;
         else sqzColors[1] = bearish_color;
      }
   }
   if(sqzStateVal == 1) { sqzValues[2] = "Avoid Coil"; sqzColors[2] = text_color; }
   else if(sqzStateVal == -1 && sqzMomoVal > 0.0) { sqzValues[2] = "Engage Long"; sqzColors[2] = bullish_color; }
   else if(sqzStateVal == -1 && sqzMomoVal < 0.0) { sqzValues[2] = "Engage Short"; sqzColors[2] = bearish_color; }
   else { sqzValues[2] = "Chop / Idle"; sqzColors[2] = text_color; }
   int sqz_stats_y = sqzPanelY + 30;
   int sqz_col1_width = 100;
   for(int i = 0; i < 3; i++) {
      int current_y = sqz_stats_y + (i * lineHeight);
      string labelName = ea_prefix + "SqzStatLabel" + IntegerToString(i);
      string valueName = ea_prefix + "SqzStatValue" + IntegerToString(i);
      string lineName = ea_prefix + "SqzStatLine" + IntegerToString(i);
      if(ObjectFind(0, labelName) < 0) {
         ObjectCreate(0, labelName, OBJ_LABEL, 0, 0, 0);
         ObjectSetInteger(0, labelName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
         ObjectSetString(0, labelName, OBJPROP_FONT, "Segoe UI");
         ObjectSetInteger(0, labelName, OBJPROP_FONTSIZE, 10);
         ObjectSetInteger(0, labelName, OBJPROP_ANCHOR, ANCHOR_LEFT_UPPER);
      }
      ObjectSetInteger(0, labelName, OBJPROP_BACK, false);
      ObjectSetInteger(0, labelName, OBJPROP_ZORDER, Z_PANEL_FOREGROUND);
      ObjectSetInteger(0, labelName, OBJPROP_XDISTANCE, sqzPanelX + padding);
      ObjectSetInteger(0, labelName, OBJPROP_YDISTANCE, current_y);
      ObjectSetString(0, labelName, OBJPROP_TEXT, sqzLabels[i]);
      ObjectSetInteger(0, labelName, OBJPROP_COLOR, text_color);
      if(ObjectFind(0, valueName) < 0) {
         ObjectCreate(0, valueName, OBJ_LABEL, 0, 0, 0);
         ObjectSetInteger(0, valueName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
         ObjectSetString(0, valueName, OBJPROP_FONT, "Segoe UI");
         ObjectSetInteger(0, valueName, OBJPROP_FONTSIZE, 10);
         ObjectSetInteger(0, valueName, OBJPROP_ANCHOR, ANCHOR_LEFT_UPPER);
      }
      ObjectSetInteger(0, valueName, OBJPROP_BACK, false);
      ObjectSetInteger(0, valueName, OBJPROP_ZORDER, Z_PANEL_FOREGROUND);
      ObjectSetInteger(0, valueName, OBJPROP_XDISTANCE, sqzPanelX + sqz_col1_width);
      ObjectSetInteger(0, valueName, OBJPROP_YDISTANCE, current_y);
      ObjectSetString(0, valueName, OBJPROP_TEXT, sqzValues[i]);
      ObjectSetInteger(0, valueName, OBJPROP_COLOR, sqzColors[i]);
      if(ObjectFind(0, lineName) < 0) {
         ObjectCreate(0, lineName, OBJ_RECTANGLE_LABEL, 0, 0, 0);
         ObjectSetInteger(0, lineName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
         ObjectSetInteger(0, lineName, OBJPROP_BGCOLOR, line_color);
         ObjectSetInteger(0, lineName, OBJPROP_BORDER_COLOR, line_color);
      }
      ObjectSetInteger(0, lineName, OBJPROP_BACK, false);
      ObjectSetInteger(0, lineName, OBJPROP_ZORDER, Z_PANEL_BACKGROUND);
      ObjectSetInteger(0, lineName, OBJPROP_XDISTANCE, sqzPanelX + padding);
      ObjectSetInteger(0, lineName, OBJPROP_YDISTANCE, current_y + lineHeight - 4);
      ObjectSetInteger(0, lineName, OBJPROP_XSIZE, sqzPanelW_loc - (padding * 2));
      ObjectSetInteger(0, lineName, OBJPROP_YSIZE, 1);
      ObjectSetInteger(0, lineName, OBJPROP_BORDER_TYPE, 0);
   }
   int sqz_gauge_y = sqz_stats_y + (3 * lineHeight) + 5;
   string sqzGaugeHeader = ea_prefix + "SqzGaugeHeader";
   if(ObjectFind(0, sqzGaugeHeader) < 0) {
      ObjectCreate(0, sqzGaugeHeader, OBJ_LABEL, 0, 0, 0);
      ObjectSetInteger(0, sqzGaugeHeader, OBJPROP_CORNER, CORNER_LEFT_UPPER);
      ObjectSetString(0, sqzGaugeHeader, OBJPROP_FONT, "Segoe UI Bold");
      ObjectSetInteger(0, sqzGaugeHeader, OBJPROP_FONTSIZE, 8);
   }
   ObjectSetInteger(0, sqzGaugeHeader, OBJPROP_BACK, false);
   ObjectSetInteger(0, sqzGaugeHeader, OBJPROP_ZORDER, Z_PANEL_FOREGROUND);
   ObjectSetInteger(0, sqzGaugeHeader, OBJPROP_XDISTANCE, sqzPanelX + padding);
   ObjectSetInteger(0, sqzGaugeHeader, OBJPROP_YDISTANCE, sqz_gauge_y);
   ObjectSetString(0, sqzGaugeHeader, OBJPROP_TEXT, "SQUEEZE MOMENTUM");
   ObjectSetInteger(0, sqzGaugeHeader, OBJPROP_COLOR, C'96,95,113');
   int sqz_num_blocks = 13;
   double sqz_max_abs = 0.0;
   int sqz_lookback = 1000;
   int sqz_check_end = (int)MathMin((double)Bars, (double)sqz_lookback);
   for(int k = 1; k < sqz_check_end; k++) {
      if(k < sqzValLimit && MathAbs(state_Sqz_Val[k]) > sqz_max_abs) sqz_max_abs = MathAbs(state_Sqz_Val[k]);
   }
   double sqz_ratio = 0.0;
   if(sqz_max_abs > 0.0) sqz_ratio = sqzMomoVal / sqz_max_abs;
   if(sqz_ratio > 1.0) sqz_ratio = 1.0;
   if(sqz_ratio < -1.0) sqz_ratio = -1.0;
   int sqz_blockW = (sqzPanelW_loc - padding * 2 - (sqz_num_blocks - 1) * 1) / sqz_num_blocks;
   int sqz_blockY = sqz_gauge_y + 15;
   int sqz_blockH = 10;
   for(int s = 0; s < sqz_num_blocks; s++) {
      string blockName = ea_prefix + "sqz_gauge_b_" + IntegerToString(s);
      if(ObjectFind(0, blockName) < 0) {
         ObjectCreate(0, blockName, OBJ_RECTANGLE_LABEL, 0, 0, 0);
         ObjectSetInteger(0, blockName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
      }
      ObjectSetInteger(0, blockName, OBJPROP_XDISTANCE, sqzPanelX + padding + (s * (sqz_blockW + 1)));
      ObjectSetInteger(0, blockName, OBJPROP_YDISTANCE, sqz_blockY);
      ObjectSetInteger(0, blockName, OBJPROP_XSIZE, sqz_blockW);
      ObjectSetInteger(0, blockName, OBJPROP_YSIZE, sqz_blockH);
      ObjectSetInteger(0, blockName, OBJPROP_BACK, false);
      ObjectSetInteger(0, blockName, OBJPROP_ZORDER, Z_PANEL_FOREGROUND);
      color sqz_block_color = bg_color;
      if(s == 6) {
         sqz_block_color = text_color;
      } else if(sqz_ratio > 0.0 && s > 6) {
         double rel_pos = (double)(s - 6) / 6.0;
         if(rel_pos <= sqz_ratio) {
            if(sqzStateVal == 1) sqz_block_color = text_color;
            else if(sqzStateVal == -1) sqz_block_color = bullish_color;
            else sqz_block_color = bg_color;
         }
      } else if(sqz_ratio < 0.0 && s < 6) {
         double rel_pos = (double)(6 - s) / 6.0;
         if(rel_pos <= MathAbs(sqz_ratio)) {
            if(sqzStateVal == 1) sqz_block_color = text_color;
            else if(sqzStateVal == -1) sqz_block_color = bearish_color;
            else sqz_block_color = bg_color;
         }
      }
      ObjectSetInteger(0, blockName, OBJPROP_BGCOLOR, sqz_block_color);
      ObjectSetInteger(0, blockName, OBJPROP_BORDER_COLOR, border_color);
      ObjectSetInteger(0, blockName, OBJPROP_BORDER_TYPE, 0);
   }
   int rosc_header_y = sqz_blockY + sqz_blockH + 8;
   string roscHeaderName = ea_prefix + "SqzRoscHeader";
   if(ObjectFind(0, roscHeaderName) < 0) {
      ObjectCreate(0, roscHeaderName, OBJ_LABEL, 0, 0, 0);
      ObjectSetInteger(0, roscHeaderName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
      ObjectSetString(0, roscHeaderName, OBJPROP_FONT, "Segoe UI Bold");
      ObjectSetInteger(0, roscHeaderName, OBJPROP_FONTSIZE, 8);
   }
   ObjectSetInteger(0, roscHeaderName, OBJPROP_BACK, false);
   ObjectSetInteger(0, roscHeaderName, OBJPROP_ZORDER, Z_PANEL_FOREGROUND);
   ObjectSetInteger(0, roscHeaderName, OBJPROP_XDISTANCE, sqzPanelX + padding);
   ObjectSetInteger(0, roscHeaderName, OBJPROP_YDISTANCE, rosc_header_y);
   ObjectSetString(0, roscHeaderName, OBJPROP_TEXT, "RANGE OSCILLATOR");
   ObjectSetInteger(0, roscHeaderName, OBJPROP_COLOR, C'96,95,113');
   int rosc_stats_y = rosc_header_y + 18;
   int roscStateLimit = ArraySize(state_RangeOsc_State);
   int roscValLimit = ArraySize(state_RangeOsc_Val);
   int roscStateVal = (1 < roscStateLimit) ? state_RangeOsc_State[1] : 0;
   double roscOscVal = (1 < roscValLimit) ? state_RangeOsc_Val[1] : 0.0;
   string roscStateStr = "Neutral";
   color roscStateColor = text_color;
   if(roscStateVal == 1) { roscStateStr = "Strong Bull"; roscStateColor = clrOrange; }
   else if(roscStateVal == 2) { roscStateStr = "Weak Bull"; roscStateColor = bullish_color; }
   else if(roscStateVal == -1) { roscStateStr = "Strong Bear"; roscStateColor = clrDodgerBlue; }
   else if(roscStateVal == -2) { roscStateStr = "Weak Bear"; roscStateColor = bearish_color; }
   string roscOscStr = (roscOscVal >= 0.0 ? "+" : "") + DoubleToString(roscOscVal, 2);
   color roscOscColor = text_color;
   if(roscStateVal == 1) roscOscColor = clrOrange;
   else if(roscStateVal == 2) roscOscColor = bullish_color;
   else if(roscStateVal == -1) roscOscColor = clrDodgerBlue;
   else if(roscStateVal == -2) roscOscColor = bearish_color;
   string roscLabels[2];
   roscLabels[0] = "State";
   roscLabels[1] = "Oscillator";
   string roscValues[2];
   roscValues[0] = roscStateStr;
   roscValues[1] = roscOscStr;
   color roscColors[2];
   roscColors[0] = roscStateColor;
   roscColors[1] = roscOscColor;
   for(int i = 0; i < 2; i++) {
      int current_y = rosc_stats_y + (i * lineHeight);
      string labelName = ea_prefix + "SqzRoscLabel" + IntegerToString(i);
      string valueName = ea_prefix + "SqzRoscValue" + IntegerToString(i);
      string lineName = ea_prefix + "SqzRoscLine" + IntegerToString(i);
      if(ObjectFind(0, labelName) < 0) {
         ObjectCreate(0, labelName, OBJ_LABEL, 0, 0, 0);
         ObjectSetInteger(0, labelName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
         ObjectSetString(0, labelName, OBJPROP_FONT, "Segoe UI");
         ObjectSetInteger(0, labelName, OBJPROP_FONTSIZE, 10);
         ObjectSetInteger(0, labelName, OBJPROP_ANCHOR, ANCHOR_LEFT_UPPER);
      }
      ObjectSetInteger(0, labelName, OBJPROP_BACK, false);
      ObjectSetInteger(0, labelName, OBJPROP_ZORDER, Z_PANEL_FOREGROUND);
      ObjectSetInteger(0, labelName, OBJPROP_XDISTANCE, sqzPanelX + padding);
      ObjectSetInteger(0, labelName, OBJPROP_YDISTANCE, current_y);
      ObjectSetString(0, labelName, OBJPROP_TEXT, roscLabels[i]);
      ObjectSetInteger(0, labelName, OBJPROP_COLOR, text_color);
      if(ObjectFind(0, valueName) < 0) {
         ObjectCreate(0, valueName, OBJ_LABEL, 0, 0, 0);
         ObjectSetInteger(0, valueName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
         ObjectSetString(0, valueName, OBJPROP_FONT, "Segoe UI");
         ObjectSetInteger(0, valueName, OBJPROP_FONTSIZE, 10);
         ObjectSetInteger(0, valueName, OBJPROP_ANCHOR, ANCHOR_LEFT_UPPER);
      }
      ObjectSetInteger(0, valueName, OBJPROP_BACK, false);
      ObjectSetInteger(0, valueName, OBJPROP_ZORDER, Z_PANEL_FOREGROUND);
      ObjectSetInteger(0, valueName, OBJPROP_XDISTANCE, sqzPanelX + sqz_col1_width);
      ObjectSetInteger(0, valueName, OBJPROP_YDISTANCE, current_y);
      ObjectSetString(0, valueName, OBJPROP_TEXT, roscValues[i]);
      ObjectSetInteger(0, valueName, OBJPROP_COLOR, roscColors[i]);
      if(ObjectFind(0, lineName) < 0) {
         ObjectCreate(0, lineName, OBJ_RECTANGLE_LABEL, 0, 0, 0);
         ObjectSetInteger(0, lineName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
         ObjectSetInteger(0, lineName, OBJPROP_BGCOLOR, line_color);
         ObjectSetInteger(0, lineName, OBJPROP_BORDER_COLOR, line_color);
      }
      ObjectSetInteger(0, lineName, OBJPROP_BACK, false);
      ObjectSetInteger(0, lineName, OBJPROP_ZORDER, Z_PANEL_BACKGROUND);
      ObjectSetInteger(0, lineName, OBJPROP_XDISTANCE, sqzPanelX + padding);
      ObjectSetInteger(0, lineName, OBJPROP_YDISTANCE, current_y + lineHeight - 4);
      ObjectSetInteger(0, lineName, OBJPROP_XSIZE, sqzPanelW_loc - (padding * 2));
      ObjectSetInteger(0, lineName, OBJPROP_YSIZE, 1);
      ObjectSetInteger(0, lineName, OBJPROP_BORDER_TYPE, 0);
   }
   int trendPanelX = manualPanelX;
   int trendPanelW = manualPanelW;
   int trendPanelH = 300;
   int trendPanelY = panel_y_start + manualPanelH + padding;
   string trendPanelName = ea_prefix + "TrendStrengthPanel";
   if(ObjectFind(0, trendPanelName) < 0) {
      ObjectCreate(0, trendPanelName, OBJ_RECTANGLE_LABEL, 0, 0, 0);
      ObjectSetInteger(0, trendPanelName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
   }
   ObjectSetInteger(0, trendPanelName, OBJPROP_BACK, false);
   ObjectSetInteger(0, trendPanelName, OBJPROP_ZORDER, Z_PANEL_BACKGROUND);
   ObjectSetInteger(0, trendPanelName, OBJPROP_XDISTANCE, trendPanelX);
   ObjectSetInteger(0, trendPanelName, OBJPROP_YDISTANCE, trendPanelY);
   ObjectSetInteger(0, trendPanelName, OBJPROP_XSIZE, trendPanelW);
   ObjectSetInteger(0, trendPanelName, OBJPROP_YSIZE, trendPanelH);
   ObjectSetInteger(0, trendPanelName, OBJPROP_BGCOLOR, bg_color);
   ObjectSetInteger(0, trendPanelName, OBJPROP_BORDER_COLOR, border_color);
   ObjectSetInteger(0, trendPanelName, OBJPROP_BORDER_TYPE, 0);
   string tHeaderName = ea_prefix + "TrendHeader";
   if(ObjectFind(0, tHeaderName) < 0) {
      ObjectCreate(0, tHeaderName, OBJ_LABEL, 0, 0, 0);
      ObjectSetInteger(0, tHeaderName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
      ObjectSetString(0, tHeaderName, OBJPROP_FONT, "Segoe UI Bold");
      ObjectSetInteger(0, tHeaderName, OBJPROP_FONTSIZE, 9);
   }
   ObjectSetInteger(0, tHeaderName, OBJPROP_BACK, false);
   ObjectSetInteger(0, tHeaderName, OBJPROP_ZORDER, Z_PANEL_FOREGROUND);
   ObjectSetInteger(0, tHeaderName, OBJPROP_XDISTANCE, trendPanelX + padding);
   ObjectSetInteger(0, tHeaderName, OBJPROP_YDISTANCE, trendPanelY + 7);
   ObjectSetString(0, tHeaderName, OBJPROP_TEXT, "TREND STRENGTH");
   ObjectSetInteger(0, tHeaderName, OBJPROP_COLOR, header_color);
   int gaugeBlockH = 10;
   int gaugeGap = 5;
   int startY = trendPanelY + 26;
   string gaugeHeaders[] = { "MOMENTUM", "INSTITUTIONAL PROXIMITY", "ADX", "STRENGTH", "OBV ZERO POINT" };
   int currentGaugeY = startY;
   for(int g = 0; g < 5; g++) {
      string gHeaderName = ea_prefix + "GaugeHeader_" + IntegerToString(g);
      if(ObjectFind(0, gHeaderName) < 0) {
         ObjectCreate(0, gHeaderName, OBJ_LABEL, 0, 0, 0);
         ObjectSetInteger(0, gHeaderName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
         ObjectSetString(0, gHeaderName, OBJPROP_FONT, "Segoe UI Bold");
         ObjectSetInteger(0, gHeaderName, OBJPROP_FONTSIZE, 8);
      }
      ObjectSetInteger(0, gHeaderName, OBJPROP_BACK, false);
      ObjectSetInteger(0, gHeaderName, OBJPROP_ZORDER, Z_PANEL_FOREGROUND);
      ObjectSetInteger(0, gHeaderName, OBJPROP_XDISTANCE, trendPanelX + padding);
      ObjectSetInteger(0, gHeaderName, OBJPROP_YDISTANCE, currentGaugeY);
      ObjectSetString(0, gHeaderName, OBJPROP_TEXT, gaugeHeaders[g]);
      ObjectSetInteger(0, gHeaderName, OBJPROP_COLOR, C'96,95,113');
      int num_blocks = 30;
      int active_block = -1;
      double obv_ratio = 0.0;
      if(g >= 3) num_blocks = 13;
      if(g == 0) {
         int momoLookback = 1000;
         if(Bars < momoLookback) momoLookback = Bars;
         double momo_min = 0.0; double momo_max = 0.0;
         if(momoLookback > 2) {
            momo_min = MomentumBuffer[ArrayMinimum(MomentumBuffer, momoLookback - 1, 1)];
            momo_max = MomentumBuffer[ArrayMaximum(MomentumBuffer, momoLookback - 1, 1)];
         }
         double momo_range = momo_max - momo_min;
         double current_momo_norm = 0.0;
         if(momo_range > 0) current_momo_norm = (latest_momentum_value - momo_min) / momo_range;
         if(current_momo_norm < 0) current_momo_norm = 0; if(current_momo_norm > 1) current_momo_norm = 1;
         active_block = (int)MathRound(current_momo_norm * (num_blocks - 1));
      }
      else if(g == 1) {
         double current_vol_norm = 0.0;
         if(Tier12_Vol > 0) current_vol_norm = hist_VolumeValue[1] / Tier12_Vol;
         if(current_vol_norm < 0) current_vol_norm = 0; if(current_vol_norm > 1.0) current_vol_norm = 1.0;
         active_block = (int)MathRound(current_vol_norm * (num_blocks - 1));
      }
      else if(g == 2) {
         double adx_range = adx_max_historical - adx_min_historical;
         double current_adx_norm = 0.0;
         if(adx_range > 0) current_adx_norm = (ADXBuffer[1] - adx_min_historical) / adx_range;
         if(current_adx_norm < 0) current_adx_norm = 0; if(current_adx_norm > 1) current_adx_norm = 1;
         active_block = (int)MathRound(current_adx_norm * (num_blocks - 1));
      }
      else if(g == 3) {
         int trendStep = 7;
         if(Bars > 1) trendStep = hist_trendStep_ST[1];
         if(trendStep < 1) trendStep = 1; if(trendStep > 13) trendStep = 13;
         active_block = trendStep - 1;
      }
      else if(g == 4) {
         double current_obv = state_OBV_Final[1];
         int obv_lookback = 1000;
         double max_obv_abs = 0.0;
         int check_start = 1;
         int check_end = (int)MathMin((double)Bars, (double)obv_lookback);
         for(int k = check_start; k < check_end; k++) {
            if(MathAbs(state_OBV_Final[k]) > max_obv_abs) max_obv_abs = MathAbs(state_OBV_Final[k]);
         }
         if(max_obv_abs > 0) obv_ratio = current_obv / max_obv_abs;
         if(obv_ratio > 1.0) obv_ratio = 1.0; if(obv_ratio < -1.0) obv_ratio = -1.0;
         active_block = -1;
      }
      int blockW = (trendPanelW - padding * 2 - (num_blocks - 1) * 1) / num_blocks;
      int blockY = currentGaugeY + 15;
      for(int s = 0; s < num_blocks; s++) {
         string blockName = ea_prefix + "gauge_b_" + IntegerToString(g) + "_" + IntegerToString(s);
         if(ObjectFind(0, blockName) < 0) {
            ObjectCreate(0, blockName, OBJ_RECTANGLE_LABEL, 0, 0, 0);
            ObjectSetInteger(0, blockName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
         }
         ObjectSetInteger(0, blockName, OBJPROP_XDISTANCE, trendPanelX + padding + (s * (blockW + 1)));
         ObjectSetInteger(0, blockName, OBJPROP_YDISTANCE, blockY);
         ObjectSetInteger(0, blockName, OBJPROP_XSIZE, blockW);
         ObjectSetInteger(0, blockName, OBJPROP_YSIZE, gaugeBlockH);
         ObjectSetInteger(0, blockName, OBJPROP_BACK, false);
         ObjectSetInteger(0, blockName, OBJPROP_ZORDER, Z_PANEL_FOREGROUND);
         color block_color = bg_color;
         if(g == 4) {
            if(s == 6) {
               block_color = text_color;
            } else if(obv_ratio > 0 && s > 6) {
               double rel_pos = (s - 6) / 6.0;
               if(rel_pos <= obv_ratio) block_color = C'255,165,0';
            } else if(obv_ratio < 0 && s < 6) {
               double rel_pos = (6.0 - s) / 6.0;
               if(rel_pos <= MathAbs(obv_ratio)) block_color = C'30,144,255';
            }
         } else {
            bool isActive = false;
            if(g >= 3) isActive = (s == active_block);
            else isActive = (s <= active_block);
            if(isActive) {
               double norm_s = (double)s / (double)(num_blocks - 1);
               int r = (int)MathRound(30.0 * (1.0 - norm_s) + 255.0 * norm_s);
               int g_val = (int)MathRound(144.0 * (1.0 - norm_s) + 165.0 * norm_s);
               int b = (int)MathRound(255.0 * (1.0 - norm_s) + 0.0 * norm_s);
               if(r > 255) r = 255; if(r < 0) r = 0;
               if(g_val > 255) g_val = 255; if(g_val < 0) g_val = 0;
               if(b > 255) b = 255; if(b < 0) b = 0;
               block_color = (color)((b << 16) | (g_val << 8) | r);
            }
         }
         ObjectSetInteger(0, blockName, OBJPROP_BGCOLOR, block_color);
         ObjectSetInteger(0, blockName, OBJPROP_BORDER_COLOR, border_color);
         ObjectSetInteger(0, blockName, OBJPROP_BORDER_TYPE, 0);
      }
      currentGaugeY += gaugeGap + 10 + 12;
   }
   string trendStatLabels[] = { "Trend Small", "Lookback", "Direction", "Trend Big", "Lookback+", "Direction+", "PoC Daily" };
   int trend_col1_width = 100;
   int t_stats_y = currentGaugeY + 3;
   int statsLineHeight = 18;
   for(int i = 0; i < ArraySize(trendStatLabels); i++) {
      int extra_padding = 0;
      if(i >= 3) extra_padding = 5;
      int current_y = t_stats_y + (i * statsLineHeight) + extra_padding;
      if(i == 3) {
         string sepName = ea_prefix + "TrendStatSep";
         if(ObjectFind(0, sepName) < 0) {
            ObjectCreate(0, sepName, OBJ_RECTANGLE_LABEL, 0, 0, 0);
            ObjectSetInteger(0, sepName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
            ObjectSetInteger(0, sepName, OBJPROP_BGCOLOR, line_color);
            ObjectSetInteger(0, sepName, OBJPROP_BORDER_COLOR, line_color);
         }
         ObjectSetInteger(0, sepName, OBJPROP_XDISTANCE, trendPanelX + padding);
         ObjectSetInteger(0, sepName, OBJPROP_YDISTANCE, current_y - 5);
         ObjectSetInteger(0, sepName, OBJPROP_XSIZE, trendPanelW - (padding * 2));
         ObjectSetInteger(0, sepName, OBJPROP_YSIZE, 1);
         ObjectSetInteger(0, sepName, OBJPROP_BORDER_TYPE, 0);
      }
      string labelName = ea_prefix + "TrendStatLabel" + IntegerToString(i);
      string valueName = ea_prefix + "TrendStatValue" + IntegerToString(i);
      if(ObjectFind(0, labelName) < 0) {
         ObjectCreate(0, labelName, OBJ_LABEL, 0, 0, 0);
         ObjectSetInteger(0, labelName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
         ObjectSetString(0, labelName, OBJPROP_FONT, "Segoe UI");
         ObjectSetInteger(0, labelName, OBJPROP_FONTSIZE, 10);
         ObjectSetInteger(0, labelName, OBJPROP_ANCHOR, ANCHOR_LEFT_UPPER);
      }
      ObjectSetInteger(0, labelName, OBJPROP_BACK, false);
      ObjectSetInteger(0, labelName, OBJPROP_ZORDER, Z_PANEL_FOREGROUND);
      ObjectSetInteger(0, labelName, OBJPROP_XDISTANCE, trendPanelX + padding);
      ObjectSetInteger(0, labelName, OBJPROP_YDISTANCE, current_y);
      ObjectSetString(0, labelName, OBJPROP_TEXT, trendStatLabels[i]);
      ObjectSetInteger(0, labelName, OBJPROP_COLOR, text_color);
      if(ObjectFind(0, valueName) < 0) {
         ObjectCreate(0, valueName, OBJ_LABEL, 0, 0, 0);
         ObjectSetInteger(0, valueName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
         ObjectSetString(0, valueName, OBJPROP_FONT, "Segoe UI");
         ObjectSetInteger(0, valueName, OBJPROP_FONTSIZE, 10);
         ObjectSetInteger(0, valueName, OBJPROP_ANCHOR, ANCHOR_LEFT_UPPER);
      }
      ObjectSetInteger(0, valueName, OBJPROP_BACK, false);
      ObjectSetInteger(0, valueName, OBJPROP_ZORDER, Z_PANEL_FOREGROUND);
      ObjectSetInteger(0, valueName, OBJPROP_XDISTANCE, trendPanelX + trend_col1_width);
      ObjectSetInteger(0, valueName, OBJPROP_YDISTANCE, current_y);
      if(i == 0) {
         ObjectSetString(0, valueName, OBJPROP_TEXT, detectedTrendStrength_ST);
         ObjectSetInteger(0, valueName, OBJPROP_COLOR, text_color);
      }
      else if(i == 1) {
         ObjectSetString(0, valueName, OBJPROP_TEXT, IntegerToString(detectedPeriod_ST));
         ObjectSetInteger(0, valueName, OBJPROP_COLOR, text_color);
      }
      else if(i == 2) {
         if(detectedSlope_ST > 0) {
            ObjectSetString(0, valueName, OBJPROP_TEXT, "Bullish");
            ObjectSetInteger(0, valueName, OBJPROP_COLOR, looms_active_color);
         } else if(detectedSlope_ST < 0) {
            ObjectSetString(0, valueName, OBJPROP_TEXT, "Bearish");
            ObjectSetInteger(0, valueName, OBJPROP_COLOR, activate_looms_color);
         } else {
            ObjectSetString(0, valueName, OBJPROP_TEXT, "Neutral");
            ObjectSetInteger(0, valueName, OBJPROP_COLOR, text_color);
         }
      }
      else if(i == 3) {
         ObjectSetString(0, valueName, OBJPROP_TEXT, LT_detectedTrendStrength_ST);
         ObjectSetInteger(0, valueName, OBJPROP_COLOR, text_color);
      }
      else if(i == 4) {
         ObjectSetString(0, valueName, OBJPROP_TEXT, IntegerToString(LT_detectedPeriod_ST));
         ObjectSetInteger(0, valueName, OBJPROP_COLOR, text_color);
      }
      else if(i == 5) {
         if(LT_detectedSlope_ST > 0) {
            ObjectSetString(0, valueName, OBJPROP_TEXT, "Bullish");
            ObjectSetInteger(0, valueName, OBJPROP_COLOR, looms_active_color);
         } else if(LT_detectedSlope_ST < 0) {
            ObjectSetString(0, valueName, OBJPROP_TEXT, "Bearish");
            ObjectSetInteger(0, valueName, OBJPROP_COLOR, activate_looms_color);
         } else {
            ObjectSetString(0, valueName, OBJPROP_TEXT, "Neutral");
            ObjectSetInteger(0, valueName, OBJPROP_COLOR, text_color);
         }
      }
      else if(i == 6) {
         ObjectSetString(0, valueName, OBJPROP_TEXT, (dailyPoCPrice > 0) ? DoubleToString(dailyPoCPrice, Digits) : "Calculating...");
         ObjectSetInteger(0, valueName, OBJPROP_COLOR, text_color);
      }
   }
   int obvfriendPanelX = manualPanelX;
   int obvfriendPanelW = manualPanelW;
   int obvfriendPanelH = 330;
   int obvfriendPanelY = trendPanelY + trendPanelH + padding;
   string obvfriendPanelName = ea_prefix + "OBVfriendPanel";
   if(ObjectFind(0, obvfriendPanelName) < 0) {
      ObjectCreate(0, obvfriendPanelName, OBJ_RECTANGLE_LABEL, 0, 0, 0);
      ObjectSetInteger(0, obvfriendPanelName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
   }
   ObjectSetInteger(0, obvfriendPanelName, OBJPROP_BACK, false);
   ObjectSetInteger(0, obvfriendPanelName, OBJPROP_ZORDER, Z_PANEL_BACKGROUND);
   ObjectSetInteger(0, obvfriendPanelName, OBJPROP_XDISTANCE, obvfriendPanelX);
   ObjectSetInteger(0, obvfriendPanelName, OBJPROP_YDISTANCE, obvfriendPanelY);
   ObjectSetInteger(0, obvfriendPanelName, OBJPROP_XSIZE, obvfriendPanelW);
   ObjectSetInteger(0, obvfriendPanelName, OBJPROP_YSIZE, obvfriendPanelH);
   ObjectSetInteger(0, obvfriendPanelName, OBJPROP_BGCOLOR, bg_color);
   ObjectSetInteger(0, obvfriendPanelName, OBJPROP_BORDER_COLOR, border_color);
   ObjectSetInteger(0, obvfriendPanelName, OBJPROP_BORDER_TYPE, 0);
   string obvfriendButtonNames[] = { "btnOBVfriendLock", "btnOBVfriendActivate", "btnOBVfriendForceTrade", "btnOBVfriendResetStats", "btnOBVfriendShowST" };
   string obvfriendButtonTexts[5];
   if(Panel_Lock_Security == LOCKS_DISABLED) obvfriendButtonTexts[0] = "LOCK DISABLED";
   else obvfriendButtonTexts[0] = isOBVfriendPanelLocked ? "LOCKED" : "UNLOCK";
   obvfriendButtonTexts[1] = UseOBVfriend ? "OBVFRIEND ACTIVE" : "ACTIVATE OBVFRIEND";
   obvfriendButtonTexts[2] = "FORCE TRADE";
   obvfriendButtonTexts[3] = "RESET STATS";
   obvfriendButtonTexts[4] = isOBVfriendSuperTrendVisible ? "OBVf ST VISIBLE" : "OBVf ST INVISIBLE";
   color obvfriendButtonColors[5];
   if(Panel_Lock_Security == LOCKS_DISABLED) obvfriendButtonColors[0] = text_color;
   else obvfriendButtonColors[0] = isOBVfriendPanelLocked ? locked_color : lock_color;
   obvfriendButtonColors[1] = UseOBVfriend ? looms_active_color : activate_looms_color;
   obvfriendButtonColors[2] = force_trade_color;
   obvfriendButtonColors[3] = text_color;
   obvfriendButtonColors[4] = isOBVfriendSuperTrendVisible ? looms_active_color : activate_looms_color;
   int obvfriendButtonY = obvfriendPanelY + padding;
   for(int i = 0; i < ArraySize(obvfriendButtonNames); i++) {
      string bgName = ea_prefix + obvfriendButtonNames[i] + "_bg";
      string textName = ea_prefix + obvfriendButtonNames[i] + "_text";
      if(ObjectFind(0, bgName) < 0) {
         ObjectCreate(0, bgName, OBJ_RECTANGLE_LABEL, 0, 0, 0);
         ObjectSetInteger(0, bgName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
         ObjectSetInteger(0, bgName, OBJPROP_SELECTABLE, true);
      }
      ObjectSetInteger(0, bgName, OBJPROP_BACK, false);
      ObjectSetInteger(0, bgName, OBJPROP_ZORDER, Z_BTNS_BG);
      ObjectSetInteger(0, bgName, OBJPROP_XDISTANCE, obvfriendPanelX + padding);
      ObjectSetInteger(0, bgName, OBJPROP_YDISTANCE, obvfriendButtonY);
      ObjectSetInteger(0, bgName, OBJPROP_XSIZE, obvfriendPanelW - (padding * 2));
      ObjectSetInteger(0, bgName, OBJPROP_YSIZE, mButtonH);
      ObjectSetInteger(0, bgName, OBJPROP_BGCOLOR, button_bg_color);
      ObjectSetInteger(0, bgName, OBJPROP_BORDER_COLOR, border_color);
      ObjectSetInteger(0, bgName, OBJPROP_BORDER_TYPE, 0);
      if(ObjectFind(0, textName) < 0) {
         ObjectCreate(0, textName, OBJ_LABEL, 0, 0, 0);
         ObjectSetInteger(0, textName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
         ObjectSetString(0, textName, OBJPROP_FONT, "Segoe UI Bold");
         ObjectSetInteger(0, textName, OBJPROP_FONTSIZE, 9);
         ObjectSetInteger(0, textName, OBJPROP_ANCHOR, ANCHOR_CENTER);
      }
      ObjectSetInteger(0, textName, OBJPROP_BACK, false);
      ObjectSetInteger(0, textName, OBJPROP_ZORDER, Z_BTNS_TXT);
      ObjectSetInteger(0, textName, OBJPROP_XDISTANCE, obvfriendPanelX + obvfriendPanelW / 2);
      ObjectSetInteger(0, textName, OBJPROP_YDISTANCE, obvfriendButtonY + mButtonH / 2);
      ObjectSetString(0, textName, OBJPROP_TEXT, obvfriendButtonTexts[i]);
      ObjectSetInteger(0, textName, OBJPROP_COLOR, obvfriendButtonColors[i]);
      obvfriendButtonY += mButtonH + spaceBetween;
   }
   if(g_isLoading) LogBootMessage("UI: OBVfriend Panel Created.");
   obvfriendButtonY -= spaceBetween;
   obvfriendButtonY += 10;
   string obvfriendHeaderName = ea_prefix + "OBVfriendHeader";
   if(ObjectFind(0, obvfriendHeaderName) < 0) {
      ObjectCreate(0, obvfriendHeaderName, OBJ_LABEL, 0, 0, 0);
      ObjectSetInteger(0, obvfriendHeaderName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
      ObjectSetString(0, obvfriendHeaderName, OBJPROP_FONT, "Segoe UI Bold");
      ObjectSetInteger(0, obvfriendHeaderName, OBJPROP_FONTSIZE, 9);
   }
   ObjectSetInteger(0, obvfriendHeaderName, OBJPROP_BACK, false);
   ObjectSetInteger(0, obvfriendHeaderName, OBJPROP_ZORDER, Z_PANEL_FOREGROUND);
   ObjectSetInteger(0, obvfriendHeaderName, OBJPROP_XDISTANCE, obvfriendPanelX + padding);
   ObjectSetInteger(0, obvfriendHeaderName, OBJPROP_YDISTANCE, obvfriendButtonY);
   ObjectSetString(0, obvfriendHeaderName, OBJPROP_TEXT, "OBVFRIEND");
   ObjectSetInteger(0, obvfriendHeaderName, OBJPROP_COLOR, header_color);
   obvfriendButtonY += 15;
   string obvfriendStatLabels[] = { "Wins", "Losses", "Total Profit", "Total Loss", "Commissions", "Daily Loss", "Last Close P/L" };
   int obvfriend_col1_width = 100;
   int obvfriend_stats_y = obvfriendButtonY + 5;
   for(int i = 0; i < ArraySize(obvfriendStatLabels); i++) {
      int current_y = obvfriend_stats_y + (i * lineHeight);
      string labelName = ea_prefix + "OBVfriendStatLabel" + IntegerToString(i);
      string valueName = ea_prefix + "OBVfriendStatValue" + IntegerToString(i);
      string lineName = ea_prefix + "OBVfriendStatLine" + IntegerToString(i);
      if(ObjectFind(0, labelName) < 0) {
         ObjectCreate(0, labelName, OBJ_LABEL, 0, 0, 0);
         ObjectSetInteger(0, labelName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
         ObjectSetString(0, labelName, OBJPROP_FONT, "Segoe UI");
         ObjectSetInteger(0, labelName, OBJPROP_FONTSIZE, 10);
         ObjectSetInteger(0, labelName, OBJPROP_ANCHOR, ANCHOR_LEFT_UPPER);
      }
      ObjectSetInteger(0, labelName, OBJPROP_BACK, false);
      ObjectSetInteger(0, labelName, OBJPROP_ZORDER, Z_PANEL_FOREGROUND);
      ObjectSetInteger(0, labelName, OBJPROP_XDISTANCE, obvfriendPanelX + padding);
      ObjectSetInteger(0, labelName, OBJPROP_YDISTANCE, current_y);
      ObjectSetString(0, labelName, OBJPROP_TEXT, obvfriendStatLabels[i]);
      ObjectSetInteger(0, labelName, OBJPROP_COLOR, text_color);
      if(ObjectFind(0, valueName) < 0) {
         ObjectCreate(0, valueName, OBJ_LABEL, 0, 0, 0);
         ObjectSetInteger(0, valueName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
         ObjectSetString(0, valueName, OBJPROP_FONT, "Segoe UI");
         ObjectSetInteger(0, valueName, OBJPROP_FONTSIZE, 10);
         ObjectSetInteger(0, valueName, OBJPROP_ANCHOR, ANCHOR_LEFT_UPPER);
      }
      ObjectSetInteger(0, valueName, OBJPROP_BACK, false);
      ObjectSetInteger(0, valueName, OBJPROP_ZORDER, Z_PANEL_FOREGROUND);
      ObjectSetInteger(0, valueName, OBJPROP_XDISTANCE, obvfriendPanelX + obvfriend_col1_width);
      ObjectSetInteger(0, valueName, OBJPROP_YDISTANCE, current_y);
      if(ObjectFind(0, lineName) < 0) {
         ObjectCreate(0, lineName, OBJ_RECTANGLE_LABEL, 0, 0, 0);
         ObjectSetInteger(0, lineName, OBJPROP_CORNER, CORNER_LEFT_UPPER);
         ObjectSetInteger(0, lineName, OBJPROP_BGCOLOR, line_color);
         ObjectSetInteger(0, lineName, OBJPROP_BORDER_COLOR, line_color);
      }
      ObjectSetInteger(0, lineName, OBJPROP_BACK, false);
      ObjectSetInteger(0, lineName, OBJPROP_ZORDER, Z_PANEL_BACKGROUND);
      ObjectSetInteger(0, lineName, OBJPROP_XDISTANCE, obvfriendPanelX + padding);
      ObjectSetInteger(0, lineName, OBJPROP_YDISTANCE, current_y + lineHeight - 4);
      ObjectSetInteger(0, lineName, OBJPROP_XSIZE, manualPanelW - (padding * 2));
      ObjectSetInteger(0, lineName, OBJPROP_YSIZE, 1);
      ObjectSetInteger(0, lineName, OBJPROP_BORDER_TYPE, 0);
   }
   int obvfriend_val_idx = 0;
   ObjectSetString(0, ea_prefix + "OBVfriendStatValue" + IntegerToString(obvfriend_val_idx), OBJPROP_TEXT, IntegerToString(obvfriend_wins));
   ObjectSetInteger(0, ea_prefix + "OBVfriendStatValue" + IntegerToString(obvfriend_val_idx++), OBJPROP_COLOR, obvfriend_wins > 0 ? bullish_color : text_color);
   ObjectSetString(0, ea_prefix + "OBVfriendStatValue" + IntegerToString(obvfriend_val_idx), OBJPROP_TEXT, IntegerToString(obvfriend_losses));
   ObjectSetInteger(0, ea_prefix + "OBVfriendStatValue" + IntegerToString(obvfriend_val_idx++), OBJPROP_COLOR, obvfriend_losses > 0 ? bearish_color : text_color);
   ObjectSetString(0, ea_prefix + "OBVfriendStatValue" + IntegerToString(obvfriend_val_idx), OBJPROP_TEXT, "$" + DoubleToString(obvfriend_profitTotal, 2));
   ObjectSetInteger(0, ea_prefix + "OBVfriendStatValue" + IntegerToString(obvfriend_val_idx++), OBJPROP_COLOR, obvfriend_profitTotal > 0 ? bullish_color : text_color);
   ObjectSetString(0, ea_prefix + "OBVfriendStatValue" + IntegerToString(obvfriend_val_idx), OBJPROP_TEXT, "$" + DoubleToString(obvfriend_lossTotal, 2));
   ObjectSetInteger(0, ea_prefix + "OBVfriendStatValue" + IntegerToString(obvfriend_val_idx++), OBJPROP_COLOR, obvfriend_lossTotal > 0 ? bearish_color : text_color);
   ObjectSetString(0, ea_prefix + "OBVfriendStatValue" + IntegerToString(obvfriend_val_idx), OBJPROP_TEXT, "$" + DoubleToString(MathAbs(obvfriend_totalCommissions), 2));
   ObjectSetInteger(0, ea_prefix + "OBVfriendStatValue" + IntegerToString(obvfriend_val_idx++), OBJPROP_COLOR, obvfriend_totalCommissions != 0 ? bearish_color : text_color);
   ObjectSetString(0, ea_prefix + "OBVfriendStatValue" + IntegerToString(obvfriend_val_idx), OBJPROP_TEXT, "$" + DoubleToString(obvfriend_currentDailyLoss, 2));
   ObjectSetInteger(0, ea_prefix + "OBVfriendStatValue" + IntegerToString(obvfriend_val_idx++), OBJPROP_COLOR, obvfriend_currentDailyLoss > 0 ? bearish_color : text_color);
   string obvfriendPnlString = (obvfriend_lastClosedTradeProfit >= 0 ? "+$" : "-$") + DoubleToString(MathAbs(obvfriend_lastClosedTradeProfit), 2);
   if(obvfriend_lastClosedTradeClosePrice == 0) obvfriendPnlString = "-";
   ObjectSetString(0, ea_prefix + "OBVfriendStatValue" + IntegerToString(obvfriend_val_idx), OBJPROP_TEXT, obvfriendPnlString);
   ObjectSetInteger(0, ea_prefix + "OBVfriendStatValue" + IntegerToString(obvfriend_val_idx++), OBJPROP_COLOR, obvfriend_lastClosedTradeProfit >= 0 ? bullish_color : bearish_color);
   if(g_isLoading) LogBootMessage("UI: Panel Generation Complete.");
   EnforceUIPanelLayering();
}
void EnforceUIPanelLayering() {
   const int Z_PANEL_BACK = 30000;
   const int Z_PANEL_FRONT = 30001;
   const int Z_BTNS_BG = 30004;
   const int Z_BTNS_TXT = 30005;
   const int Z_TOP_BUTTON_BG = 30006;
   const int Z_TOP_BUTTON_TEXT = 30007;
   for(int i = ObjectsTotal(0, 0, -1) - 1; i >= 0; i--) {
      string name = ObjectName(0, i, -1, -1);
      if(StringFind(name, ea_prefix) != 0) continue;
      if(StringFind(name, "btnTogglePanel") >= 0 || StringFind(name, "btnSessions") >= 0 || StringFind(name, "btnTrends") >= 0 || StringFind(name, "btnDump") >= 0 ||
         StringFind(name, "btnDots") >= 0 || StringFind(name, "btnPoc") >= 0 || StringFind(name, "btnOBV_") >= 0 || StringFind(name, "btnQuantToggle") >= 0) {
         if(StringFind(name, "_bg") > 0) ObjectSetInteger(0, name, OBJPROP_ZORDER, Z_TOP_BUTTON_BG);
         else ObjectSetInteger(0, name, OBJPROP_ZORDER, Z_TOP_BUTTON_TEXT);
         ObjectSetInteger(0, name, OBJPROP_BACK, false);
      }
      else if(StringFind(name, "LoomsMainPanel") > 0 || StringFind(name, "ManualTradePanel") > 0 || StringFind(name, "OBVfriendPanel") > 0 || StringFind(name, "QuantDashboardPanel") > 0 || StringFind(name, "SqueezePanel") > 0) {
         ObjectSetInteger(0, name, OBJPROP_BACK, false);
         ObjectSetInteger(0, name, OBJPROP_ZORDER, Z_PANEL_BACK);
      }
      else if(StringFind(name, "btn") > 0) {
         if(StringFind(name, "_bg") > 0) ObjectSetInteger(0, name, OBJPROP_ZORDER, Z_BTNS_BG);
         else ObjectSetInteger(0, name, OBJPROP_ZORDER, Z_BTNS_TXT);
         ObjectSetInteger(0, name, OBJPROP_BACK, false);
      }
      else if(StringFind(name, "Header") > 0 || StringFind(name, "Label") > 0 || StringFind(name, "Value") > 0 || StringFind(name, "Title") > 0) {
         ObjectSetInteger(0, name, OBJPROP_BACK, false);
         ObjectSetInteger(0, name, OBJPROP_ZORDER, Z_PANEL_FRONT);
      }
      else if(StringFind(name, "Line") >= 0) {
         ObjectSetInteger(0, name, OBJPROP_BACK, false);
         ObjectSetInteger(0, name, OBJPROP_ZORDER, Z_PANEL_BACK);
      }
   }
}
//+------------------------------------------------------------------+
//| SECTION 9.1 - DAILY SUMMARY REPORTING                            |
//+------------------------------------------------------------------+
void LogEndOfDaySummary() {
   if(!UseDots) return;
   datetime estNow=GetEstTime();
   if(TimeHour(estNow)!=17||dots_dailySummaryPrinted) return;
   dots_dailySummaryPrinted=true;
   int totalToday=dots_today_wins+dots_today_losses;
   double winRate=(totalToday>0)?(100.0*(double)dots_today_wins/(double)totalToday):0.0;
   double ftmoPct=0.0;
   if(MaxDailyLoss>0.0) ftmoPct=(combinedCurrentDailyLoss/MaxDailyLoss)*100.0;
   Print("DOTS| =======================================");
   Print("DOTS| === DOTS END OF DAY SUMMARY ===");
   Print("DOTS| Trades: ",totalToday,
         " | Wins: ",dots_today_wins,
         " | Losses: ",dots_today_losses,
         " | WR: ",DoubleToString(winRate,1),"%");
   Print("DOTS| P&L: ",DoubleToString(dots_today_pnl,2)," pts",
         " ($",DoubleToString(dots_today_pnl*Dots_LotSize,2),")");
   Print("DOTS| Exits: SL=",dots_today_sl,
         " Feature=",dots_today_feat,
         " Time=",dots_today_time);
   Print("DOTS| Active at close: ",dots_active_count);
   Print("DOTS| FTMO daily usage: ",DoubleToString(ftmoPct,1),
         "% ($",DoubleToString(combinedCurrentDailyLoss,2),
         " / $",DoubleToString(MaxDailyLoss,2),")");
   Print("DOTS| --- Per-Rule Performance (all time) ---");
   for(int i=0; i<DOTS_NUM_RULES; i++) {
      int rTotal=dots_rule_wins[i]+dots_rule_losses[i];
      if(rTotal==0) continue;
      double rWR=100.0*(double)dots_rule_wins[i]/(double)rTotal;
      Print("DOTS| ",dots_ruleName[i],": ",rTotal," trades | ",
            dots_rule_wins[i],"W ",dots_rule_losses[i],"L | ",
            DoubleToString(rWR,1),"% | $",
            DoubleToString(dots_rule_pnl[i],2));
   }
   Print("DOTS| =======================================");
}
//+------------------------------------------------------------------+
//| SECTION 10.0 - UI BUTTON EVENTS                                  |
//+------------------------------------------------------------------+
bool CheckManualSpread(double lotSize) {
   if(MaxSpreadUSD <= 0) return true;
   double spreadPoints = MarketInfo(Symbol(), MODE_SPREAD);
   double tickValue = MarketInfo(Symbol(), MODE_TICKVALUE);
   double tickSize = MarketInfo(Symbol(), MODE_TICKSIZE);
   double spreadCost = 0.0;
   if(tickSize > 0) spreadCost = (spreadPoints * Point * tickValue / tickSize) * lotSize;
   if(spreadCost >= MaxSpreadUSD) {
      Print("SECURITY ALERT (Manual): Current spread cost $", DoubleToString(spreadCost, 2), " exceeds max of $", DoubleToString(MaxSpreadUSD, 2), ". Trade aborted.");
      return false;
   }
   return true;
}
void OnChartEvent(const int id, const long &lparam, const double &dparam, const string &sparam) {
   if(id != CHARTEVENT_OBJECT_CLICK) return;
   if(StringFind(sparam, ea_prefix) != 0) return;
   string clicked_object = StringSubstr(sparam, StringLen(ea_prefix));
   if(clicked_object == "btnTogglePanel_bg" || clicked_object == "btnTogglePanel_text") {
      isPanelVisible = !isPanelVisible;
      DrawControlPanel();
      ChartRedraw();
      return;
   }
   else if(StringFind(clicked_object, "btnSessions") == 0) {
      isSessionVisualsVisible = !isSessionVisualsVisible;
      if(!isSessionVisualsVisible) DeleteSessionObjects();
      else DrawSessionBoxes();
      DrawControlPanel();
      ChartRedraw();
      return;
   }
   else if(StringFind(clicked_object, "btnTrends") == 0) {
      isTrendVisualsVisible = !isTrendVisualsVisible;
      if(!isTrendVisualsVisible) {
         DeleteST_AdaptiveTrendChannel();
         DeleteLT_AdaptiveTrendChannel();
         DeleteST_TrendDirectionIndicator();
         DeleteLT_TrendDirectionIndicator();
         DeleteTrendFlipVLines();
      }
      else {
         DrawST_AdaptiveTrendChannel();
         DrawLT_AdaptiveTrendChannel();
         DrawHistoricalIndicators_FromBuffers();
      }
      DrawControlPanel();
      ChartRedraw();
      return;
   }
   else if(StringFind(clicked_object, "btnDump") == 0) {
      ExportDataForAnalysis();
      return;
   }
   else if(StringFind(clicked_object, "btnDots") == 0) {
      isSignalDotsVisible = !isSignalDotsVisible;
      if(!isSignalDotsVisible) DeleteSignalMarkers();
      else RedrawD2DSignals();
      DrawControlPanel();
      ChartRedraw();
      return;
   }
   else if(StringFind(clicked_object, "btnQuantToggle8") == 0) {
      isDotsVisualsVisible = !isDotsVisualsVisible;
      Print("Quant Toggle 8 Clicked. DOTS Monitor Panel: ", isDotsVisualsVisible);
      if(!isDotsVisualsVisible) RemoveDotsPanel();
      else DrawDotsPanel();
      DrawControlPanel();
      ChartRedraw();
      return;
   }
   if(sparam==ea_prefix+"btnDotsActivate_bg"||sparam==ea_prefix+"btnDotsActivate_text") {
      isDotsTradeActive=!isDotsTradeActive;
      Print("DOTS| Trading is now: ",isDotsTradeActive?"ACTIVE":"INACTIVE");
      if(isDotsVisualsVisible) DrawDotsPanel();
      ChartRedraw();
      return;
   }
   else if(StringFind(clicked_object, "btnPoc") == 0) {
      isPocVisualsVisible = !isPocVisualsVisible;
      if(!isPocVisualsVisible) DeleteDailyPoC();
      else DrawDailyPoC();
      DrawControlPanel();
      ChartRedraw();
      return;
   }
   else if(StringFind(clicked_object, "btnOBV_") == 0) {
      isOBVVisualsVisible = !isOBVVisualsVisible;
      if(!isOBVVisualsVisible) DeleteOBV_Visuals();
      else {
         for(int i = Bars - 1; i >= 1; i--) DrawOBV_Visuals_Historical(i);
      }
      DrawControlPanel();
      ChartRedraw();
      return;
   }
   else if(StringFind(clicked_object, "btnQuantToggle1") == 0) {
      showOBVfCandles = !showOBVfCandles;
      Print("Quant Toggle 1 Clicked. OBVf mode: ", showOBVfCandles);
      if(showOBVfCandles) {
         isOBVfriendSuperTrendVisible = true;
         isSuperTrendVisible = false;
         DeleteSuperTrendLine();
         DrawOBVfriendSuperTrendLine();
      } else {
         isOBVfriendSuperTrendVisible = false;
         isSuperTrendVisible = true;
         DeleteOBVfriendSuperTrendLine();
         DrawSuperTrendLine();
      }
      for(int i=ObjectsTotal(0,-1,-1)-1; i>=0; i--) {
         string name=ObjectName(0,i,-1,-1);
         if(StringFind(name,ea_prefix+"c_")==0) ObjectDelete(0,name);
      }
      ColorSignalBars();
      SyncLiveSuperTrendVisuals();
      DrawLiveSignalBarSegment();
      if(isSuperTrendVisible) DrawLiveSuperTrendSegment();
      if(isOBVfriendSuperTrendVisible) DrawLiveOBVfriendSuperTrendSegment();
      DrawControlPanel();
      ChartRedraw();
      return;
   }
   else if(StringFind(clicked_object, "btnQuantToggle2") == 0) {
      isHarmonicVolVisible = !isHarmonicVolVisible;
      Print("Quant Toggle 2 Clicked. Harmonic Vol: ", isHarmonicVolVisible);
      if(!isHarmonicVolVisible) {
         DeleteHarmonicVolumeObjects();
      } else {
         DrawHarmonicVolumeCandles();
      }
      DrawControlPanel();
      ChartRedraw();
      return;
   }
   if(!isPanelVisible) return;
   bool mainLockOverride = (Panel_Lock_Security == LOCKS_DISABLED);
   if(StringFind(clicked_object, "btnQuantToggle3") == 0) {
      isOBVfLineVisible = !isOBVfLineVisible;
      Print("Quant Toggle 3 Clicked. OBVf Signal Line: ", isOBVfLineVisible);
      if(!isOBVfLineVisible) {
         for(int i=ObjectsTotal(0,-1,-1)-1; i>=0; i--) {
            string name=ObjectName(0,i,-1,-1);
            if(StringFind(name,ea_prefix+"obv_signal_")==0) ObjectDelete(0,name);
         }
      } else {
         if(isOBVVisualsVisible) {
            for(int i=Bars-1; i>=1; i--) DrawOBV_Visuals_Historical(i);
         }
      }
      DrawControlPanel();
      ChartRedraw();
      return;
   }
   else if(StringFind(clicked_object, "btnQuantToggle4") == 0) {
      isPriceTrackerVisible = !isPriceTrackerVisible;
      Print("Quant Toggle 4 Clicked. KAMA Volume Oscillator: ", isPriceTrackerVisible);
      if(!isPriceTrackerVisible) {
         for(int i=ObjectsTotal(0,-1,-1)-1; i>=0; i--) {
            string name=ObjectName(0,i,-1,-1);
            if(StringFind(name,ea_prefix+"obv_emaline_")==0) ObjectDelete(0,name);
         }
      } else {
         if(isOBVVisualsVisible) {
            for(int i=Bars-1; i>=1; i--) DrawOBV_Visuals_Historical(i);
         }
      }
      DrawControlPanel();
      ChartRedraw();
      return;
   }
   else if(StringFind(clicked_object, "btnQuantToggle5") == 0) {
      isKamaHistoVisible = !isKamaHistoVisible;
      Print("Quant Toggle 5 Clicked. KAMA Histo: ", isKamaHistoVisible);
      if(!isKamaHistoVisible) {
         for(int i=ObjectsTotal(0,-1,-1)-1; i>=0; i--) {
            string name=ObjectName(0,i,-1,-1);
            if(StringFind(name,ea_prefix+"obv_llema_")==0) ObjectDelete(0,name);
         }
      } else {
         if(isOBVVisualsVisible) {
            for(int i=Bars-1; i>=1; i--) DrawOBV_Visuals_Historical(i);
         }
      }
      DrawControlPanel();
      ChartRedraw();
      return;
   }
   else if(StringFind(clicked_object, "btnQuantToggle6") == 0) {
      Print("Quant Toggle 6 Clicked. Squashing Chart...");
      SquashChartToMiddleThird(ChartID());
      ChartRedraw();
      return;
   }
   else if(StringFind(clicked_object, "btnQuantToggle7") == 0) {
      isRangeOscVisible = !isRangeOscVisible;
      Print("Quant Toggle 7 Clicked. Range Oscillator: ", isRangeOscVisible);
      if(!isRangeOscVisible) {
         for(int i=ObjectsTotal(0,-1,-1)-1; i>=0; i--) {
            string name=ObjectName(0,i,-1,-1);
            if(StringFind(name,ea_prefix+"obv_rosc_")==0) ObjectDelete(0,name);
         }
      } else {
         if(isOBVVisualsVisible) {
            for(int i=Bars-1; i>=1; i--) DrawOBV_Visuals_Historical(i);
         }
      }
      DrawControlPanel();
      ChartRedraw();
      return;
   }
   if(StringFind(clicked_object, "btnLock") == 0 || StringFind(clicked_object, "btnLooms") == 0 ||
      StringFind(clicked_object, "btnForceTrade") == 0 || (StringFind(clicked_object, "btnResetStats") == 0 && StringFind(clicked_object, "btnM") != 0 && StringFind(clicked_object, "btnOBVfriend") != 0) ||
      StringFind(clicked_object, "btnRePaint") == 0 || StringFind(clicked_object, "btnST") == 0 || StringFind(clicked_object, "btnTradeHistory") == 0) {
      if(StringFind(clicked_object, "btnLock") == 0) {
         if(!mainLockOverride) {
            isLocked = !isLocked;
            if(!isLocked) statsPanelUnlockTime = TimeCurrent();
            Print(isLocked ? "Looms EA controls locked." : "Looms EA controls unlocked for 10 seconds.");
         }
      }
      else if(isLocked && !mainLockOverride) {
         Print("Looms EA is locked. Unlock to use other controls.");
      }
      else {
         if(StringFind(clicked_object, "btnLooms") == 0) {
            isLoomsActive = !isLoomsActive;
            if(isLoomsActive) { activationTime = TimeCurrent(); Print("Looms EA activated."); }
            else { Print("Looms EA deactivated."); }
         }
         else if(StringFind(clicked_object, "btnForceTrade") == 0) ForceTradeExecution();
         else if(StringFind(clicked_object, "btnResetStats") == 0) {
            statsResetTime = TimeCurrent();
            manual_statsResetTime = TimeCurrent();
            UpdateStatsFromHistory();
            Print("All EA and Manual statistics have been reset.");
         }
         else if(StringFind(clicked_object, "btnRePaint") == 0) {
            ObjectsDeleteAll(0, ea_prefix + "Boot");
            RemoveBootConsole();
            g_isLoading = true;
            g_loadingStartTime = GetTickCount();
            boot_log_counter = 0;
            for(int b = 0; b < 65; b++) boot_log_lines[b] = "";
            DrawBootConsole();
         }
         else if(StringFind(clicked_object, "btnST") == 0) {
            isSuperTrendVisible = !isSuperTrendVisible;
            if(isSuperTrendVisible) { Print("SuperTrend visual ON."); DrawSuperTrendLine(); }
            else { Print("SuperTrend visual OFF."); DeleteSuperTrendLine(); }
         }
         else if(StringFind(clicked_object, "btnTradeHistory") == 0) {
            isTradeHistoryVisible = !isTradeHistoryVisible;
            if(isTradeHistoryVisible) DrawCustomTradeHistory();
            else DeleteTradeHistoryObjects();
         }
      }
      DrawControlPanel();
      ChartRedraw();
      return;
   }
   else if(StringFind(clicked_object, "btnMLock") == 0 || StringFind(clicked_object, "btnMTrade") == 0 ||
           StringFind(clicked_object, "btnMClose") == 0 || StringFind(clicked_object, "btnMResetStats") == 0) {
      if(StringFind(clicked_object, "btnMLock") == 0) {
         if(!mainLockOverride) {
            isManualPanelLocked = !isManualPanelLocked;
            if(!isManualPanelLocked) manualPanelUnlockTime = TimeCurrent();
            Print(isManualPanelLocked ? "Manual trade panel locked." : "Manual trade panel unlocked for 10 seconds.");
         }
      }
      else if(isManualPanelLocked && !mainLockOverride) {
         Print("Manual trade panel is locked. Unlock to place trades.");
      }
      else {
         RefreshRates();
         if(!IsTradeAllowed()) { Print("[Manual] Trade execution failed. Check AutoTrading button and EA properties."); return; }
         if(combinedCurrentDailyLoss >= MaxDailyLoss && MaxDailyLoss > 0) { Print("[Manual] Trade blocked: Combined daily loss limit has been reached."); return; }
         if(UseOpenHours) {
            datetime estTime = GetEstTime();
            int currentHourEST = TimeHour(estTime);
            int currentMinuteEST = TimeMinute(estTime);
            int openTotal = OpenHourEST * 60 + OpenMinuteEST;
            int closeTotal = CloseHourEST * 60 + CloseMinuteEST;
            int currentTotal = currentHourEST * 60 + currentMinuteEST;
            if(currentTotal < openTotal || currentTotal >= closeTotal) { Print("[Manual] Trade blocked: Outside session."); return; }
         }
         int estDay = GetEstDayOfWeek(TimeGMT());
         bool dayAllowed = true;
         if(estDay == 1 && !TradeOnMonday) dayAllowed = false;
         else if(estDay == 2 && !TradeOnTuesday) dayAllowed = false;
         else if(estDay == 3 && !TradeOnWednesday) dayAllowed = false;
         else if(estDay == 4 && !TradeOnThursday) dayAllowed = false;
         else if(estDay == 5 && !TradeOnFriday) dayAllowed = false;
         else if(estDay == 0 && !TradeOnSunday) dayAllowed = false;
         else if(estDay == 6) dayAllowed = false;
         if(!dayAllowed) { Print("[Manual] Trade blocked: Trading not allowed today."); return; }
         string comment = "Manual_" + IntegerToString(ManualMagicNumber);
         int ticket = -1;
         double sl = 0;
         int lastSigIdx = 1;
         for(int k = 1; k < Bars; k++) {
            if(LockBuffer[k] != 0) { lastSigIdx = k; break; }
         }
         double histDist = 0;
         if(SuperTrend[lastSigIdx] > 0 && SuperTrend[lastSigIdx] != EMPTY_VALUE) {
            histDist = MathAbs(Close[lastSigIdx] - SuperTrend[lastSigIdx]);
         }
         if(histDist == 0) histDist = 50 * Point;
         if(StringFind(clicked_object, "btnMTradeBuy") == 0) {
            for(int i = OrdersTotal() - 1; i >= 0; i--) {
               if(OrderSelect(i, SELECT_BY_POS, MODE_TRADES) && OrderSymbol() == Symbol() && OrderMagicNumber() == ManualMagicNumber && OrderType() == OP_SELL) {
                  if(!OrderClose(OrderTicket(), OrderLots(), Ask, Slippage, clrRed)) Print("[Manual] Failed to purge opposing SELL order: ", GetLastError());
               }
            }
            sl = NormalizeDouble(Ask - histDist, Digits);
            sl = ValidateStopLoss(OP_BUY, sl);
            if(StringFind(clicked_object, "btnMTradeBuy1") == 0) {
               if(CheckManualSpread(Manual_Buy_Lot_1)) ticket = OrderSend(Symbol(), OP_BUY, Manual_Buy_Lot_1, Ask, Slippage, sl, 0, comment, ManualMagicNumber, 0, C'146,134,124');
            }
            else if(StringFind(clicked_object, "btnMTradeBuy2") == 0) {
               if(CheckManualSpread(Manual_Buy_Lot_2)) ticket = OrderSend(Symbol(), OP_BUY, Manual_Buy_Lot_2, Ask, Slippage, sl, 0, comment, ManualMagicNumber, 0, C'146,134,124');
            }
         }
         else if(StringFind(clicked_object, "btnMTradeSell") == 0) {
            for(int i = OrdersTotal() - 1; i >= 0; i--) {
               if(OrderSelect(i, SELECT_BY_POS, MODE_TRADES) && OrderSymbol() == Symbol() && OrderMagicNumber() == ManualMagicNumber && OrderType() == OP_BUY) {
                  if(!OrderClose(OrderTicket(), OrderLots(), Bid, Slippage, clrRed)) Print("[Manual] Failed to purge opposing BUY order: ", GetLastError());
               }
            }
            sl = NormalizeDouble(Bid + histDist, Digits);
            sl = ValidateStopLoss(OP_SELL, sl);
            if(StringFind(clicked_object, "btnMTradeSell1") == 0) {
               if(CheckManualSpread(Manual_Sell_Lot_1)) ticket = OrderSend(Symbol(), OP_SELL, Manual_Sell_Lot_1, Bid, Slippage, sl, 0, comment, ManualMagicNumber, 0, C'89,116,124');
            }
            else if(StringFind(clicked_object, "btnMTradeSell2") == 0) {
               if(CheckManualSpread(Manual_Sell_Lot_2)) ticket = OrderSend(Symbol(), OP_SELL, Manual_Sell_Lot_2, Bid, Slippage, sl, 0, comment, ManualMagicNumber, 0, C'89,116,124');
            }
         }
         else if(StringFind(clicked_object, "btnMClose") == 0) CloseManualTrades();
         else if(StringFind(clicked_object, "btnMResetStats") == 0) {
            manual_statsResetTime = TimeCurrent();
            UpdateStatsFromHistory();
            Print("Manual trade statistics have been reset.");
         }
         if(ticket < 0 && StringFind(clicked_object, "btnMTrade") == 0) Print("[Manual] order FAILED. Error: ", GetLastError());
      }
      DrawControlPanel();
      ChartRedraw();
      return;
   }
   else if(StringFind(clicked_object, "btnOBVfriendLock") == 0 || StringFind(clicked_object, "btnOBVfriendActivate") == 0 ||
           StringFind(clicked_object, "btnOBVfriendForceTrade") == 0 || StringFind(clicked_object, "btnOBVfriendResetStats") == 0 ||
           StringFind(clicked_object, "btnOBVfriendShowST") == 0) {
      if(StringFind(clicked_object, "btnOBVfriendLock") == 0) {
         if(!mainLockOverride) {
            isOBVfriendPanelLocked = !isOBVfriendPanelLocked;
            if(!isOBVfriendPanelLocked) obvfriendPanelUnlockTime = TimeCurrent();
            Print(isOBVfriendPanelLocked ? "OBVfriend panel locked." : "OBVfriend panel unlocked for 10 seconds.");
         }
      }
      else if(isOBVfriendPanelLocked && !mainLockOverride) {
         Print("OBVfriend panel is locked. Unlock to use controls.");
      }
      else {
         if(StringFind(clicked_object, "btnOBVfriendResetStats") == 0) {
            obvfriend_statsResetTime = TimeCurrent();
            obvfriend_lastTradedDirection = 0;
            obvfriend_lastVisualDirection = 0;
            UpdateStatsFromHistory();
            Print("OBVfriend statistics, trade logic, and visual logic have been reset.");
         }
         else if(StringFind(clicked_object, "btnOBVfriendForceTrade") == 0) ForceOBVfriendTradeExecution();
         else if(StringFind(clicked_object, "btnOBVfriendActivate") == 0) {
            UseOBVfriend = !UseOBVfriend;
            Print(UseOBVfriend ? "OBVfriend mode activated." : "OBVfriend mode deactivated.");
         }
         else if(StringFind(clicked_object, "btnOBVfriendShowST") == 0) {
            isOBVfriendSuperTrendVisible = !isOBVfriendSuperTrendVisible;
            if(isOBVfriendSuperTrendVisible) { Print("OBVfriend SuperTrend visual ON."); DrawOBVfriendSuperTrendLine(); }
            else { Print("OBVfriend SuperTrend visual OFF."); DeleteOBVfriendSuperTrendLine(); }
         }
      }
      DrawControlPanel();
      ChartRedraw();
      return;
   }
}
//+------------------------------------------------------------------+
//| SECTION 10.1 - TIMER EVENT                                       |
//+------------------------------------------------------------------+
void OnTimer() {
   if(g_isLoading) {
      if(GetTickCount()-g_loadingStartTime>3000) {
         g_isLoading=false;
         DrawLoadingBar(1.0,"SYSTEM READY...");
         Print("LOOMS BOOT COMPLETE. System Ready.");
         ForceRePaintSignals();
      } else {
         double progress=(double)(GetTickCount()-g_loadingStartTime)/3000.0;
         DrawLoadingBar(progress,"INITIALIZING SYSTEM...");
         ChartRedraw();
         return;
      }
   }
   if(g_loadingStartTime>0&&!g_isLoading&&GetTickCount()-g_loadingStartTime>60000) {
      RemoveBootConsole();
      RemoveLoadingBar();
      g_loadingStartTime=0;
   }
   if(!isPanelVisible) return;
   if(!isLocked&&(TimeCurrent()-statsPanelUnlockTime>=10)) {
      isLocked=true;
      DrawControlPanel();
      ChartRedraw();
   }
   if(!isManualPanelLocked&&(TimeCurrent()-manualPanelUnlockTime>=10)) {
      isManualPanelLocked=true;
      DrawControlPanel();
      ChartRedraw();
   }
   if(!isOBVfriendPanelLocked&&(TimeCurrent()-obvfriendPanelUnlockTime>=10)) {
      isOBVfriendPanelLocked=true;
      DrawControlPanel();
      ChartRedraw();
   }
   static int tickCounter=0;
   tickCounter++;
   if(tickCounter%5==0) {
      UpdateLiveTrackers();
      UpdateStatsFromHistory();
      DrawControlPanel();
      ChartRedraw();
   }
   ManageForcedSessionActions();
   EnforceTradingSession();
   EnforceBlockTimeSession();
   if(UseBlockTime&&IsTesting()) EnforceBlockTimeSession();
}
//+------------------------------------------------------------------+
//| SECTION 11 - THE SHUTDOWN ROUTINE (STATEFUL)                     |
//+------------------------------------------------------------------+
void OnDeinit(const int reason) {
   EventKillTimer();
   RemoveDotsPanel();
   RemoveDotsChartMarkers();
   ObjectsDeleteAll(0,ea_prefix);
   CleanupResources();
   ChartRedraw();
}
void RemoveUIPanels() {
   for(int i=ObjectsTotal(0,-1,-1)-1; i>=0; i--) {
      string objectName=ObjectName(0,i,-1,-1);
      if(StringFind(objectName,ea_prefix)==0) {
         if(StringFind(objectName,"btnTogglePanel")>=0) continue;
         if(StringFind(objectName,"btnSessions")>=0) continue;
         if(StringFind(objectName,"btnTrends")>=0) continue;
         if(StringFind(objectName,"btnDots")>=0) continue;
         if(StringFind(objectName,"btnPoc")>=0) continue;
         if(StringFind(objectName,"btnOBV_")>=0) continue;
         if(StringFind(objectName,"btnDump")>=0) continue;
         if(StringFind(objectName,"btnQuantToggle1")>=0) continue;
         if(StringFind(objectName,"btnQuantToggle2")>=0) continue;
         if(StringFind(objectName,"LoomsMainPanel")>=0||StringFind(objectName,"loomsTitle")>=0||
            StringFind(objectName,"wgTitle")>=0||StringFind(objectName,"btn")>=0||
            StringFind(objectName,"LoomsHeader")>=0||StringFind(objectName,"LoomsStat")>=0||
            StringFind(objectName,"TotalPerf")>=0||StringFind(objectName,"ManualTradePanel")>=0||
            StringFind(objectName,"ManualHeader")>=0||StringFind(objectName,"ManualStat")>=0||
            StringFind(objectName,"TrendStrengthPanel")>=0||StringFind(objectName,"TrendHeader")>=0||
            StringFind(objectName,"momo_block_")>=0||StringFind(objectName,"step_block_")>=0||
            StringFind(objectName,"gauge_block_")>=0||StringFind(objectName,"gauge_b_")>=0||
            StringFind(objectName,"GaugeHeader_")>=0||StringFind(objectName,"TrendAngleHeader")>=0||
            StringFind(objectName,"TrendStat")>=0||StringFind(objectName,"OBVfriendPanel")>=0||
            StringFind(objectName,"OBVfriendHeader")>=0||StringFind(objectName,"OBVfriendStat")>=0||
            StringFind(objectName,"QuantDashboardPanel")>=0||StringFind(objectName,"QuantHeader")>=0||
            StringFind(objectName,"QuantStat")>=0||
            StringFind(objectName,"SqueezePanel")>=0||StringFind(objectName,"Sqz")>=0||
            StringFind(objectName,"sqz_gauge_b_")>=0||
            StringFind(objectName,"DotsPanel")>=0||
            StringFind(objectName,"BootPanelBG")>=0||StringFind(objectName,"BootLog_")>=0)
         {
            ObjectDelete(0,objectName);
         }
      }
   }
}
void CleanupResources() {
   ArrayFree(UpTrendBuffer);
   ArrayFree(DownTrendBuffer);
   ArrayFree(LockBuffer);
   ArrayFree(LockTime);
   ArrayFree(ATR_1M_Array);
   ArrayFree(assignedATR);
   ArrayFree(UpperBand);
   ArrayFree(LowerBand);
   ArrayFree(SuperTrend);
   ArrayFree(Direction);
   ArrayFree(MomentumBuffer);
   ArrayFree(ADXBuffer);
   ArrayFree(ADX_SmoothedPlusDM);
   ArrayFree(ADX_SmoothedMinusDM);
   ArrayFree(ADX_SmoothedTR);
   ArrayFree(OBVfriend_UpTrendBuffer);
   ArrayFree(OBVfriend_DownTrendBuffer);
   ArrayFree(OBVfriend_SuperTrend);
   ArrayFree(OBVfriend_Direction);
   ArrayFree(OBV_BasisBuffer);
   ArrayFree(OBV_AtrBuffer);
   ArrayFree(OBV_AtrMaBuffer);
   ArrayFree(OBV_DirStepBuffer);
   ArrayFree(OBV_DirStepCountBuffer);
   ArrayFree(OBV_PersistBuffer);
   ArrayFree(OBV_UpperBandBuffer);
   ArrayFree(OBV_LowerBandBuffer);
   ArrayFree(U_BasisBuffer);
   ArrayFree(U_AtrBuffer);
   ArrayFree(U_AtrMaBuffer);
   ArrayFree(U_DirStepBuffer);
   ArrayFree(U_PersistBuffer);
   ArrayFree(U_UpperBandBuffer);
   ArrayFree(U_LowerBandBuffer);
   ArrayFree(U_UpCntBuffer);
   ArrayFree(U_DnCntBuffer);
   ArrayFree(hist_LT_trendStep_ST);
   ArrayFree(hist_LT_detectedSlope_ST);
   ArrayFree(hist_trendStep_ST);
   ArrayFree(hist_detectedSlope_ST);
   ArrayFree(Master_hist_LT_detectedSlope_ST);
   ArrayFree(Master_hist_detectedSlope_ST);
   ArrayFree(hist_detectedAnchorBar_ST);
   ArrayFree(hist_LT_detectedAnchorBar_ST);
   ArrayFree(hist_ADXValue);
   ArrayFree(hist_VolumeValue);
   ArrayFree(hist_ST_Flip_Event);
   ArrayFree(hist_AnchorType_ST);
   ArrayFree(hist_BarsSinceFlip_ST);
   ArrayFree(hist_AnchorType_LT);
   ArrayFree(hist_BarsSinceFlip_LT);
   ArrayFree(hist_DecayState_ST);
   ArrayFree(hist_DecayState_LT);
   ArrayFree(state_ADX);
   ArrayFree(state_Momentum);
   ArrayFree(state_D2D_Upper);
   ArrayFree(state_D2D_Lower);
   ArrayFree(state_D2D_Dir);
   ArrayFree(state_OBV_Accum);
   ArrayFree(state_OBV_Fast);
   ArrayFree(state_OBV_Slow);
   ArrayFree(state_OBV_Macd);
   ArrayFree(state_OBV_Final);
   ArrayFree(state_OBV_Velocity);
   ArrayFree(state_TChan_Sum);
   ArrayFree(state_TChan_B5);
   ArrayFree(state_TChan_OC);
   ArrayFree(state_HarmVol_LLEMA);
   ArrayFree(state_HarmVol_KAMA);
   ArrayFree(state_HarmVol_EMA8);
   ArrayFree(state_HarmVol_EMA21);
   ArrayFree(state_HarmVol_EMAOsc);
   ArrayFree(state_Sqz_BB_Basis);
   ArrayFree(state_Sqz_BB_Dev);
   ArrayFree(state_Sqz_KC_RangeMa);
   ArrayFree(state_Sqz_Detrended);
   ArrayFree(state_Sqz_Val);
   ArrayFree(state_Sqz_State);
   ArrayFree(state_RangeOsc_MA);
   ArrayFree(state_RangeOsc_ATR);
   ArrayFree(state_RangeOsc_Val);
   ArrayFree(state_RangeOsc_State);
   ArrayFree(state_Slope_EMA_ST);
   ArrayFree(state_Slope_EMA_LT);
   ArrayFree(state_Slope_Accel_ST);
   ArrayFree(state_Slope_Accel_LT);
   ArrayFree(state_Micro_IBSP);
   ArrayFree(state_Micro_Lambda);
   ArrayFree(state_Micro_TickIntensity);
   ArrayFree(state_Micro_GarmanKlass);
   ArrayFree(state_Micro_Rejection);
   ArrayFree(state_Micro_OrderFlowDelta);
   ArrayFree(state_Micro_BarEntropy);
   ArrayFree(state_Micro_LogReturn);
   ArrayFree(state_Micro_PriceAccel);
   ArrayFree(state_Micro_RollProxy);
   ArrayFree(state_Micro_BarOverlap);
   ArrayFree(state_Micro_FailedBreak);
   ArrayFree(state_Micro_MomoTransfer);
   ArrayFree(state_Micro_MicroGap);
   ArrayFree(state_Micro_HLAsymmetry);
   ArrayFree(state_Micro_VolAccel);
   ArrayFree(state_Micro_RangeVelocity);
   ArrayFree(state_Micro_RangeAccel);
   ArrayFree(state_Micro_ThrustEff);
   ArrayFree(state_Micro_AutoCorr);
   ArrayFree(state_Micro_Entropy);
   ArrayFree(state_Micro_VPIN);
   ArrayFree(state_Micro_FractalDim);
   ArrayFree(state_Micro_VolOfVol);
   ArrayFree(state_Micro_Amihud);
   ArrayFree(state_Micro_WickImbalance);
   ArrayFree(state_Micro_CSSpread);
   ArrayFree(state_Micro_Hurst);
   ArrayFree(hist_Brain_Sensitivity);
   ArrayFree(hist_Poc_Price);
   ArrayFree(hist_OBV_Zero_Value);
   ArrayFree(hist_Efficiency_Ratio);
   ArrayFree(hist_UpperWick);
   ArrayFree(hist_VolumeRatio10);
   ArrayFree(hist_KAMA_Slope);
   ArrayFree(hist_KAMA_Dist_ATR);
   ArrayFree(atr_ema1);
   ArrayFree(atr_ema2);
   ArrayFree(atr_ema3);
   ArrayFree(atr_final_val);
}
//jebEND