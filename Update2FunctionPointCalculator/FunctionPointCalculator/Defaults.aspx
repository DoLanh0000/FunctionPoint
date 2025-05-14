<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Defaults.aspx.cs" Inherits="FunctionPointCalculator.Defaults" Async="true" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Function Point Calculator</title>
    <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/css/bootstrap.min.css" rel="stylesheet" />
    <link href="https://fonts.googleapis.com/css2?family=Roboto:wght@400;500;700&display=swap" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css" rel="stylesheet" />
    <style>
        body {
            font-family: 'Roboto', sans-serif;
            background: linear-gradient(135deg, #E5E7EB 0%, #D1D5DB 100%);
            color: #333;
            font-weight: 500;
        }
        .container {
            margin-top: 50px;
            max-width: 1000px;
            padding: 20px;
        }
        .card {
            border: none;
            border-radius: 15px;
            box-shadow: 0 5px 15px rgba(0,0,0,0.05);
            background: #fff;
            transition: transform 0.3s ease;
        }
        .card:hover {
            transform: translateY(-5px);
        }
        .card-header {
            background: linear-gradient(90deg, #A1A1A1 0%, #D4D4D2 100%);
            color: #333;
            font-weight: 600;
            border-radius: 15px 15px 0 0;
            padding: 15px;
            font-size: 1.3rem;
        }
        .btn-custom {
            font-size: 1rem;
            padding: 10px 25px;
            border-radius: 25px;
            transition: all 0.3s ease;
            position: relative;
            background: linear-gradient(90deg, #3B82F6 0%, #60A5FA 100%);
            border: none;
            color: white;
            font-weight: 500;
        }
        .btn-custom:hover {
            transform: scale(1.05);
            box-shadow: 0 5px 15px rgba(0,0,0,0.2);
            background: linear-gradient(90deg, #2563EB 0%, #3B82F6 100%);
        }
        .btn-custom.loading::after {
            content: "\f1ce";
            font-family: "Font Awesome 5 Free";
            font-weight: 900;
            position: absolute;
            right: 10px;
            animation: spin 1s linear infinite;
        }
        @keyframes spin {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
        }
        .form-label {
            font-size: 0.9rem;
            color: #6B7280;
            margin-bottom: 3px;
            font-weight: 400;
        }
        .result {
            font-weight: 600;
            color: #3B82F6;
            font-size: 1.3rem;
            transition: opacity 0.3s ease;
        }
        .table th, .table td {
            padding: 12px;
            vertical-align: middle;
            border-color: #D4D4D2;
            font-size: 1rem;
        }
        .table thead th {
            background: #E5E7EB;
            color: #6B7280;
            font-weight: 600;
        }
        .table tbody tr:hover {
            background-color: #F3F4F6;
        }
        .form-group {
            margin-bottom: 12px;
        }
        .vaf-label {
            font-size: 1rem;
            color: #6B7280;
            display: inline-block;
            width: 200px;
            font-weight: 500;
        }
        .vaf-input {
            width: 60px;
            display: inline-block;
            border-radius: 5px;
            border: 1px solid #D4D4D2;
            padding: 5px;
            background: #F9FAFB;
            font-size: 1rem;
        }
        .weight-column {
            background: linear-gradient(90deg, #D4D4D2 0%, #A1A1A1 100%);
            font-weight: 600;
            color: #333;
        }
        .icon {
            margin-right: 8px;
            color: #3B82F6;
        }
        .form-control:focus {
            border-color: #3B82F6;
            box-shadow: 0 0 5px rgba(59, 130, 246, 0.5);
        }
        .form-control::placeholder {
            color: #A1A1A1;
            font-style: italic;
            font-weight: 400;
        }
        .tooltip-inner {
            background-color: #3B82F6;
            color: white;
            font-size: 0.9rem;
        }
        .tooltip .arrow::before {
            border-top-color: #3B82F6;
        }
        #chartContainer {
            display: none;
            margin-top: 20px;
            height: 500px;
        }
        #chartCanvas {
            max-height: 450px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h1 class="text-center mb-3" style="font-size: 2.8rem; color: #333; font-weight: 700;">
                <i class="fas fa-calculator icon"></i>Function Point Calculator
            </h1>
            <p class="text-center text-muted mb-5" style="font-size: 1.2rem; font-weight: 400;">Estimate software size with precision and efficiency</p>

            <!-- Phần tải lên file SRS -->
            <div class="card mb-5">
                <div class="card-header">
                    <i class="fas fa-file-upload icon"></i>Upload Software Requirements Specification (SRS)
                </div>
                <div class="card-body">
                    <div class="form-group">
                        <label class="form-label">Upload SRS Document (PDF, Text, or JPG)</label>
                        <asp:FileUpload ID="fileSrsUpload" runat="server" CssClass="form-control" />
                    </div>
                    <asp:Button ID="btnAnalyzeSrs" runat="server" Text="Analyze with AI" OnClick="BtnAnalyzeSrs_Click" CssClass="btn btn-info btn-custom" OnClientClick="this.classList.add('loading');" />
                </div>
            </div>

            <!-- Bảng nhập EI, EO, EQ, ILF, EIF -->
            <div class="card mb-5">
                <div class="card-header">
                    <i class="fas fa-cogs icon"></i>Unadjusted Function Points (UFP)
                </div>
                <div class="card-body">
                    <table class="table table-bordered">
                        <thead>
                            <tr>
                                <th>Element</th>
                                <th colspan="3" style="background: linear-gradient(90deg, #6B7280 0%, #9CA3AF 100%); color: white;">Complexity Weighting Factor</th>
                                <th>Low</th>
                                <th>Average</th>
                                <th>High</th>
                            </tr>
                            <tr>
                                <th></th>
                                <th class="weight-column">Low (Simple)</th>
                                <th class="weight-column">Average</th>
                                <th class="weight-column">High (Complex)</th>
                                <th></th>
                                <th></th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td>
                                    <span data-toggle="tooltip" data-placement="top" title="Inputs that enter the system from external sources">
                                        External Inputs (EI)<br />
                                        <small class="form-label">Data entering the system</small>
                                    </span>
                                </td>
                                <td class="weight-column">3</td>
                                <td class="weight-column">4</td>
                                <td class="weight-column">6</td>
                                <td><asp:TextBox ID="txtEiLow" runat="server" CssClass="form-control" Width="60px" Text=""></asp:TextBox></td>
                                <td><asp:TextBox ID="txtEiAvg" runat="server" CssClass="form-control" Width="60px" Text=""></asp:TextBox></td>
                                <td><asp:TextBox ID="txtEiHigh" runat="server" CssClass="form-control" Width="60px" Text=""></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td>
                                    <span data-toggle="tooltip" data-placement="top" title="Outputs generated by the system with processing">
                                        External Outputs (EO)<br />
                                        <small class="form-label">Data output with processing</small>
                                    </span>
                                </td>
                                <td class="weight-column">4</td>
                                <td class="weight-column">5</td>
                                <td class="weight-column">7</td>
                                <td><asp:TextBox ID="txtEoLow" runat="server" CssClass="form-control" Width="60px" Text=""></asp:TextBox></td>
                                <td><asp:TextBox ID="txtEoAvg" runat="server" CssClass="form-control" Width="60px" Text=""></asp:TextBox></td>
                                <td><asp:TextBox ID="txtEoHigh" runat="server" CssClass="form-control" Width="60px" Text=""></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td>
                                    <span data-toggle="tooltip" data-placement="top" title="Simple data retrieval operations">
                                        External Inquiries (EQ)<br />
                                        <small class="form-label">Simple data retrieval</small>
                                    </span>
                                </td>
                                <td class="weight-column">3</td>
                                <td class="weight-column">4</td>
                                <td class="weight-column">6</td>
                                <td><asp:TextBox ID="txtEqLow" runat="server" CssClass="form-control" Width="60px" Text=""></asp:TextBox></td>
                                <td><asp:TextBox ID="txtEqAvg" runat="server" CssClass="form-control" Width="60px" Text=""></asp:TextBox></td>
                                <td><asp:TextBox ID="txtEqHigh" runat="server" CssClass="form-control" Width="60px" Text=""></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td>
                                    <span data-toggle="tooltip" data-placement="top" title="Data referenced from external systems">
                                        External Interface Files (EIF)<br />
                                        <small class="form-label">External data referenced</small>
                                    </span>
                                </td>
                                <td class="weight-column">5</td>
                                <td class="weight-column">7</td>
                                <td class="weight-column">10</td>
                                <td><asp:TextBox ID="txtEifLow" runat="server" CssClass="form-control" Width="60px" Text=""></asp:TextBox></td>
                                <td><asp:TextBox ID="txtEifAvg" runat="server" CssClass="form-control" Width="60px" Text=""></asp:TextBox></td>
                                <td><asp:TextBox ID="txtEifHigh" runat="server" CssClass="form-control" Width="60px" Text=""></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td>
                                    <span data-toggle="tooltip" data-placement="top" title="Data maintained within the system">
                                        Internal Logical Files (ILF)<br />
                                        <small class="form-label">Internal data maintained</small>
                                    </span>
                                </td>
                                <td class="weight-column">7</td>
                                <td class="weight-column">10</td>
                                <td class="weight-column">15</td>
                                <td><asp:TextBox ID="txtIlfLow" runat="server" CssClass="form-control" Width="60px" Text=""></asp:TextBox></td>
                                <td><asp:TextBox ID="txtIlfAvg" runat="server" CssClass="form-control" Width="60px" Text=""></asp:TextBox></td>
                                <td><asp:TextBox ID="txtIlfHigh" runat="server" CssClass="form-control" Width="60px" Text=""></asp:TextBox></td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>

            <!-- Bảng nhập VAF -->
            <div class="card mb-5">
                <div class="card-header">
                    <i class="fas fa-sliders-h icon"></i>Value Adjustment Factor (VAF)
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="vaf-label">F1 - Data Communications</label>
                                <asp:TextBox ID="txtF1" runat="server" CssClass="form-control vaf-input" Text=""></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="vaf-label">F2 - Distributed Data Processing</label>
                                <asp:TextBox ID="txtF2" runat="server" CssClass="form-control vaf-input" Text=""></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="vaf-label">F3 - Performance</label>
                                <asp:TextBox ID="txtF3" runat="server" CssClass="form-control vaf-input" Text=""></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="vaf-label">F4 - Heavily Used Configuration</label>
                                <asp:TextBox ID="txtF4" runat="server" CssClass="form-control vaf-input" Text=""></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="vaf-label">F5 - Transaction Rate</label>
                                <asp:TextBox ID="txtF5" runat="server" CssClass="form-control vaf-input" Text=""></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="vaf-label">F6 - On-line Data Entry</label>
                                <asp:TextBox ID="txtF6" runat="server" CssClass="form-control vaf-input" Text=""></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="vaf-label">F7 - End-User Efficiency</label>
                                <asp:TextBox ID="txtF7" runat="server" CssClass="form-control vaf-input" Text=""></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="vaf-label">F8 - On-line Update</label>
                                <asp:TextBox ID="txtF8" runat="server" CssClass="form-control vaf-input" Text=""></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="vaf-label">F9 - Complex Processing</label>
                                <asp:TextBox ID="txtF9" runat="server" CssClass="form-control vaf-input" Text=""></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="vaf-label">F10 - Reusability</label>
                                <asp:TextBox ID="txtF10" runat="server" CssClass="form-control vaf-input" Text=""></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="vaf-label">F11 - Installation Ease</label>
                                <asp:TextBox ID="txtF11" runat="server" CssClass="form-control vaf-input" Text=""></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="vaf-label">F12 - Operational Ease</label>
                                <asp:TextBox ID="txtF12" runat="server" CssClass="form-control vaf-input" Text=""></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="vaf-label">F13 - Multiple Sites</label>
                                <asp:TextBox ID="txtF13" runat="server" CssClass="form-control vaf-input" Text=""></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="vaf-label">F14 - Facilitate Change</label>
                                <asp:TextBox ID="txtF14" runat="server" CssClass="form-control vaf-input" Text=""></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Nút tính toán, reset và xuất báo cáo -->
            <div class="text-center mb-5">
                <asp:Button ID="btnCalculate" runat="server" Text="Calculate FP" OnClick="BtnCalculate_Click" CssClass="btn btn-primary btn-custom mr-3" OnClientClick="this.classList.add('loading');" />
                <asp:Button ID="btnReset" runat="server" Text="Reset" OnClick="BtnReset_Click" CssClass="btn btn-secondary btn-custom mr-3" />
                <asp:Button ID="btnExport" runat="server" Text="Export PDF Report" OnClick="BtnExport_Click" CssClass="btn btn-success btn-custom" />
            </div>

            <!-- Kết quả -->
            <div class="card mb-5">
                <div class="card-header">
                    <i class="fas fa-chart-line icon"></i>Calculation Results
                </div>
                <div class="card-body">
                    <asp:Label ID="lblUfp" runat="server" CssClass="result d-block"></asp:Label>
                    <asp:Label ID="lblVaf" runat="server" CssClass="result d-block"></asp:Label>
                    <asp:Label ID="lblFp" runat="server" CssClass="result d-block"></asp:Label>
                </div>
            </div>

            <!-- Nút View Chart -->
            <div class="text-center mb-5">
                <asp:Button ID="btnViewChart" runat="server" Text="View Chart" CssClass="btn btn-warning btn-custom" OnClientClick="toggleChart(); return false;" />
            </div>

            <!-- Biểu đồ riêng -->
            <div class="card mb-5">
                <div class="card-header">
                    <i class="fas fa-chart-bar icon"></i>Function Point Chart
                </div>
                <div class="card-body">
                    <div id="chartContainer">
                        <canvas id="chartCanvas" style="max-height: 450px;"></canvas>
                    </div>
                </div>
            </div>

            <!-- Lịch sử tính toán -->
            <div class="card mb-5">
                <div class="card-header">
                    <i class="fas fa-history icon"></i>Calculation History
                </div>
                <div class="card-body">
                    <asp:Button ID="btnViewHistory" runat="server" Text="View History" OnClick="BtnViewHistory_Click" CssClass="btn btn-info btn-custom mb-3" OnClientClick="this.classList.add('loading');" />
                    <asp:GridView ID="gvHistory" runat="server" CssClass="table table-bordered" AutoGenerateColumns="False" Visible="False" OnRowEditing="gvHistory_RowEditing" OnRowDeleting="gvHistory_RowDeleting" OnRowCancelingEdit="gvHistory_RowCancelingEdit" OnRowUpdating="gvHistory_RowUpdating">
                        <Columns>
                            <asp:BoundField DataField="Id" HeaderText="ID" ReadOnly="True" />
                            <asp:BoundField DataField="UFP" HeaderText="UFP" DataFormatString="{0:F2}" />
                            <asp:BoundField DataField="VAF" HeaderText="VAF" DataFormatString="{0:F2}" />
                            <asp:BoundField DataField="FP" HeaderText="FP" DataFormatString="{0:F2}" />
                            <asp:BoundField DataField="CalculationDate" HeaderText="Calculation Date" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" ReadOnly="True" />
                            <asp:BoundField DataField="SrsFileName" HeaderText="SRS File Name" />
                            <asp:CommandField ShowEditButton="True" ShowDeleteButton="True" ButtonType="Button" ControlStyle-CssClass="btn btn-sm btn-custom" />
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </form>
    <script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.4/dist/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/js/bootstrap.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/chart.js@3.9.1/dist/chart.min.js"></script>
    <script>
        $(document).ready(function () {
            $('[data-toggle="tooltip"]').tooltip();
            $('#<%= btnAnalyzeSrs.ClientID %>').click(function () {
                $(this).addClass('loading');
            });
            $('#<%= btnViewHistory.ClientID %>').click(function () {
                $(this).addClass('loading');
            });
        });

        let fpChart = null;

        function toggleChart() {
            const chartContainer = document.getElementById('chartContainer');
            if (chartContainer.style.display === 'none' || chartContainer.style.display === '') {
                chartContainer.style.display = 'block';
                loadChartData();
            } else {
                chartContainer.style.display = 'none';
                if (fpChart) {
                    fpChart.destroy();
                    fpChart = null;
                }
            }
        }

        function loadChartData() {
            // First, check if chartData is already available from server-side
            if (window.chartData && !window.chartData.error) {
                renderChart(window.chartData);
                return;
            }

            // If not, try AJAX call
            $.ajax({
                type: 'POST',
                url: 'Defaults.aspx/GetChartData',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (response) {
                    const data = response.d;
                    if (data && !data.error) {
                        window.chartData = JSON.parse(data);
                        renderChart(window.chartData);
                    } else {
                        alert('Error loading chart data: ' + (data ? data.error : 'No data returned'));
                    }
                },
                error: function (xhr, status, error) {
                    alert('Error loading chart data via AJAX: ' + error + '. Status: ' + xhr.status + ' - ' + xhr.statusText + '. Please click "View History" first.');
                }
            });
        }

        function renderChart(data) {
            const ctx = document.getElementById('chartCanvas').getContext('2d');
            if (fpChart) {
                fpChart.destroy();
            }

            // Create gradient colors for bars
            const gradientColors = data.map((_, index) => {
                const gradient = ctx.createLinearGradient(0, 0, 0, 400);
                const hue = (index * 40) % 360; // Vary hue for each bar
                gradient.addColorStop(0, `hsla(${hue}, 70%, 60%, 0.8)`);
                gradient.addColorStop(1, `hsla(${hue}, 70%, 40%, 0.8)`);
                return gradient;
            });

            fpChart = new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: data.map(item => `Project ${item.Id}\n(${new Date(item.CalculationDate).toLocaleDateString('en-GB', { day: '2-digit', month: '2-digit', year: '2-digit', hour: '2-digit', minute: '2-digit' })})`),
                    datasets: [{
                        label: 'Function Points (FP)',
                        data: data.map(item => item.FP),
                        backgroundColor: gradientColors,
                        borderColor: gradientColors.map(color => color),
                        borderWidth: 1,
                        barPercentage: 0.7,
                        categoryPercentage: 0.8
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    scales: {
                        y: {
                            beginAtZero: true,
                            title: {
                                display: true,
                                text: 'Function Points (FP)',
                                font: { size: 16, weight: 'bold', family: 'Roboto' },
                                color: '#333'
                            },
                            ticks: {
                                font: { size: 14, family: 'Roboto' },
                                color: '#666',
                                stepSize: 50
                            },
                            grid: {
                                color: 'rgba(0, 0, 0, 0.1)',
                                borderDash: [5, 5]
                            }
                        },
                        x: {
                            title: {
                                display: true,
                                text: 'Projects (Date & Time)',
                                font: { size: 16, weight: 'bold', family: 'Roboto' },
                                color: '#333'
                            },
                            ticks: {
                                font: { size: 12, family: 'Roboto' },
                                color: '#666',
                                maxRotation: 45,
                                minRotation: 45,
                                padding: 10
                            },
                            grid: {
                                display: false
                            }
                        }
                    },
                    plugins: {
                        legend: {
                            display: true,
                            position: 'top',
                            labels: {
                                font: { size: 14, family: 'Roboto' },
                                color: '#333',
                                padding: 20
                            }
                        },
                        title: {
                            display: true,
                            text: 'Function Point Analysis Over Time',
                            font: { size: 18, weight: 'bold', family: 'Roboto' },
                            color: '#333',
                            padding: { top: 10, bottom: 20 }
                        },
                        tooltip: {
                            backgroundColor: 'rgba(0, 0, 0, 0.8)',
                            titleFont: { size: 16, family: 'Roboto' },
                            bodyFont: { size: 14, family: 'Roboto' },
                            padding: 10,
                            cornerRadius: 5
                        },
                        datalabels: {
                            display: true,
                            anchor: 'end',
                            align: 'top',
                            font: { size: 12, weight: 'bold', family: 'Roboto' },
                            color: '#333',
                            formatter: (value) => value.toFixed(2)
                        }
                    },
                    layout: {
                        padding: { left: 30, right: 30, top: 30, bottom: 30 }
                    }
                },
                plugins: [ChartDataLabels]
            });
        }
    </script>
    <script src="https://cdn.jsdelivr.net/npm/chartjs-plugin-datalabels@2.0.0/dist/chartjs-plugin-datalabels.min.js"></script>
</head>
</html>