import { useState } from "react";
import { z } from "zod";
import { Player } from "@/models/dbModels";
import { fetchEditPlayer } from "@/services/player";
import { formSchema } from "@/components/forms/EditPlayerForm";
import { StandardError, processError } from "@/untils/errorHandler";

export function useEditPlayer() {
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState<StandardError | null>(null);

    const editPlayer = async (
        player: Player,
        values: z.infer<typeof formSchema>
    ): Promise<Player> => {
        setIsLoading(true);
        setError(null);

        try {
            const updatedPlayer: Player = {
                ...player,
                username: values.username,
                additionalInformation: values.additionalInformation ?? "",
            };

            await fetchEditPlayer(updatedPlayer);

            return updatedPlayer;
        } catch (err: unknown) {
            const standardError: StandardError = processError(err);
            setError(standardError);
            throw standardError;
        } finally {
            setIsLoading(false);
        }
    };

    return { editPlayer, isLoading, error };
}
