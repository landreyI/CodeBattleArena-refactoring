import { ReactNode } from 'react'
import {
    ChartContainer,
    ChartConfig,
    ChartTooltip,
    ChartTooltipContent,
} from '../ui/chart'

import {
    PolarGrid,
    PolarRadiusAxis,
    RadialBar,
    RadialBarChart,
    Label,
    PolarAngleAxis,
} from "recharts"

interface DataKeyConfig {
    key: string
    color?: string
}

interface MetricConfig {
    domain?: [number, number] // по умолчанию [0, 100]
    valueKey: string
    label?: string // подпись в центре
    formatValue?: (v: number) => string // форматирование числа
}

interface Props {
    chartConfig: ChartConfig
    chartData: any[]
    bars: DataKeyConfig[]
    className?: string
    children?: ReactNode
    metric: MetricConfig
}

export function RadialChart({
    chartConfig,
    chartData,
    bars,
    className,
    children,
    metric
}: Props) {
    return (
        <div className={`${className}`}>
            <ChartContainer
                config={chartConfig}
                className="mx-auto aspect-square h-full w-full"
            >
                <RadialBarChart
                    data={chartData}
                    startAngle={0}
                    endAngle={360}
                    innerRadius={80}
                    outerRadius={140}
                >
                    <PolarGrid
                        gridType="circle"
                        radialLines={false}
                        stroke="none"
                        polarRadius={[86, 74]}
                        className="first:fill-muted last:fill-background"
                    />

                    <ChartTooltip
                        cursor={false}
                        content={<ChartTooltipContent hideLabel nameKey={metric.valueKey} />}
                    />

                    <PolarAngleAxis
                        type="number"
                        tick={false}
                        domain={metric.domain || [0, 100]}
                    />

                    {bars.map(({ key, color = 'var(--color-blue)' }, index) => (
                        <RadialBar
                            key={index}
                            dataKey={key}
                            fill={color}
                            background
                        />
                    ))}

                    <PolarRadiusAxis
                        tick={false}
                        tickLine={false}
                        axisLine={false}
                    >
                        <Label
                            content={({ viewBox }) => {
                                const data = chartData[0]
                                const value = data?.[metric.valueKey]
                                if (
                                    viewBox &&
                                    "cx" in viewBox &&
                                    "cy" in viewBox &&
                                    value !== undefined
                                ) {
                                    const formattedValue = metric.formatValue
                                        ? metric.formatValue(value)
                                        : value.toLocaleString()

                                    return (
                                        <text
                                            x={viewBox.cx}
                                            y={viewBox.cy}
                                            textAnchor="middle"
                                            dominantBaseline="middle"
                                        >
                                            <tspan
                                                x={viewBox.cx}
                                                y={viewBox.cy}
                                                className="fill-foreground text-4xl font-bold"
                                            >
                                                {formattedValue}
                                            </tspan>
                                            {metric.label && (
                                                <tspan
                                                    x={viewBox.cx}
                                                    y={(viewBox.cy || 0) + 24}
                                                    className="fill-muted-foreground"
                                                >
                                                    {metric.label}
                                                </tspan>
                                            )}
                                        </text>
                                    )
                                }
                                return null
                            }}
                        />
                    </PolarRadiusAxis>
                </RadialBarChart>
            </ChartContainer>

            {children}
        </div>
    )
}

export default RadialChart
