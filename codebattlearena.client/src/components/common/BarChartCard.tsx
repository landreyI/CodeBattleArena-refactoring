import { ReactNode } from 'react'
import { Card, CardContent, CardFooter, CardHeader } from '../ui/card'
import {
    ChartContainer,
    ChartConfig,
    ChartTooltip,
    ChartTooltipContent
} from '../ui/chart'
import { BarChart, Bar, CartesianGrid, XAxis } from 'recharts'

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

export function BarChartCard({
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
                    <BarChart data={chartData}>
                        <CartesianGrid vertical={false} />
                        <XAxis
                            dataKey={xAxisKey}
                            tickLine={false}
                            tickMargin={10}
                        />
                        <ChartTooltip content={<ChartTooltipContent />} />

                        {bars.map(({ key, color = 'var(--color-blue)' }, index) => (
                            <Bar
                                key={index}
                                dataKey={key}
                                fill={color}
                                radius={4}
                            />
                        ))}
                    </BarChart>
                </ChartContainer>
            </CardContent>

            {children && <CardFooter>{children}</CardFooter>}
        </Card>
    )
}

export default BarChartCard
