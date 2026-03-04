import { ReactNode } from 'react'
import { Card, CardContent, CardFooter, CardHeader } from '../ui/card'
import {
    ChartContainer,
    ChartConfig,
    ChartTooltip,
    ChartTooltipContent
} from '../ui/chart'
import { CartesianGrid, RadarChart, PolarAngleAxis, Radar, PolarGrid } from 'recharts'

interface DataKeyConfig {
    key: string
    color?: string
}

interface Props {
    title: string
    chartConfig: ChartConfig
    chartData: any[]
    xAxisKey: string
    bars: DataKeyConfig[]
    className?: string,
    chartClassName?: string,
    children?: ReactNode
}

export function RadarChartCard({
    title,
    chartConfig,
    chartData,
    xAxisKey,
    bars,
    className,
    chartClassName,
    children
}: Props) {
    return (
        <Card className={`w-full h-full p-4 ${className}`}>
            <CardHeader className="text-md font-semibold">{title}</CardHeader>

            <CardContent className="p-0">
                <ChartContainer config={chartConfig} className={`h-[40vh] w-full ${chartClassName}`}>
                    <RadarChart data={chartData}>
                        <ChartTooltip cursor={false} content={<ChartTooltipContent />} />
                        <PolarGrid />
                        <PolarAngleAxis dataKey={xAxisKey} />

                        {bars.map(({ key, color = 'var(--color-blue)' }, index) => (
                            <Radar
                                key={index}
                                dataKey={key}
                                fill={color}
                                radius={4}
                                fillOpacity={0.6}
                            />
                        ))}
                    </RadarChart>
                </ChartContainer>
            </CardContent>

            {children && <CardFooter>{children}</CardFooter>}
        </Card>
    )
}

export default RadarChartCard
