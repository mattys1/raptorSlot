interface Wager {
    wagerAmount: number;
    isPremiumToken: boolean;
}
interface DrawResult {
    value: number
}
interface PlayResponse {
    wager: Wager 
    draw: DrawResult
}

const Play = async (wagerAmount: number, betType: number, selected: number[]): Promise<{ok: boolean; body: PlayResponse | string}> => {
    const payload = {
        wager: { wagerAmount, isPremiumToken: false },
        rouletteChoice: { betType, selected },
    };

    const response = await fetch('/api/games/roulette/play', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    });
    
    const data: PlayResponse | string = response.ok ? await response.json() : await response.text();

    return {ok: response.ok, body: data}
}
