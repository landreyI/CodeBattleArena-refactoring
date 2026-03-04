import * as React from "react"
import * as ProgressPrimitive from "@radix-ui/react-progress"
import { cn } from "@/lib/utils"

interface ProgressProps extends React.ComponentProps<typeof ProgressPrimitive.Root> {
    value?: number
    children?: React.ReactNode
}

function Progress({ className, value, children, ...props }: ProgressProps) {
    return (
        <div className="relative w-full">
            <ProgressPrimitive.Root
                data-slot="progress"
                className={cn(
                    "bg-primary/20 h-2 w-full overflow-hidden rounded-full",
                    className
                )}
                {...props}
            >
                <ProgressPrimitive.Indicator
                    data-slot="progress-indicator"
                    className="bg-primary h-full transition-all"
                    style={{ width: `${value ?? 0}%` }}
                />
            </ProgressPrimitive.Root>

            {children && (
                <div className="absolute inset-0 flex items-center justify-center text-xs font-semibold text-white">
                    {children}
                </div>
            )}
        </div>
    )
}

export { Progress }
