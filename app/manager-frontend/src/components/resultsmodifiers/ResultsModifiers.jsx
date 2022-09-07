import {
    StackDivider,
    VStack,
    Grid,
    Box,
    GridItem,
    Text,
    Button,
    useDisclosure
} from "@chakra-ui/react";
import { DragDropContext, Draggable, Droppable } from 'react-beautiful-dnd';
import { useBackendApi } from "contexts/BackendApi";
import { useActivitySourceResultsModifiersList } from "api/activitysources"
import { useState } from "react";
import { FaGripVertical } from "react-icons/fa";



export const ResultsModifiers = ({
    id
}) => {

    const { resultsmodifier } = useBackendApi();
    const { data, mutate } = useActivitySourceResultsModifiersList(id);

    const onDragEnd = async (result) => {
        // dropped outside the list
        if (!result.destination) {
            return;
        }

        await resultsmodifier.putOrder({ position: result.destination.index + 1, id: result.draggableId })

        await mutate();
    };


    // const {
    //     isOpen: isDeleteOpen,
    //     onOpen: onDeleteOpen,
    //     onClose: onDeleteClose,
    // } = useDisclosure();
    // const {
    //     isOpen: isUpdateOpen,
    //     onOpen: onUpdateOpen,
    //     onClose: onUpdateClose,
    // } = useDisclosure();

    // const [selected, setSelected] = useState();

    // const onDelete = async () => {
    //     console.log()
    //     await resultsmodifier.delete({ id: id });
    //     await mutate();
    //     setSelected(undefined);
    //     onDeleteClose();
    // };
    // const onClickDelete = (item) => {
    //     setSelected(item);
    //     onDeleteOpen();
    // };
    // const closeDelete = () => {
    //     onDeleteClose();
    //     setSelected(undefined);
    // };
    // const closeUpdate = () => {
    //     onUpdateClose();
    //     setSelected(undefined);
    // };
    // const onClickUpdate = (item) => {
    //     setSelected(item);
    //     onUpdateOpen();
    // };
    return (

        <VStack
            bg="whiteAlpha"
            borderColor="gray.300"
            borderWidth={1}
            borderRadius={5}
            h="100%"
            p={5}
            align="stretch"
            // divider={<StackDivider borderColor='blackAlpha.700' />}
            spacing={4}
            display='grid'
        >
            <Text fontWeight={"bold"} pr={4}>
                Results Modifiers:
            </Text>
            <Grid
                templateAreas={`"header header"
                "nav main"
                "nav footer"`}
                templateColumns='repeat(1,1fr)'
                gap={1}
                display={'grid'}

            >
                <DragDropContext onDragEnd={onDragEnd} >
                    <Droppable droppableId="droppable">
                        {(provided, snapshot) => (
                            <div
                                {...provided.droppableProps}
                                ref={provided.innerRef}

                            >
                                <Grid
                                    templateColumns={'repeat(3,1fr)'}
                                    display={'grid'}
                                    pb={2}

                                >
                                    <GridItem colStart={1} colSpan={1} area={'nav'} pl={2}>
                                        <Text fontWeight={'bold'} > Order </Text>
                                    </GridItem>
                                    <GridItem colStart={2} colSpan={1} area={'nav'} pl={5}>
                                        <Text fontWeight={'bold'} > Type </Text>
                                    </GridItem>
                                    <GridItem colStart={3} colSpan={1} area={'nav'} pl={5}>
                                        <Text fontWeight={'bold'} > Parameters </Text>
                                    </GridItem>
                                </Grid>


                                {data.sort((item, index) => item.order - index.order).map((item, index) => (
                                    <Draggable key={item.id} draggableId={String(item.id)} index={index} >
                                        {(provided, snapshot) => (
                                            <div
                                                ref={provided.innerRef}
                                                {...provided.draggableProps}
                                                {...provided.dragHandleProps}

                                            >
                                                <Grid
                                                    templateColumns={'repeat(3,1fr)'}
                                                    display={'grid'}
                                                    pb={2}
                                                    borderRadius={5}
                                                >


                                                    <GridItem
                                                        colStart={1}
                                                        colSpan={1}
                                                        bg={snapshot.isDragging ? 'blue.100' : 'gray.100'}
                                                        h='10'
                                                        pl={2}
                                                        display='flex'


                                                    >

                                                        <FaGripVertical display={'flex'} /><Text display='flex'> {item.order} </Text>
                                                    </GridItem>

                                                    <GridItem
                                                        colStart={2}
                                                        colSpan={1}
                                                        bg={snapshot.isDragging ? 'blue.100' : 'gray.100'}
                                                        h='10'
                                                        pl={5}>

                                                        <Text p={'1'}>{item.type.id}</Text>
                                                    </GridItem>
                                                    <GridItem
                                                        colStart={3}
                                                        colSpan={1}
                                                        bg={snapshot.isDragging ? 'blue.100' : 'gray.100'}
                                                        h='10'
                                                        pl={5}>
                                                        <Text p={'1'}>{item.parameters}</Text>

                                                    </GridItem>


                                                </Grid>



                                            </div>
                                        )}
                                    </Draggable>


                                ))}
                                {provided.placeholder}

                            </div>


                        )}

                    </Droppable>
                </DragDropContext>
            </Grid>
        </VStack >
    )
};