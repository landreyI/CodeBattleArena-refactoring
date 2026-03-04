import { Inbox } from 'lucide-react';

interface Props {
    message: string;
}

export function EmptyState({ message }: Props) {
    return (
        <div className="flex gap-3 items-center justify-center h-auto text-gray bg-gray-100 border-l-8 border-gray rounded-2xl p-6 m-5 text-center">
            <Inbox className="w-10 h-10 animate-pulse" />
            <p className="text-xl font-extrabold">{message}</p>
        </div>
    );
}

export default EmptyState;